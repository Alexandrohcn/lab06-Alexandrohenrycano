using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lab06_AlexandroCano.DTOs;
using Lab06_AlexandroCano.Services;

namespace Lab06_AlexandroCano.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/register - Cualquiera puede registrarse, pero SIEMPRE como User
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, userId) = await _authService.RegisterAsync(dto, forcedRole: "User");

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, userId });
        }

        // POST: api/auth/register-admin - SOLO Admins pueden crear otros Admins
        [HttpPost("register-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, userId) = await _authService.RegisterAsync(dto, forcedRole: "Admin");

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, userId });
        }

        // POST: api/auth/login - Cualquiera puede intentar login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, data) = await _authService.LoginAsync(dto);

            if (!success)
                return Unauthorized(new { message });

            return Ok(data);
        }

        // GET: api/auth/profile - Cualquier usuario autenticado
        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var username = User.Identity?.Name;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var userId = User.FindFirst("UserId")?.Value;

            return Ok(new { userId, username, role });
        }

        // GET: api/auth/users - Solo Admins (política AdminOnly)
        [HttpGet("users")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }

        // PUT: api/auth/change-password - User o Admin (política UserOrAdmin)
        [HttpPut("change-password")]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token inválido." });

            var (success, message) = await _authService.ChangePasswordAsync(userId, dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}