using System.Collections.Generic;
using System.Threading.Tasks;
using pollbackend.Models;

namespace pollbackend.Data
{
    public interface IPollRepository
    {
        Task<Poll?> GetByIdAsync(long id);
        Task AddAsync(Poll poll);
        Task UpdateAsync(Poll poll);
        Task<List<Poll>> GetActivePollsAsync();
        Task<List<Poll>> GetAllPollsPaginatedAsync(int page, int pageSize);
        Task<int> GetTotalPollsCountAsync();
        Task<bool> ExistsAsync(long id);
        Task DeleteAsync(long id);
    }
}
