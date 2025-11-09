using Microsoft.AspNetCore.Identity;

namespace PrestamoDispositivos.Models
{
    public class ApplicationUser 
    {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string UserName { get; set; }
            public string Email { get; set; }
            public string PasswordHash { get; set; }
            public string Role { get; set; }
            public bool TwoFactorEnabled { get; set; } = false;
            public string? TwoFactorCode { get; set; }
            public DateTime? LockoutEnd { get; set; }
        
    }
}
