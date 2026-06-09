using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pollbackend.Application.DTOs;
using pollbackend.Application.Interfaces;
using System.Threading.Tasks;

namespace pollbackend.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPollRepository _pollRepository;
        private readonly IVoteRepository _voteRepository;

        public AdminController(
            IUserRepository userRepository, 
            IPollRepository pollRepository, 
            IVoteRepository voteRepository)
        {
            _userRepository = userRepository;
            _pollRepository = pollRepository;
            _voteRepository = voteRepository;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var total = await _userRepository.GetTotalUsersCountAsync();
            var users = await _userRepository.GetPaginatedUsersAsync(page, pageSize);

            // Project to DTO (using the mapping/select in repository)
            var list = new System.Collections.Generic.List<UserListDto>();
            foreach (var u in users)
            {
                list.Add(new UserListDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                });
            }

            var result = new PaginatedResultDto<UserListDto>
            {
                TotalRecords = total,
                Page = page,
                PageSize = pageSize,
                Data = list
            };

            return Ok(result);
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics()
        {
            var totalUsers = await _userRepository.GetTotalUsersCountAsync();
            var totalPolls = await _pollRepository.GetTotalPollsCountAsync();
            var totalVotes = await _voteRepository.GetTotalVotesCountAsync();
            
            // Get enabled/disabled poll counts
            var activePolls = await _pollRepository.GetActivePollsAsync();
            var activeCount = activePolls.Count;
            var inactiveCount = totalPolls - activeCount;

            var stats = new
            {
                TotalUsers = totalUsers,
                TotalPolls = totalPolls,
                TotalVotes = totalVotes,
                ActivePolls = activeCount,
                InactivePolls = inactiveCount >= 0 ? inactiveCount : 0
            };

            return Ok(stats);
        }
    }
}
