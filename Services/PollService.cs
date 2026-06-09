using AutoMapper;
using pollbackend.Dtos;
using pollbackend.Data;
using pollbackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pollbackend.Services
{
    public class PollService : IPollService
    {
        private readonly IPollRepository _pollRepository;
        private readonly IMapper _mapper;

        public PollService(IPollRepository pollRepository, IMapper mapper)
        {
            _pollRepository = pollRepository;
            _mapper = mapper;
        }

        public async Task<PollDto> CreatePollAsync(CreatePollDto createPollDto, long creatorId)
        {
            var poll = new Poll
            {
                Title = createPollDto.Title,
                Description = createPollDto.Description,
                IsEnabled = false, // disabled by default until explicitly enabled
                CreatedBy = creatorId,
                CreatedAt = DateTime.UtcNow,
                Options = createPollDto.Options.Select(optText => new PollOption
                {
                    OptionText = optText
                }).ToList()
            };

            await _pollRepository.AddAsync(poll);

            return _mapper.Map<PollDto>(poll);
        }

        public async Task<PollDto> CreateFifaWorldCupPollAsync(long creatorId)
        {
            var fifaNations = new List<string>
            {
                "Argentina", "Brazil", "France", "Germany", "Spain", "England", "Portugal", "Netherlands",
                "Belgium", "Italy", "Croatia", "Uruguay", "Senegal", "USA", "Japan", "Morocco"
            };

            var createPollDto = new CreatePollDto
            {
                Title = "Who will win the 2026 FIFA World Cup?",
                Description = "Vote for your favorite nation to win the next FIFA World Cup tournament.",
                Options = fifaNations
            };

            return await CreatePollAsync(createPollDto, creatorId);
        }

        public async Task<bool> EnablePollAsync(long pollId)
        {
            var poll = await _pollRepository.GetByIdAsync(pollId);
            if (poll == null)
            {
                return false;
            }

            poll.IsEnabled = true;
            await _pollRepository.UpdateAsync(poll);
            return true;
        }

        public async Task<bool> DisablePollAsync(long pollId)
        {
            var poll = await _pollRepository.GetByIdAsync(pollId);
            if (poll == null)
            {
                return false;
            }

            poll.IsEnabled = false;
            await _pollRepository.UpdateAsync(poll);
            return true;
        }

        public async Task<List<PollDto>> GetActivePollsAsync()
        {
            var activePolls = await _pollRepository.GetActivePollsAsync();
            return _mapper.Map<List<PollDto>>(activePolls);
        }

        public async Task<PollDto?> GetPollByIdAsync(long id)
        {
            var poll = await _pollRepository.GetByIdAsync(id);
            return poll == null ? null : _mapper.Map<PollDto>(poll);
        }

        public async Task<PaginatedResultDto<PollDto>> GetAllPollsPaginatedAsync(int page, int pageSize)
        {
            var total = await _pollRepository.GetTotalPollsCountAsync();
            var data = await _pollRepository.GetAllPollsPaginatedAsync(page, pageSize);

            return new PaginatedResultDto<PollDto>
            {
                TotalRecords = total,
                Page = page,
                PageSize = pageSize,
                Data = _mapper.Map<List<PollDto>>(data)
            };
        }

        public async Task<bool> DeletePollAsync(long pollId)
        {
            var exists = await _pollRepository.ExistsAsync(pollId);
            if (!exists)
            {
                return false;
            }

            await _pollRepository.DeleteAsync(pollId);
            return true;
        }
    }
}
