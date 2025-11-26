using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.Models;
using PrestamoDispositivos.Models.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PrestamoDispositivos.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatacontextPres _context;
        private readonly INotyfService _notyf;

        private const int MaxFailedAccessAttempts = 5;
        private static readonly TimeSpan DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);

        public AccountController(DatacontextPres context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // ========================
        //     REGISTRO
        // ========================
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _context.Users
                .AnyAsync(u => u.UserName == model.UserName || u.Email == model.Email);

            if (existingUser)
            {
                ModelState.AddModelError("", "El nombre de usuario o correo electrónico ya están en uso.");
                return View(model);
            }

            string userRole = DetermineUserRole(model.Email);

            ApplicationUser newUser = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = userRole,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                TwoFactorEnabled = false // SIEMPRE DESACTIVADO
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            await SignInUser(newUser, false);
            _notyf.Success($"¡Bienvenido, {newUser.UserName}! Tu cuenta como {userRole} ha sido creada.");

            return RedirectToAction("Index", "Home");
        }

        private string DetermineUserRole(string email)
        {
            if (email.EndsWith("@admin.gmail.com", StringComparison.OrdinalIgnoreCase))
                return "DeviceManAdmin";

            var adminEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "admin@ejemplo.com",
                "supervisor@ejemplo.com"
            };

            if (adminEmails.Contains(email))
                return "DeviceManAdmin";

            return "Estudiante";
        }

        // ========================
        //        LOGIN
        // ========================
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

            // Bloqueo
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
                return View("Lockout");

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

            // Login correcto
            user.AccessFailedCount = 0;
            await _context.SaveChangesAsync();

            await SignInUser(user, model.RememberMe);

            _notyf.Success($"¡Bienvenido, {user.UserName}!");


            // Redirección por rol
            if (user.Role == "DeviceManAdmin")
                return RedirectToAction("Index", "DeviceManager");

            return RedirectToAction("Index", "Home");
        }

        // ========================
        //      LOGOUT
        // ========================
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _notyf.Information("Sesión cerrada.");
            return RedirectToAction("Login");
        }

        // ========================
        //   PERFIL Y AJUSTES
        // ========================
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userGuid = GetUserIdFromClaims();
            if (userGuid == null) return RedirectToAction("Login");

            var user = await _context.Users.FindAsync(userGuid.Value);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role ?? "Estudiante",
                TwoFactorEnabled = false,
                LockoutEnabled = user.LockoutEnabled,
                AccessFailedCount = user.AccessFailedCount
            };

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var userGuid = GetUserIdFromClaims();
            if (userGuid == null) return RedirectToAction("Login");

            var user = await _context.Users.FindAsync(userGuid.Value);
            if (user == null) return NotFound();

            var model = new SettingsViewModel
            {
                CurrentEmail = user.Email,
                TwoFactorEnabled = false
            };

            return View(model);
        }

        // ========================
        //    CAMBIO DE CONTRASEÑA
        // ========================
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Datos inválidos.");
                return RedirectToAction(nameof(Settings));
            }

            var userGuid = GetUserIdFromClaims();
            if (userGuid == null) return RedirectToAction("Login");

            var user = await _context.Users.FindAsync(userGuid.Value);
            if (user == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
            {
                _notyf.Error("La contraseña actual es incorrecta.");
                return RedirectToAction(nameof(Settings));
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            _notyf.Success("Contraseña actualizada correctamente.");
            return RedirectToAction(nameof(Settings));
        }

        // ========================
        //  MÉTODOS PRIVADOS
        // ========================
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

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = isPersistent,
                    ExpiresUtc = DateTime.UtcNow.AddHours(8)
                });
        }

        private Guid? GetUserIdFromClaims()
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(idString, out var guid) ? guid : null;
        }

        // ========================
        //  VIEWMODELS INTERNOS
        // ========================
        public class ProfileViewModel
        {
            public string UserName { get; set; } = "";
            public string Email { get; set; } = "";
            public string Role { get; set; } = "";
            public bool TwoFactorEnabled { get; set; }
            public bool LockoutEnabled { get; set; }
            public int AccessFailedCount { get; set; }
        }

        public class SettingsViewModel
        {
            public bool TwoFactorEnabled { get; set; }
            public string CurrentEmail { get; set; } = "";
        }

        public class ChangePasswordViewModel
        {
            [Required]
            public string CurrentPassword { get; set; } = "";

            [Required]
            [StringLength(100, MinimumLength = 6)]
            public string NewPassword { get; set; } = "";

            [Compare("NewPassword")]
            public string ConfirmPassword { get; set; } = "";
        }
    }
}
