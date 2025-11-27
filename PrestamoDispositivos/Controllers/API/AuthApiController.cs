using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.Services.Abstractions;
using System.ComponentModel.DataAnnotations;


namespace PrestamoDispositivos.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] // Este controlador no requiere autenticación
    public class AuthApiController : ControllerBase
    {
        private readonly DatacontextPres _context;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthApiController> _logger;
        private readonly INotyfService _notyfService;

        public AuthApiController ( DatacontextPres context, IJwtTokenService jwtTokenService, ILogger<AuthApiController> logger, INotyfService notyfService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
            _notyfService = notyfService;
        }

       
        [HttpPost("login")]
      
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return (IActionResult)Response<LoginRequest>.Failure("Datos de login inválidos" );
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email || u.UserName == request.Email);

            if (user == null)
            {
                _logger.LogWarning($"Intento de login fallido - Usuario no encontrado: {request.Email}");
                return Unauthorized(Response<object>.Failure("Credenciales inválidas"));
            }

            // Verificar bloqueo
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            {
                _logger.LogWarning($"Intento de login - Cuenta bloqueada: {user.Email}");
                return Unauthorized(Response<object>.Failure("Cuenta bloqueada temporalmente"));
            }

            // Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                user.AccessFailedCount++;

                if (user.AccessFailedCount >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    user.AccessFailedCount = 0;
                    _logger.LogWarning($"Cuenta bloqueada por intentos fallidos: {user.Email}");
                }

                await _context.SaveChangesAsync();
                return Unauthorized(Response<object>.Failure("Credenciales inválidas"));
            }

            // Login exitoso
            user.AccessFailedCount = 0;
            await _context.SaveChangesAsync();

            // Generar token
            var token = _jwtTokenService.GenerateToken(user);

            _logger.LogInformation($"✅ Login exitoso - Usuario: {user.Email}, Rol: {user.Role}");

            return Ok(Response<object>.Success(new
            {
                token = token,
                user = new
                {
                    id = user.Id,
                    userName = user.UserName,
                    email = user.Email,
                    role = user.Role
                },
                expiresIn = 3600 // 60 minutos
            }, "Autenticación exitosa"));
        }

        [HttpPost("refresh")]
        [Authorize] // Requiere token válido (aunque esté expirado)
       
        public async Task<IActionResult> RefreshToken()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(Response<object>.Failure("Token inválido"));
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return Unauthorized(Response<object>.Failure("Usuario no encontrado"));
            }

            var newToken = _jwtTokenService.GenerateToken(user);

            return Ok(Response<object>.Success(new
            {
                token = newToken,
                expiresIn = 3600
            }, "Token refrescado exitosamente"));
        }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "El email o usuario es requerido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = string.Empty;
    }
}