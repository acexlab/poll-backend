using pollbackend.Application.DTOs;
using pollbackend.Application.Interfaces;
using pollbackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pollbackend.Application.Services
{
    public class VoteService : IVoteService
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IPollRepository _pollRepository;

        public VoteService(IVoteRepository voteRepository, IPollRepository pollRepository)
        {
            _voteRepository = voteRepository;
            _pollRepository = pollRepository;
        }

        public async Task<bool> CastVoteAsync(CastVoteDto castVoteDto, long userId)
        {
            var poll = await _pollRepository.GetByIdAsync(castVoteDto.PollId);
            if (poll == null)
            {
                throw new KeyNotFoundException("Poll not found.");
            }

            if (!poll.IsEnabled)
            {
                throw new InvalidOperationException("Voting is closed for this poll.");
            }

            // Verify if the option belongs to this poll
            if (poll.Options.All(o => o.Id != castVoteDto.PollOptionId))
            {
                throw new ArgumentException("Selected option does not belong to this poll.");
            }

            // Enforce unique voting rule: check if user already voted in this poll
            bool alreadyVoted = await _voteRepository.HasUserVotedOnPollAsync(userId, castVoteDto.PollId);
            if (alreadyVoted)
            {
                throw new InvalidOperationException("You have already voted in this poll.");
            }

            var vote = new Vote
            {
                UserId = userId,
                PollId = castVoteDto.PollId,
                PollOptionId = castVoteDto.PollOptionId,
                VotedAt = DateTime.UtcNow
            };

            await _voteRepository.AddAsync(vote);
            return true;
        }

        public async Task<PollResultDto?> GetResultsAsync(long pollId)
        {
            var poll = await _pollRepository.GetByIdAsync(pollId);
            if (poll == null)
            {
                return null;
            }

            var votes = await _voteRepository.GetVotesByPollIdAsync(pollId);
            int totalVotes = votes.Count;

            var results = poll.Options.Select(opt =>
            {
                int count = votes.Count(v => v.PollOptionId == opt.Id);
                double percentage = totalVotes > 0 
                    ? Math.Round((double)count / totalVotes * 100, 2) 
                    : 0.0;

                return new VoteResultItemDto
                {
                    OptionId = opt.Id,
                    OptionText = opt.OptionText,
                    VoteCount = count,
                    Percentage = percentage
                };
            }).ToList();

            return new PollResultDto
            {
                PollId = poll.Id,
                PollTitle = poll.Title,
                TotalVotes = totalVotes,
                Results = results
            };
        }

        public async Task<UserVoteDto?> GetUserVoteOnPollAsync(long userId, long pollId)
        {
            var poll = await _pollRepository.GetByIdAsync(pollId);
            if (poll == null)
            {
                return null;
            }

            var vote = await _voteRepository.GetUserVoteOnPollAsync(userId, pollId);
            if (vote == null)
            {
                return null;
            }

            var option = poll.Options.FirstOrDefault(o => o.Id == vote.PollOptionId);

            return new UserVoteDto
            {
                PollId = poll.Id,
                PollTitle = poll.Title,
                VotedOptionId = vote.PollOptionId,
                VotedOptionText = option?.OptionText ?? "Unknown Option",
                VotedAt = vote.VotedAt
            };
        }

        public async Task<List<UserVoteDto>> GetUserVotesAsync(long userId)
        {
            var votes = await _voteRepository.GetUserVotesAsync(userId);
            if (votes.Count == 0)
            {
                return new List<UserVoteDto>();
            }

            var userVotes = new List<UserVoteDto>();
            foreach (var vote in votes)
            {
                var poll = await _pollRepository.GetByIdAsync(vote.PollId);
                if (poll != null)
                {
                    var option = poll.Options.FirstOrDefault(o => o.Id == vote.PollOptionId);
                    userVotes.Add(new UserVoteDto
                    {
                        PollId = vote.PollId,
                        PollTitle = poll.Title,
                        VotedOptionId = vote.PollOptionId,
                        VotedOptionText = option?.OptionText ?? "Unknown Option",
                        VotedAt = vote.VotedAt
                    });
                }
            }

            return userVotes;
        }
    }
}
