using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Lab06_AlexandroCano.Models;
 
namespace Lab06_AlexandroCano.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
 
        public TokenService(IConfiguration config)
        {
            _config = config;
        }
 
        public (string token, DateTime expiration) GenerateToken(User user)
        {
            var jwtKey = _config["Jwt:Key"]!;
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var minutes = int.Parse(_config["Jwt:ExpireMinutes"] ?? "60");
 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
 
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            };
 
            var expiration = DateTime.UtcNow.AddMinutes(minutes);
 
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );
 
            return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
        }
    }
}