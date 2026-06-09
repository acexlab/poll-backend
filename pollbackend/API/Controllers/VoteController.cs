using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pollbackend.Application.DTOs;
using pollbackend.Application.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace pollbackend.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/votes")]
    public class VoteController : ControllerBase
    {
        private readonly IVoteService _voteService;

        public VoteController(IVoteService voteService)
        {
            _voteService = voteService;
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
    }
}
