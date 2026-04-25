using Lab06_AlexandroCano.Models;
 
namespace Lab06_AlexandroCano.Services
{
    public interface ITokenService
    {
        (string token, DateTime expiration) GenerateToken(User user);
    }
}