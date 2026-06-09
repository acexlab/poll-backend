using System.Threading.Tasks;
using pollbackend.Dtos;

namespace pollbackend.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<UserDto> RegisterAsync(RegisterRequestDto request);
    }
}
