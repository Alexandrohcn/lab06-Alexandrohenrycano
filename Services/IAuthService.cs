using Lab06_AlexandroCano.DTOs;
 
namespace Lab06_AlexandroCano.Services
{
    public interface IAuthService
    {
        Task<(bool success, string message, TokenResponseDto? data)> LoginAsync(LoginDto dto);
        Task<(bool success, string message, int? userId)> RegisterAsync(RegisterDto dto);
    }
}