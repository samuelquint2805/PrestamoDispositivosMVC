using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Models;
using System.Security.Claims;

namespace PrestamoDispositivos.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ApplicationUser user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                Role = "Estudiante" // o DeviceManAdmin si es admin
            };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Autenticación inmediata tras registro
            await SignInUser(user);

            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(AppUser user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var claimsIdentity = new ClaimsIdentity(claims, "MiCookieAuth");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync("MiCookieAuth",
                new ClaimsPrincipal(claimsIdentity), authProperties);
        }

    }
}
