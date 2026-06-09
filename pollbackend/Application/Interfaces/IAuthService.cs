using System.Threading.Tasks;
using pollbackend.Application.DTOs;

namespace pollbackend.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<UserDto> RegisterAsync(RegisterRequestDto request);
    }
}
