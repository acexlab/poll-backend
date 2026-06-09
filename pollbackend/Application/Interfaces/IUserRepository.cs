using System.Collections.Generic;
using System.Threading.Tasks;
using pollbackend.Domain.Entities;

namespace pollbackend.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(long id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByEmailAsync(string email);
        Task AddAsync(User user);
        Task<int> GetTotalUsersCountAsync();
        Task<List<User>> GetPaginatedUsersAsync(int page, int pageSize);
    }
}
