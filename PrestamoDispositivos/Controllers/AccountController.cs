using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Models;
using PrestamoDispositivos.Models.ViewModels;
using PrestamoDispositivos.Services.Abstractions;
using PrestamoDispositivos.Services.Implementations;
using System.Security.Claims;

namespace PrestamoDispositivos.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatacontextPres _context;
        private readonly INotyfService _notyf;
        private readonly IAppUser _appUser;
        private readonly IRolservice _rolService;

        private const int MaxFailedAccessAttempts = 5;
        private static readonly TimeSpan DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

        public AccountController(
            DatacontextPres context,
            INotyfService notyf,
            IAppUser appUser,
            IRolservice rolService)
        {
            _context = context;
            _notyf = notyf;
            _appUser = appUser;
            _rolService = rolService;
        }

        // ════════════════════════════════════════════
        //  LISTADO DE USUARIOS (solo SuperAdmin)
        // ════════════════════════════════════════════

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            Response<List<ApplicationUserDTO>> response = await _appUser.GetAllUserUsAsync();

            if (!response.IsSuccess)
                return View(new List<ApplicationUserDTO>());

            return View(response.Result ?? new List<ApplicationUserDTO>());
        }

        // ════════════════════════════════════════════
        //  EDITAR USUARIO (SuperAdmin)
        // ════════════════════════════════════════════

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            Response<ApplicationUserDTO> response = await _appUser.GetuserByIdAsync(id);
            if (!response.IsSuccess)
                return RedirectToAction(nameof(Index));

            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromForm] ApplicationUserDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var response = await _appUser.UpdateUserUsAsync(id, dto);

            if (!response.IsSuccess)
            {
                _notyf.Error(response.Message ?? "Error al actualizar usuario.");
                return View(dto);
            }

            _notyf.Success("Usuario actualizado correctamente.");
            return RedirectToAction(nameof(Index));
        }

        // ════════════════════════════════════════════
        //  ELIMINAR USUARIO (SuperAdmin)
        // ════════════════════════════════════════════

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            Response<bool> response = await _appUser.DeleteSUserUsAsync(id);

            if (!response.IsSuccess)
                _notyf.Error(response.Message ?? "Error al eliminar usuario.");
            else
                _notyf.Success("Usuario eliminado.");

            return RedirectToAction(nameof(Index));
        }

        // ════════════════════════════════════════════
        //  ASIGNAR ROL A USUARIO (SuperAdmin)
        // ════════════════════════════════════════════

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AssignRol(Guid id)
        {
            var userResponse = await _appUser.GetuserByIdAsync(id);
            if (!userResponse.IsSuccess)
                return RedirectToAction(nameof(Index));

            var rolesResponse = await _rolService.GetAllRolesAsync();
            ViewBag.Roles = rolesResponse.IsSuccess ? rolesResponse.Result : new List<setRolDTO>();
            ViewBag.Usuario = userResponse.Result;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AssignRol(Guid id, Guid idRol)
        {
            var response = await _rolService.AssignRolToUserAsync(id, idRol);

            if (!response.IsSuccess)
                _notyf.Error(response.Message ?? "Error al asignar rol.");
            else
                _notyf.Success("Rol asignado correctamente.");

            return RedirectToAction(nameof(Index));
        }

        // ════════════════════════════════════════════
        //  REGISTRO
        // ════════════════════════════════════════════

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

            // Verificar duplicado de usuario o correo
            var existingUser = await _context.Users
                .AnyAsync(u => u.usuario == model.UserName || u.CorreoElectrónico == model.Email);

            if (existingUser)
            {
                ModelState.AddModelError("", "El nombre de usuario o correo electrónico ya están en uso.");
                return View(model);
            }

            // Determinar el rol por email
            string nombreRol = _rolService.DetermineRolByEmail(model.Email);

            // Crear el ApplicationUser
            var newUser = new ApplicationUser
            {
                idUsuario = Guid.NewGuid(),
                usuario = model.UserName,
                CorreoElectrónico = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Estado = "Activo",
                fechaRegistro = DateTime.UtcNow,
                codigo2FA = null
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Asignar rol determinado
            var rolResponse = await _rolService.AssignRolToUserByNameAsync(newUser.idUsuario, nombreRol);
            if (!rolResponse.IsSuccess)
            {
                // Si falla la asignación de rol, igualmente continuamos pero lo notificamos
                _notyf.Warning($"Usuario creado pero no se pudo asignar el rol '{nombreRol}' automáticamente.");
            }

            // Guardar en TempData para el siguiente paso
            TempData["NewUserId"] = newUser.idUsuario.ToString();
            TempData["NewUserRol"] = nombreRol;

            _notyf.Success($"Usuario creado. Completa tu perfil de {TraducirRol(nombreRol)}.");

            // Redirigir al formulario de perfil según rol
            return nombreRol switch
            {
                RolService.ROL_SUPERADMIN => RedirectToAction(nameof(CompleteAdminProfile)),
                RolService.ROL_LENDER => RedirectToAction(nameof(CompleteLenderProfile)),
                _ => RedirectToAction(nameof(CompleteStudentProfile))
            };
        }

        // ════════════════════════════════════════════
        //  COMPLETAR PERFIL: SUPERADMIN
        // ════════════════════════════════════════════

        [AllowAnonymous]
        [HttpGet]
        public IActionResult CompleteAdminProfile()
        {
            if (!ValidarTempDataRegistro()) return RedirectToAction(nameof(Register));
            TempData.Keep("NewUserId");
            TempData.Keep("NewUserRol");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteAdminProfile(CompleteAdminProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.Keep("NewUserId");
                TempData.Keep("NewUserRol");
                return View(model);
            }

            if (!ObtenerUserId(out Guid userId))
            {
                _notyf.Error("Sesión expirada. Por favor, regístrate nuevamente.");
                return RedirectToAction(nameof(Register));
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _notyf.Error("Usuario no encontrado.");
                return RedirectToAction(nameof(Register));
            }

            // Crear perfil Administrator
            var admin = new Administrator
            {
                IdAdmin = Guid.NewGuid(),
                Nombre = model.Nombre,
                numeroCelular = model.NumeroCelular,
                ApplicationUserId = userId
            };

            _context.Administradores.Add(admin);
            await _context.SaveChangesAsync();

            // Login automático
            await SignInUserAsync(user);

            _notyf.Success("¡Perfil de SuperAdmin completado exitosamente!");
            return RedirectToAction("Index", "Home");
        }

        // ════════════════════════════════════════════
        //  COMPLETAR PERFIL: LENDER
        // ════════════════════════════════════════════

        [AllowAnonymous]
        [HttpGet]
        public IActionResult CompleteLenderProfile()
        {
            if (!ValidarTempDataRegistro()) return RedirectToAction(nameof(Register));
            TempData.Keep("NewUserId");
            TempData.Keep("NewUserRol");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteLenderProfile(completeLenderProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.Keep("NewUserId");
                TempData.Keep("NewUserRol");
                return View(model);
            }

            if (!ObtenerUserId(out Guid userId))
            {
                _notyf.Error("Sesión expirada. Por favor, regístrate nuevamente.");
                return RedirectToAction(nameof(Register));
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _notyf.Error("Usuario no encontrado.");
                return RedirectToAction(nameof(Register));
            }

            // Crear perfil Lender (Prestamista)
            var lenderEntity = new lender
            {
                idPres = Guid.NewGuid(),
                Nombre = model.Nombre,
                numeroCelular = model.NumeroCelular,
                ApplicationUserId = userId
            };

            _context.Prestamista.Add(lenderEntity);
            await _context.SaveChangesAsync();

            // Login automático
            await SignInUserAsync(user);

            _notyf.Success("¡Perfil de Prestamista completado exitosamente!");
            return RedirectToAction("Index", "Loan"); // Redirigir al módulo de préstamos
        }

        // ════════════════════════════════════════════
        //  COMPLETAR PERFIL: STUDENT
        // ════════════════════════════════════════════

        [AllowAnonymous]
        [HttpGet]
        public IActionResult CompleteStudentProfile()
        {
            if (!ValidarTempDataRegistro()) return RedirectToAction(nameof(Register));
            TempData.Keep("NewUserId");
            TempData.Keep("NewUserRol");
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
                TempData.Keep("NewUserRol");
                return View(model);
            }

            if (!ObtenerUserId(out Guid userId))
            {
                _notyf.Error("Sesión expirada. Por favor, regístrate nuevamente.");
                return RedirectToAction(nameof(Register));
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _notyf.Error("Usuario no encontrado.");
                return RedirectToAction(nameof(Register));
            }

            // Verificar si el carnet ya existe
            var carnetExiste = await _context.Estudiante
                .AnyAsync(s => s.carnet == model.Carnet);

            if (carnetExiste)
            {
                _notyf.Error("El carnet ya está registrado en el sistema.");
                TempData.Keep("NewUserId");
                TempData.Keep("NewUserRol");
                return View(model);
            }

            // Crear perfil Student
            var student = new Student
            {
                IdEst = Guid.NewGuid(),
                Nombre = model.Nombre,
                carnet = model.Carnet,
                DocumentoID = model.DocumentoID,
                numeroCelular = model.NumeroCelular,
                ApplicationUserId = userId
            };

            _context.Estudiante.Add(student);
            await _context.SaveChangesAsync();

            // Login automático
            await SignInUserAsync(user);

            _notyf.Success("¡Perfil de estudiante completado exitosamente!");
            return RedirectToAction("Index", "Home");
        }

        // ════════════════════════════════════════════
        //  LOGIN
        // ════════════════════════════════════════════

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
            => View(new LoginViewModel { ReturnUrl = returnUrl });

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Buscar por correo o nombre de usuario
            var user = await _context.Users
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u =>
                    u.CorreoElectrónico == model.Email ||
                    u.usuario == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Credenciales inválidas.");
                return View(model);
            }

            // Verificar bloqueo de cuenta
            if (user.Estado == "Bloqueado")
                return View("Lockout");

            // Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                _notyf.Warning("Credenciales inválidas.");
                ModelState.AddModelError("", "Credenciales inválidas.");
                return View(model);
            }

            // Verificar estado activo
            if (user.Estado != "Activo")
            {
                ModelState.AddModelError("", "Tu cuenta no está activa. Contacta al administrador.");
                return View(model);
            }

            await SignInUserAsync(user, model.RememberMe);

            _notyf.Success($"¡Bienvenido, {user.usuario}!");

            // Redirigir según rol
            string rolNombre = user.Rol?.nombreRol ?? RolService.ROL_STUDENT;

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return rolNombre switch
            {
                RolService.ROL_SUPERADMIN => RedirectToAction("Index", "Administrator"),
                RolService.ROL_LENDER => RedirectToAction("Index", "Loan"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // ════════════════════════════════════════════
        //  LOGOUT
        // ════════════════════════════════════════════

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _notyf.Information("Sesión cerrada correctamente.");
            return RedirectToAction("Index", "Home");
        }

        // ════════════════════════════════════════════
        //  PERFIL
        // ════════════════════════════════════════════

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userGuid = ObtenerUserIdDeClaims();
            if (userGuid == null) return RedirectToAction(nameof(Login));

            var user = await _context.Users
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.idUsuario == userGuid.Value);

            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                UserName = user.usuario ?? "",
                Email = user.CorreoElectrónico ?? "",
                NombreRol = user.Rol?.nombreRol ?? "Sin rol",
                Estado = user.Estado ?? "Desconocido",
                FechaRegistro = user.fechaRegistro,
                TwoFactorEnabled = !string.IsNullOrEmpty(user.codigo2FA)
            };

            return View(model);
        }

        // ════════════════════════════════════════════
        //  CONFIGURACIÓN / SETTINGS
        // ════════════════════════════════════════════

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var userGuid = ObtenerUserIdDeClaims();
            if (userGuid == null) return RedirectToAction(nameof(Login));

            var user = await _context.Users.FindAsync(userGuid.Value);
            if (user == null) return NotFound();

            var model = new SettingsViewModel
            {
                CurrentEmail = user.CorreoElectrónico ?? "",
                TwoFactorEnabled = !string.IsNullOrEmpty(user.codigo2FA)
            };

            return View(model);
        }

        // ════════════════════════════════════════════
        //  CAMBIO DE CONTRASEÑA
        // ════════════════════════════════════════════

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

            var userGuid = ObtenerUserIdDeClaims();
            if (userGuid == null) return RedirectToAction(nameof(Login));

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

        // ════════════════════════════════════════════
        //  GESTIÓN DE ROLES (SuperAdmin)
        // ════════════════════════════════════════════

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> RolesIndex()
        {
            var response = await _rolService.GetAllRolesAsync();
            return View(response.IsSuccess ? response.Result : new List<setRolDTO>());
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult CreateRol() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateRol(setRolDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var response = await _rolService.CreateRolAsync(dto);
            if (!response.IsSuccess)
            {
                _notyf.Error(response.Message ?? "Error al crear rol.");
                return View(dto);
            }

            _notyf.Success("Rol creado correctamente.");
            return RedirectToAction(nameof(RolesIndex));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> EditRol(Guid id)
        {
            var response = await _rolService.GetRolByIdAsync(id);
            if (!response.IsSuccess) return RedirectToAction(nameof(RolesIndex));
            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> EditRol(Guid id, setRolDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var response = await _rolService.UpdateRolAsync(id, dto);
            if (!response.IsSuccess)
            {
                _notyf.Error(response.Message ?? "Error al actualizar rol.");
                return View(dto);
            }

            _notyf.Success("Rol actualizado correctamente.");
            return RedirectToAction(nameof(RolesIndex));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteRol(Guid id)
        {
            var response = await _rolService.DeleteRolAsync(id);

            if (!response.IsSuccess)
                _notyf.Error(response.Message ?? "Error al eliminar rol.");
            else
                _notyf.Success("Rol eliminado.");

            return RedirectToAction(nameof(RolesIndex));
        }

        // ════════════════════════════════════════════
        //  MÉTODOS PRIVADOS AUXILIARES
        // ════════════════════════════════════════════

        /// <summary>
        /// Genera las claims y firma la cookie de autenticación.
        /// Incluye el nombre del Rol como claim para que [Authorize(Roles="...")] funcione.
        /// </summary>
        private async Task SignInUserAsync(ApplicationUser user, bool isPersistent = false)
        {
            // Recargar usuario con rol si no viene incluido
            string rolNombre = user.Rol?.nombreRol
                ?? (await _context.Users
                        .Include(u => u.Rol)
                        .FirstOrDefaultAsync(u => u.idUsuario == user.idUsuario))
                    ?.Rol?.nombreRol
                ?? RolService.ROL_STUDENT;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,           user.usuario          ?? ""),
                new Claim(ClaimTypes.Email,          user.CorreoElectrónico ?? ""),
                new Claim(ClaimTypes.Role,           rolNombre),
                new Claim(ClaimTypes.NameIdentifier, user.idUsuario.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = isPersistent,
                    ExpiresUtc = DateTime.UtcNow.AddHours(8)
                });
        }

        /// <summary>Obtiene el Guid del usuario autenticado desde las Claims.</summary>
        private Guid? ObtenerUserIdDeClaims()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(idStr, out var guid) ? guid : null;
        }

        /// <summary>Valida que exista TempData del proceso de registro.</summary>
        private bool ValidarTempDataRegistro()
        {
            if (TempData["NewUserId"] == null)
            {
                _notyf.Error("Sesión expirada. Por favor, regístrate nuevamente.");
                return false;
            }
            return true;
        }

        /// <summary>Intenta parsear el UserId desde TempData.</summary>
        private bool ObtenerUserId(out Guid userId)
        {
            var idStr = TempData["NewUserId"]?.ToString();
            if (string.IsNullOrEmpty(idStr) || !Guid.TryParse(idStr, out userId))
            {
                userId = Guid.Empty;
                return false;
            }
            return true;
        }

        /// <summary>Traduce el nombre del rol a español para mensajes UI.</summary>
        private static string TraducirRol(string rol) => rol switch
        {
            RolService.ROL_SUPERADMIN => "SuperAdministrador",
            RolService.ROL_LENDER => "Prestamista",
            _ => "Estudiante"
        };
    }
}