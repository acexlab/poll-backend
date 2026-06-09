using System.Collections.Generic;
using System.Threading.Tasks;
using pollbackend.Dtos;

namespace pollbackend.Services
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
        Task<bool> DeletePollAsync(long pollId);
        Task<bool> EnableViewResultsAsync(long pollId);
        Task<bool> DisableViewResultsAsync(long pollId);
    }
}
