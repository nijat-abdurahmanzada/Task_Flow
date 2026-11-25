using TaskFlow.API.DTOs;

namespace TaskFlow.API.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        public Task<AuthResponseDto >LoginAsync(LoginDto dto);
    }
}
