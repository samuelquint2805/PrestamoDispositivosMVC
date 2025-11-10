using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    // Clase de usuario para autenticación manual (no Identity)
    public class ApplicationUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [ForeignKey("User")]
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? User { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Password hash (guardado con BCrypt)
        public string PasswordHash { get; set; } = string.Empty;

        // Rol (por simplicidad lo vamos a mantenr como propiedad)
        public string Role { get; set; } = "Student";

        // 2FA
        public bool TwoFactorEnabled { get; set; } = false;
        public string? TwoFactorCode { get; set; }
        public DateTime? TwoFactorCodeExpiry { get; set; }

        // Lockout / fallos
        public bool LockoutEnabled { get; set; } = true;
        public int AccessFailedCount { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; }
    }
}