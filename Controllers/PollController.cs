using FluentValidation;
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
    [Route("api/polls")]
    public class PollController : ControllerBase
    {
        private readonly IPollService _pollService;
        private readonly IValidator<CreatePollDto> _createPollValidator;

        public PollController(IPollService pollService, IValidator<CreatePollDto> createPollValidator)
        {
            _pollService = pollService;
            _createPollValidator = createPollValidator;
        }

        private long GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            return claim != null ? long.Parse(claim.Value) : 0;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreatePollDto request)
        {
            var validationResult = await _createPollValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors });
            }

            var creatorId = GetUserId();
            var poll = await _pollService.CreatePollAsync(request, creatorId);
            return CreatedAtAction(nameof(GetById), new { id = poll.Id }, poll);
        }

        [HttpPost("fifa")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFifaWorldCupPoll()
        {
            var creatorId = GetUserId();
            var poll = await _pollService.CreateFifaWorldCupPollAsync(creatorId);
            return CreatedAtAction(nameof(GetById), new { id = poll.Id }, poll);
        }

        [HttpPut("{id}/enable")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Enable(long id)
        {
            var result = await _pollService.EnablePollAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Poll not found." });
            }
            return Ok(new { message = "Poll enabled successfully." });
        }

        [HttpPut("{id}/disable")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Disable(long id)
        {
            var result = await _pollService.DisablePollAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Poll not found." });
            }
            return Ok(new { message = "Poll disabled successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetPolls([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var isAdmin = User.IsInRole("Admin");
            if (isAdmin)
            {
                var paginated = await _pollService.GetAllPollsPaginatedAsync(page, pageSize);
                return Ok(paginated);
            }
            else
            {
                var active = await _pollService.GetActivePollsAsync();
                return Ok(active);
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActivePolls()
        {
            var active = await _pollService.GetActivePollsAsync();
            return Ok(active);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var poll = await _pollService.GetPollByIdAsync(id);
            if (poll == null)
            {
                return NotFound(new { message = "Poll not found." });
            }
            return Ok(poll);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _pollService.DeletePollAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Poll not found." });
            }
            return Ok(new { message = "Poll deleted successfully." });
        }

        [HttpPut("{id}/enable-results")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnableResults(long id)
        {
            var result = await _pollService.EnableViewResultsAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Poll not found." });
            }
            return Ok(new { message = "Poll results visibility enabled successfully." });
        }

        [HttpPut("{id}/disable-results")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DisableResults(long id)
        {
            var result = await _pollService.DisableViewResultsAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Poll not found." });
            }
            return Ok(new { message = "Poll results visibility disabled successfully." });
        }
    }
}
