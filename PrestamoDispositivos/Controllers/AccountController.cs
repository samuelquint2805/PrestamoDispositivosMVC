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
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register() => View();

        [AllowAnonymous]
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

            // Determinar el rol según el email
            string userRole = DetermineUserRole(model.Email);

            // Crear el ApplicationUser
            ApplicationUser newUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = null,
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = userRole,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                TwoFactorEnabled = false
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Guardar el ID en TempData para usarlo en el siguiente paso
            TempData["NewUserId"] = newUser.Id.ToString();
            TempData["NewUserRole"] = userRole;

            _notyf.Success($"Usuario creado. Ahora completa tu perfil de {(userRole == "DeviceManAdmin" ? "Administrador" : "Estudiante")}.");

            // Redirigir según el rol
            if (userRole == "DeviceManAdmin")
                return RedirectToAction("CompleteAdminProfile", "Account");
            else
                return RedirectToAction("CompleteStudentProfile", "Account");
        }

        // ========================
        //  COMPLETAR PERFIL ADMIN
        // ========================
        [HttpGet]
        public IActionResult CompleteAdminProfile()
        {
            if (TempData["NewUserId"] == null)
            {
                _notyf.Error("Sesión expirada. Por favor, regístrate nuevamente.");
                return RedirectToAction("Register");
            }

            TempData.Keep("NewUserId");
            TempData.Keep("NewUserRole");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteAdminProfile(CompleteAdminProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.Keep("NewUserId");
                TempData.Keep("NewUserRole");
                return View(model);
            }

            var userIdStr = TempData["NewUserId"]?.ToString();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
            {
                _notyf.Error("Sesión expirada. Por favor, regístrate nuevamente.");
                return RedirectToAction("Register");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _notyf.Error("Usuario no encontrado.");
                return RedirectToAction("Register");
            }

            // Verificar si ya existe un admin con ese usuario
            var existingAdmin = await _context.AdminDisp
                .FirstOrDefaultAsync(a => a.Usuario == model.Usuario);

            if (existingAdmin != null)
            {
                _notyf.Error("El nombre de usuario de administrador ya existe.");
                TempData.Keep("NewUserId");
                TempData.Keep("NewUserRole");
                return View(model);
            }

            // Crear el deviceManager
            var deviceManager = new deviceManager
            {
                IdAdmin = Guid.NewGuid(),
                Nombre = model.Nombre,
                Usuario = model.Usuario,
                Contraseña = BCrypt.Net.BCrypt.HashPassword(model.Contraseña),
                ApplicationUserId = userId
            };

            _context.AdminDisp.Add(deviceManager);
            await _context.SaveChangesAsync();

            // Actualizar el ApplicationUser para que apunte a sí mismo
            user.ApplicationUserId = userId;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Iniciar sesión automáticamente
            await SignInUser(user, false);

            _notyf.Success("¡Perfil de administrador completado exitosamente!");
            return RedirectToAction("Index", "DeviceManager");
        }

        // ========================
        // COMPLETAR PERFIL ESTUDIANTE
        // ========================
        [AllowAnonymous]
        [HttpGet]
        public IActionResult CompleteStudentProfile()
        {
            if (TempData["NewUserId"] == null)
            {
                _notyf.Error("Sesión expirada. Por favor, regístrate nuevamente.");
                return RedirectToAction("Register");
            }

            TempData.Keep("NewUserId");
            TempData.Keep("NewUserRole");

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteStudentProfile(CompleteStudentProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.Keep("NewUserId");
                TempData.Keep("NewUserRole");
                return View(model);
            }

            var userIdStr = TempData["NewUserId"]?.ToString();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
            {
                _notyf.Error("Sesión expirada. Por favor, regístrate nuevamente.");
                return RedirectToAction("Register");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _notyf.Error("Usuario no encontrado.");
                return RedirectToAction("Register");
            }

            // Verificar si ya existe un estudiante con ese carnet
            var existingStudent = await _context.Estudiante
                .FirstOrDefaultAsync(s => s.Carnet == model.Carnet);

            if (existingStudent != null)
            {
                _notyf.Error("El carnet ya está registrado.");
                TempData.Keep("NewUserId");
                TempData.Keep("NewUserRole");
                return View(model);
            }

            // Estado por defecto (Activo)
            Guid defaultStatusId = Guid.Parse("1EAA1209-075C-4E29-91C9-33824518AD93");

            // Crear el Student
            var student = new Student
            {
                IdEst = Guid.NewGuid(),
                Nombre = model.Nombre,
                Telefono = model.Telefono,
                Edad = model.Edad,
                semestreCursado = model.SemestreCursado,
                Carnet = model.Carnet,
                ApplicationUserId = userId,
                EstadoEstId = defaultStatusId
            };

            _context.Estudiante.Add(student);
            await _context.SaveChangesAsync();

            // Actualizar el ApplicationUser para que apunte a sí mismo
            user.ApplicationUserId = userId;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Iniciar sesión automáticamente
            await SignInUser(user, false);

            _notyf.Success("¡Perfil de estudiante completado exitosamente!");
            return RedirectToAction("Index", "Home");
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

            user.AccessFailedCount = 0;
            await _context.SaveChangesAsync();

            await SignInUser(user, model.RememberMe);

            _notyf.Success($"¡Bienvenido, {user.UserName}!");

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
