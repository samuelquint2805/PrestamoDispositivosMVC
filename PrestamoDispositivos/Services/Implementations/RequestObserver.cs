using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.Models;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Services.Implementations
{
    public class RequestObserver
    {
        /// <summary>
        /// Observador de Auditoría: registra en BD cada cambio de estado de una solicitud.
        /// </summary>
        public class AuditRequestObserver : Irequestobserver
        {
            private readonly DatacontextPres _context;
            private readonly ILogger<AuditRequestObserver> _logger;

            public AuditRequestObserver(DatacontextPres context, ILogger<AuditRequestObserver> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task Update(Guid idSolicitud, string nuevoEstado, string mensaje)
            {
                try
                {
                    var audit = new AuditReportsClass
                    {
                        IdAudit = Guid.NewGuid(),
                        accion = $"Solicitud {idSolicitud} → {nuevoEstado}",
                        FechaEvento = DateTime.UtcNow,
                        Descripcion = mensaje
                    };

                    _context.ReportesyAuditorias.Add(audit);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("[AUDIT] Solicitud {Id} cambió a '{Estado}': {Msg}",
                        idSolicitud, nuevoEstado, mensaje);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[AUDIT] Error al registrar auditoría de solicitud {Id}", idSolicitud);
                }
            }
        }

        /// <summary>
        /// Observador de Notificación: escribe en el log (y podría enviar email/push).
        /// Se puede extender para integrar IEmailSender, SignalR, etc.
        /// </summary>
        public class NotificationRequestObserver : Irequestobserver
        {
            private readonly ILogger<NotificationRequestObserver> _logger;

            public NotificationRequestObserver(ILogger<NotificationRequestObserver> logger)
            {
                _logger = logger;
            }

            public Task Update(Guid idSolicitud, string nuevoEstado, string mensaje)
            {
                // Aquí se puede inyectar IEmailSender o IHubContext<NotificationHub>
                // para enviar notificaciones en tiempo real al estudiante.
                _logger.LogInformation("[NOTIFY] Solicitud {Id} → '{Estado}': {Msg}",
                    idSolicitud, nuevoEstado, mensaje);

                // Ejemplo futuro:
                // await _emailSender.SendAsync(userEmail, "Tu solicitud fue " + nuevoEstado, mensaje);

                return Task.CompletedTask;
            }
        }
    }
}
