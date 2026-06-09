using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pollbackend.Dtos;
using pollbackend.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace pollbackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/votes")]
    public class VoteController : ControllerBase
    {
        private readonly IVoteService _voteService;
        private readonly IPollService _pollService;

        public VoteController(IVoteService voteService, IPollService pollService)
        {
            _voteService = voteService;
            _pollService = pollService;
        }

        private long GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            return claim != null ? long.Parse(claim.Value) : 0;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CastVote([FromBody] CastVoteDto request)
        {
            var userId = GetUserId();
            var success = await _voteService.CastVoteAsync(request, userId);
            if (!success)
            {
                return BadRequest(new { message = "Failed to record vote." });
            }

            return Ok(new { message = "Vote cast successfully." });
        }

        [HttpGet("results/{pollId}")]
        public async Task<IActionResult> GetResults(long pollId)
        {
            var results = await _voteService.GetResultsAsync(pollId);
            if (results == null)
            {
                return NotFound(new { message = "Poll not found." });
            }

            var isUser = User.IsInRole("User");
            if (isUser)
            {
                var poll = await _pollService.GetPollByIdAsync(pollId);
                if (poll != null && !poll.ShowResults)
                {
                    return StatusCode(403, new { message = "Results are hidden for this poll." });
                }
            }

            return Ok(results);
        }

        [HttpGet("my-votes")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyVotes()
        {
            var userId = GetUserId();
            var votes = await _voteService.GetUserVotesAsync(userId);
            return Ok(votes);
        }

        [HttpGet("my-vote/{pollId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyVoteOnPoll(long pollId)
        {
            var userId = GetUserId();
            var vote = await _voteService.GetUserVoteOnPollAsync(userId, pollId);
            if (vote == null)
            {
                return NotFound(new { message = "No vote cast by this user on this poll." });
            }
            return Ok(vote);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _voteService.DeleteVoteAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Vote not found." });
            }
            return Ok(new { message = "Vote deleted successfully." });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AlterVote(long id, [FromBody] AlterVoteRequestDto request)
        {
            var success = await _voteService.AlterVoteAsync(id, request.NewOptionId);
            if (!success)
            {
                return BadRequest(new { message = "Failed to alter vote. Make sure the vote and the option belong to the same poll." });
            }
            return Ok(new { message = "Vote updated successfully." });
        }
    }

    public class AlterVoteRequestDto
    {
        public long NewOptionId { get; set; }
    }
}
