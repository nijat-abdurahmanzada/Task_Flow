using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlow.API.Data;
using TaskFlow.API.DTOs;
using TaskFlow.API.Models;
using TaskFlow.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TaskFlow.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _hasher = new();

        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _secretKey;
        private readonly int _expirationInMinutes;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _issuer = configuration["Jwt:Issuer"]!;
            _audience = configuration["Jwt:Audience"]!;
            _secretKey = configuration["Jwt:SecretKey"]!;
            _expirationInMinutes = int.Parse(configuration["Jwt:ExpirationInMinutes"]!);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: _issuer,
                audience: _audience,
                signingCredentials: credentials,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expirationInMinutes)
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }


        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            bool exist = await _context.Users.AnyAsync(u => u.Email == dto.Email);

            if (exist)
            {
                throw new Exception("Email is already registered.");
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
            };

            var hashedPassword = _hasher.HashPassword(user, dto.Password);
            user.PasswordHash = hashedPassword;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                Email = user.Email,
                FullName = user.FullName,
                UserId = user.Id,
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                throw new Exception("User could not find!");
            }

            var password = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (password == PasswordVerificationResult.Failed)
            {
                throw new Exception("Password is not correct!");
            }
            else if (password == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _hasher.HashPassword(user, dto.Password);

                await _context.SaveChangesAsync();
            }

            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                Email = user.Email,
                UserId = user.Id,
                FullName = user.FullName
            };
        }
    }
}
