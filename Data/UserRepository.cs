using Microsoft.EntityFrameworkCore;
using pollbackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pollbackend.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly PollDbContext _context;

        public UserRepository(PollDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(long id)
        {
            return await _context.Users
                .Select(x => new User
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    PasswordHash = x.PasswordHash,
                    Role = x.Role,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Select(x => new User
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    PasswordHash = x.PasswordHash,
                    Role = x.Role,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Select(x => new User
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    PasswordHash = x.PasswordHash,
                    Role = x.Role,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username == username);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<List<User>> GetPaginatedUsersAsync(int page, int pageSize)
        {
            return await _context.Users
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new User
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    Role = x.Role,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();
        }
    }
}
