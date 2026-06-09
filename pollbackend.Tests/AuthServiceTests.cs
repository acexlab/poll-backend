using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using pollbackend.Dtos;
using pollbackend.Data;
using pollbackend.Services;
using pollbackend.Models;
using System.Threading.Tasks;
using Xunit;

namespace pollbackend.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _configMock = new Mock<IConfiguration>();
            
            // Set up config mock fallback
            _configMock.Setup(c => c["Jwt:SecretKey"]).Returns("super_secret_key_at_least_32_characters_long_1234567890");
            _configMock.Setup(c => c["Jwt:Issuer"]).Returns("PollingAppAPI");
            _configMock.Setup(c => c["Jwt:Audience"]).Returns("PollingAppAngular");

            _authService = new AuthService(_userRepoMock.Object, _mapperMock.Object, _configMock.Object);
        }

        [Fact]
        public async Task Login_Should_Return_Jwt_When_Credentials_Valid()
        {
            // Arrange
            var username = "testuser";
            var password = "testpassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            
            var user = new User
            {
                Id = 1,
                Username = username,
                Email = "test@example.com",
                PasswordHash = hashedPassword,
                Role = Role.User,
                IsActive = true
            };

            _userRepoMock.Setup(r => r.GetByUsernameAsync(username)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { Id = 1, Username = username, Email = user.Email, Role = "User" });

            var requestDto = new LoginRequestDto { Username = username, Password = password };

            // Act
            var result = await _authService.LoginAsync(requestDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.Equal("User", result.Role);
        }

        [Fact]
        public async Task Login_Should_Return_Null_When_Credentials_Invalid()
        {
            // Arrange
            var username = "testuser";
            var password = "testpassword";
            
            _userRepoMock.Setup(r => r.GetByUsernameAsync(username)).ReturnsAsync((User?)null);

            var requestDto = new LoginRequestDto { Username = username, Password = password };

            // Act
            var result = await _authService.LoginAsync(requestDto);

            // Assert
            Assert.Null(result);
        }
    }
}
