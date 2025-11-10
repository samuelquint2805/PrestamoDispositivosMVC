using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace PrestamoDispositivos.Services.Implementations
{
    public class SmtpEmailSender
    {
        private readonly IConfiguration _config;
        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var smtp = _config.GetSection("Smtp");
            var host = smtp["Host"];
            if (string.IsNullOrEmpty(host)) return; // SMTP no configurado

            var port = int.Parse(smtp["Port"] ?? "25");
            var user = smtp["User"];
            var pass = smtp["Pass"];
            var from = smtp["From"] ?? user;

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = bool.Parse(smtp["EnableSsl"] ?? "true"),
            };

            if (!string.IsNullOrEmpty(user))
                client.Credentials = new NetworkCredential(user, pass);

            var mail = new MailMessage(from, toEmail, subject, htmlMessage)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mail);
        }
    }
}