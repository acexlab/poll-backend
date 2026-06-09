using Microsoft.EntityFrameworkCore;
using pollbackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pollbackend.Data
{
    public class VoteRepository : IVoteRepository
    {
        private readonly PollDbContext _context;

        public VoteRepository(PollDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Vote vote)
        {
            await _context.Votes.AddAsync(vote);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vote vote)
        {
            _context.Votes.Update(vote);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasUserVotedOnPollAsync(long userId, long pollId)
        {
            return await _context.Votes.AnyAsync(x => x.UserId == userId && x.PollId == pollId);
        }

        public async Task<Vote?> GetUserVoteOnPollAsync(long userId, long pollId)
        {
            return await _context.Votes
                .Select(x => new Vote
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    PollId = x.PollId,
                    PollOptionId = x.PollOptionId,
                    VotedAt = x.VotedAt
                })
                .FirstOrDefaultAsync(x => x.UserId == userId && x.PollId == pollId);
        }

        public async Task<List<Vote>> GetVotesByPollIdAsync(long pollId)
        {
            return await _context.Votes
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.PollOption)
                .Where(x => x.PollId == pollId)
                .ToListAsync();
        }

        public async Task<int> GetTotalVotesCountAsync()
        {
            return await _context.Votes.CountAsync();
        }

        public async Task<List<Vote>> GetUserVotesAsync(long userId)
        {
            return await _context.Votes
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => new Vote
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    PollId = x.PollId,
                    PollOptionId = x.PollOptionId,
                    VotedAt = x.VotedAt
                })
                .ToListAsync();
        }

        public async Task<Vote?> GetByIdAsync(long id)
        {
            return await _context.Votes.FindAsync(id);
        }

        public async Task DeleteAsync(long id)
        {
            var vote = await _context.Votes.FindAsync(id);
            if (vote != null)
            {
                _context.Votes.Remove(vote);
                await _context.SaveChangesAsync();
            }
        }
    }
}
