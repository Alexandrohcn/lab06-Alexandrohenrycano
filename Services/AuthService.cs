using Lab06_AlexandroCano.DTOs;
using Lab06_AlexandroCano.Models;
using Lab06_AlexandroCano.Repositories;
 
namespace Lab06_AlexandroCano.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
 
        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }
 
        public async Task<(bool success, string message, TokenResponseDto? data)> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username);
 
            if (user == null)
                return (false, "Credenciales inválidas.", null);
 
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return (false, "Credenciales inválidas.", null);
 
            var (token, expiration) = _tokenService.GenerateToken(user);
 
            var response = new TokenResponseDto
            {
                Token = token,
                Expiration = expiration,
                Username = user.Username,
                Role = user.Role
            };
 
            return (true, "Login exitoso.", response);
        }
 
        public async Task<(bool success, string message, int? userId)> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepository.UsernameExistsAsync(dto.Username))
                return (false, "El nombre de usuario ya está en uso.", null);
 
            if (await _userRepository.EmailExistsAsync(dto.Email))
                return (false, "El email ya está registrado.", null);
 
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
 
            var created = await _userRepository.AddAsync(user);
 
            return (true, "Usuario registrado correctamente.", created.Id);
        }
    }
}
