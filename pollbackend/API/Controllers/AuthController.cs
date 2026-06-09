using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using pollbackend.Application.DTOs;
using pollbackend.Application.Interfaces;
using System.Threading.Tasks;

namespace pollbackend.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<LoginRequestDto> _loginValidator;
        private readonly IValidator<RegisterRequestDto> _registerValidator;

        public AuthController(
            IAuthService authService, 
            IValidator<LoginRequestDto> loginValidator, 
            IValidator<RegisterRequestDto> registerValidator)
        {
            _authService = authService;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors });
            }

            var response = await _authService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var validationResult = await _registerValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors });
            }

            try
            {
                var response = await _authService.RegisterAsync(request);
                return CreatedAtAction(nameof(Register), new { username = response.Username }, response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
