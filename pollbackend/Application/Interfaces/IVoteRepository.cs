using System.Collections.Generic;
using System.Threading.Tasks;
using pollbackend.Domain.Entities;

namespace pollbackend.Application.Interfaces
{
    public interface IVoteRepository
    {
        Task AddAsync(Vote vote);
        Task<bool> HasUserVotedOnPollAsync(long userId, long pollId);
        Task<Vote?> GetUserVoteOnPollAsync(long userId, long pollId);
        Task<List<Vote>> GetVotesByPollIdAsync(long pollId);
        Task<int> GetTotalVotesCountAsync();
        Task<List<Vote>> GetUserVotesAsync(long userId);
    }
}
