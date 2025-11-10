using System;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.Extensions.Configuration;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.Models;

namespace PrestamoDispositivos.Services.Implementations
{
    public class TwoFactorService
    {
        private readonly DatacontextPres _db;
        private readonly IConfiguration _config;
        private readonly SmtpEmailSender _smtp;
        private readonly INotyfService _notyf;

        public TwoFactorService(DatacontextPres db, IConfiguration config, SmtpEmailSender smtp, INotyfService notyf)
        {
            _db = db;
            _config = config;
            _smtp = smtp;
            _notyf = notyf;
        }

        public async Task<string> GenerateAndSendCodeAsync(ApplicationUser user)
        {
            var code = new Random().Next(100000, 999999).ToString();
            user.TwoFactorCode = code;
            user.TwoFactorCodeExpiry = DateTime.UtcNow.AddMinutes(5);
            await _db.SaveChangesAsync();

            // Intentar enviar por SMTP si está configurado
            var smtpHost = _config.GetSection("Smtp")["Host"];
            if (!string.IsNullOrEmpty(smtpHost))
            {
                var subject = "Código de verificación - PrestamoDispositivos";
                var body = $"Tu código de verificación es: <strong>{code}</strong>. Expira en 5 minutos.";
                await _smtp.SendEmailAsync(user.Email, subject, body);
            }
            else
            {
                // Para desarrollo: mostrar el código con Notyf (no usar en producción)
                _notyf.Success($"Código 2FA generado: {code} (solo para desarrollo)");
            }

            return code;
        }

        public bool ValidateCode(ApplicationUser user, string code)
        {
            if (user.TwoFactorCode == null) return false;
            if (user.TwoFactorCodeExpiry == null) return false;
            if (DateTime.UtcNow > user.TwoFactorCodeExpiry) return false;
            return string.Equals(user.TwoFactorCode, code, StringComparison.Ordinal);
        }
    }
}
