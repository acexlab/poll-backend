using System.Collections.Generic;
using System.Threading.Tasks;
using pollbackend.Application.DTOs;

namespace pollbackend.Application.Interfaces
{
    public interface IPollService
    {
        Task<PollDto> CreatePollAsync(CreatePollDto createPollDto, long creatorId);
        Task<PollDto> CreateFifaWorldCupPollAsync(long creatorId);
        Task<bool> EnablePollAsync(long pollId);
        Task<bool> DisablePollAsync(long pollId);
        Task<List<PollDto>> GetActivePollsAsync();
        Task<PollDto?> GetPollByIdAsync(long id);
        Task<PaginatedResultDto<PollDto>> GetAllPollsPaginatedAsync(int page, int pageSize);
    }
}
