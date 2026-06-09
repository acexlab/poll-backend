using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using pollbackend.Application.DTOs;
using pollbackend.Application.Interfaces;
using pollbackend.Domain.Entities;
using pollbackend.Domain.Enums;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace pollbackend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !user.IsActive)
            {
                return null;
            }

            // Verify password hash
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return null;
            }

            // Generate JWT Token
            var token = GenerateToken(user);

            var userDto = _mapper.Map<UserDto>(user);

            return new LoginResponseDto
            {
                Token = token,
                Role = user.Role.ToString(),
                User = userDto
            };
        }

        public async Task<UserDto> RegisterAsync(RegisterRequestDto request)
        {
            // Check if username or email exists
            if (await _userRepository.ExistsByUsernameAsync(request.Username))
            {
                throw new Exception("Username is already taken.");
            }

            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new Exception("Email is already registered.");
            }

            // Parse Role
            if (!Enum.TryParse<Role>(request.Role, true, out var parsedRole))
            {
                parsedRole = Role.User;
            }

            // Hash password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = parsedRole,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return _mapper.Map<UserDto>(user);
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            // Generate a secure key (fallback for development if not present in config)
            var secret = _configuration["Jwt:SecretKey"] ?? "super_secret_key_at_least_32_characters_long_1234567890";
            var key = Encoding.UTF8.GetBytes(secret);

            var claims = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim(ClaimTypes.Name, user.Username), // for standard identity mechanisms
                new Claim(ClaimTypes.Role, user.Role.ToString())
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _configuration["Jwt:Issuer"] ?? "PollingAppAPI",
                Audience = _configuration["Jwt:Audience"] ?? "PollingAppAngular",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
