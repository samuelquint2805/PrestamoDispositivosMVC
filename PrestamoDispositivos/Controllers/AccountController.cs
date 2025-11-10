using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.Models;
using PrestamoDispositivos.Models.ViewModels;
using PrestamoDispositivos.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrestamoDispositivos.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatacontextPres _context;
        private readonly TwoFactorService _twoFactor;
        private readonly INotyfService _notyf;

        // configuración
        private const int MaxFailedAccessAttempts = 5;
        private static readonly TimeSpan DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

        public AccountController(DatacontextPres context, TwoFactorService twoFactor, INotyfService notyf)
        {
            _context = context;
            _twoFactor = twoFactor;
            _notyf = notyf;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email || u.UserName == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Credenciales inválidas.");
                return View(model);
            }

            // Verificar bloqueo
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            {
                return View("Lockout");
            }

            // Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                user.AccessFailedCount++;
                if (user.AccessFailedCount >= MaxFailedAccessAttempts && user.LockoutEnabled)
                {
                    user.LockoutEnd = DateTime.UtcNow.Add(DefaultLockoutTimeSpan);
                    user.AccessFailedCount = 0;
                    await _context.SaveChangesAsync();
                    _notyf.Error("Cuenta bloqueada por intentos fallidos.");
                    return View("Lockout");
                }

                await _context.SaveChangesAsync();
                ModelState.AddModelError("", "Credenciales inválidas.");
                return View(model);
            }

            // Si llegó aquí, la contraseña es correcta
            user.AccessFailedCount = 0;
            await _context.SaveChangesAsync();

            // Verificación 2FA
            if (user.TwoFactorEnabled)
            {
                await _twoFactor.GenerateAndSendCodeAsync(user);
                TempData["TwoFactorUserId"] = user.Id.ToString();
                TempData["ReturnUrl"] = model.ReturnUrl ?? string.Empty;
                TempData["RememberMe"] = model.RememberMe.ToString();
                return RedirectToAction(nameof(LoginWith2fa));
            }

            //  Iniciar sesión normal
            await SignInUser(user, model.RememberMe);

            // Redirección por rol
            if (user.Role == "DeviceManager")
                return RedirectToAction("Index", "DeviceManagers");

            if (user.Role == "Student")
                return RedirectToAction("Index", "Home");

            // Redirección por ReturnUrl si existe
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult LoginWith2fa()
        {
            var model = new LoginWith2faViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!TempData.TryGetValue("TwoFactorUserId", out var objUserId))
            {
                _notyf.Error("Sesión 2FA inválida. Vuelve a iniciar sesión.");
                return RedirectToAction(nameof(Login));
            }

            if (!Guid.TryParse(objUserId as string, out var userId))
                return RedirectToAction(nameof(Login));

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return RedirectToAction(nameof(Login));

            var code = (model.TwoFactorCode ?? string.Empty)
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty);

            if (!_twoFactor.ValidateCode(user, code))
            {
                ModelState.AddModelError("", "Código de autenticación inválido o expirado.");
                return View(model);
            }

            // Código válido: limpiar y firmar sesión
            user.TwoFactorCode = null;
            user.TwoFactorCodeExpiry = null;
            await _context.SaveChangesAsync();

            bool rememberMe = false;
            if (TempData.TryGetValue("RememberMe", out var rm))
                bool.TryParse(rm as string, out rememberMe);

            await SignInUser(user, rememberMe);

            var returnUrl = string.Empty;
            if (TempData.TryGetValue("ReturnUrl", out var ru))
                returnUrl = ru as string ?? string.Empty;

            // 🔐 Redirección por rol después de 2FA
            if (user.Role == "DeviceManager")
                return RedirectToAction("Index", "DeviceManagers");

            if (user.Role == "Student")
                return RedirectToAction("Index", "Home");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MiCookieAuth");
            _notyf.Information("Sesión cerrada.");
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(ApplicationUser user, bool isPersistent)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role ?? "Student"),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    };

            var claimsIdentity = new ClaimsIdentity(claims, "MiCookieAuth");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignInAsync("MiCookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);
        }
}
    }