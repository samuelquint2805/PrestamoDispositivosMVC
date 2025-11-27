using PrestamoDispositivos.Models;
using System.Security.Claims;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface IJwtTokenService
    {
        string GenerateToken(ApplicationUser user);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
