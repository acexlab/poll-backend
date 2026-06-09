using Microsoft.EntityFrameworkCore;
using pollbackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pollbackend.Data
{
    public class PollRepository : IPollRepository
    {
        private readonly PollDbContext _context;

        public PollRepository(PollDbContext context)
        {
            _context = context;
        }

        public async Task<Poll?> GetByIdAsync(long id)
        {
            return await _context.Polls
                .Select(x => new Poll
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    IsEnabled = x.IsEnabled,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    Options = x.Options.Select(o => new PollOption
                    {
                        Id = o.Id,
                        PollId = o.PollId,
                        OptionText = o.OptionText
                    }).ToList()
                })
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(Poll poll)
        {
            await _context.Polls.AddAsync(poll);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Poll poll)
        {
            var existing = await _context.Polls.FindAsync(poll.Id);
            if (existing != null)
            {
                existing.IsEnabled = poll.IsEnabled;
                existing.Title = poll.Title;
                existing.Description = poll.Description;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Poll>> GetActivePollsAsync()
        {
            return await _context.Polls
                .AsNoTracking()
                .Where(x => x.IsEnabled)
                .Select(x => new Poll
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    IsEnabled = x.IsEnabled,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    Options = x.Options.Select(o => new PollOption
                    {
                        Id = o.Id,
                        PollId = o.PollId,
                        OptionText = o.OptionText
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<Poll>> GetAllPollsPaginatedAsync(int page, int pageSize)
        {
            return await _context.Polls
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new Poll
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    IsEnabled = x.IsEnabled,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    Options = x.Options.Select(o => new PollOption
                    {
                        Id = o.Id,
                        PollId = o.PollId,
                        OptionText = o.OptionText
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<int> GetTotalPollsCountAsync()
        {
            return await _context.Polls.CountAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Polls.AnyAsync(x => x.Id == id);
        }

        public async Task DeleteAsync(long id)
        {
            var poll = await _context.Polls.FindAsync(id);
            if (poll != null)
            {
                _context.Polls.Remove(poll);
                await _context.SaveChangesAsync();
            }
        }
    }
}
