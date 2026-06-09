using System.Collections.Generic;
using System.Threading.Tasks;
using pollbackend.Dtos;

namespace pollbackend.Services
{
    public interface IVoteService
    {
        Task<bool> CastVoteAsync(CastVoteDto castVoteDto, long userId);
        Task<PollResultDto?> GetResultsAsync(long pollId);
        Task<UserVoteDto?> GetUserVoteOnPollAsync(long userId, long pollId);
        Task<List<UserVoteDto>> GetUserVotesAsync(long userId);
        Task<bool> DeleteVoteAsync(long id);
        Task<bool> AlterVoteAsync(long voteId, long newOptionId);
    }
}
