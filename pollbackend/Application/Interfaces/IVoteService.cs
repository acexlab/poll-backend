using System.Collections.Generic;
using System.Threading.Tasks;
using pollbackend.Application.DTOs;

namespace pollbackend.Application.Interfaces
{
    public interface IVoteService
    {
        Task<bool> CastVoteAsync(CastVoteDto castVoteDto, long userId);
        Task<PollResultDto?> GetResultsAsync(long pollId);
        Task<UserVoteDto?> GetUserVoteOnPollAsync(long userId, long pollId);
        Task<List<UserVoteDto>> GetUserVotesAsync(long userId);
    }
}
