using System.Collections.Generic;
using System.Threading.Tasks;
using pollbackend.Models;

namespace pollbackend.Data
{
    public interface IVoteRepository
    {
        Task AddAsync(Vote vote);
        Task UpdateAsync(Vote vote);
        Task<bool> HasUserVotedOnPollAsync(long userId, long pollId);
        Task<Vote?> GetUserVoteOnPollAsync(long userId, long pollId);
        Task<List<Vote>> GetVotesByPollIdAsync(long pollId);
        Task<int> GetTotalVotesCountAsync();
        Task<List<Vote>> GetUserVotesAsync(long userId);
        Task<Vote?> GetByIdAsync(long id);
        Task DeleteAsync(long id);
    }
}
