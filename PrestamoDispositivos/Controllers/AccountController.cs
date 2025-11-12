using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // Configuración
        private const int MaxFailedAccessAttempts = 5;
        private static readonly TimeSpan DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);

        public AccountController(DatacontextPres context, TwoFactorService twoFactor, INotyfService notyf)
        {
            _context = context;
            _twoFactor = twoFactor;
            _notyf = notyf;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Verificar si el usuario o email ya existen
            var existingUser = await _context.Users
                .AnyAsync(u => u.UserName == model.UserName || u.Email == model.Email);

            if (existingUser)
            {
                ModelState.AddModelError("", "El nombre de usuario o correo electrónico ya están en uso.");
                return View(model);
            }

            // Determinar rol basado en el email
            string userRole = DetermineUserRole(model.Email);

            // Crear nuevo usuario
            ApplicationUser newUser = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = userRole,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                TwoFactorEnabled = false,
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // ✅ INICIAR SESIÓN AUTOMÁTICAMENTE después del registro
            await SignInUser(newUser, isPersistent: false);

            _notyf.Success($"¡Bienvenido, {newUser.UserName}! Tu cuenta como {userRole} ha sido creada.");

            // Redireccionar según el rol
            if (userRole == "DeviceManAdmin")
                return RedirectToAction("Index", "DeviceManagers");

            return RedirectToAction("Index", "Home");
        }

        // Método para determinar el rol
        private string DetermineUserRole(string email)
        {
            // Por dominio de email
            if (email.EndsWith("@admin.gmail.com", StringComparison.OrdinalIgnoreCase))
                return "DeviceManAdmin";

            // Lista de emails administradores
            var adminEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "admin@ejemplo.com",
                "supervisor@ejemplo.com"
            };

            if (adminEmails.Contains(email))
                return "DeviceManAdmin";

            // Por defecto, todos son estudiantes
            return "Estudiante";
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Datos inválidos." });
                }
                return View(model);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email || u.UserName == model.Email);

            if (user == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Credenciales inválidas." });
                }
                ModelState.AddModelError("", "Credenciales inválidas.");
                return View(model);
            }

            // Verificar bloqueo
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Cuenta bloqueada. Intenta más tarde." });
                }
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

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Cuenta bloqueada por intentos fallidos." });
                    }
                    return View("Lockout");
                }

                await _context.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Credenciales inválidas." });
                }
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

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, requires2FA = true, redirectUrl = Url.Action(nameof(LoginWith2fa)) });
                }
                return RedirectToAction(nameof(LoginWith2fa));
            }

            // Iniciar sesión normal
            await SignInUser(user, model.RememberMe);

            _notyf.Success($"¡Bienvenido, {user.UserName}!");

            // Determinar URL de redirección
            string redirectUrl = "/";
            if (user.Role == "DeviceManAdmin")
                redirectUrl = Url.Action("Index", "DeviceManagers") ?? "/";
            else if (user.Role == "Estudiante")
                redirectUrl = Url.Action("Index", "Home") ?? "/";
            else if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                redirectUrl = model.ReturnUrl;

            // Responder según el tipo de solicitud
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, redirectUrl = redirectUrl });
            }

            return Redirect(redirectUrl);
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

            // Redirección por rol después de 2FA
            if (user.Role == "DeviceManAdmin")
                return RedirectToAction("Index", "DeviceManagers");

            if (user.Role == "Estudiante")
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

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _notyf.Information("Sesión cerrada.");
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(ApplicationUser user, bool isPersistent)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "Estudiante"),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}