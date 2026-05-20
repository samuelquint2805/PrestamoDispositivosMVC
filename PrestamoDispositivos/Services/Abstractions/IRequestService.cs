using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
   
        /// <summary>
        /// Servicio de Solicitudes de préstamo.
        /// Integra el patrón Observer: cada cambio de estado notifica a los observadores registrados.
        /// </summary>
        public interface IRequestService
        {
            // ── CONSULTAS ────────────────────────────────────────────────

            /// <summary>Obtiene todas las solicitudes (uso: Lender / SuperAdmin).</summary>
            Task<Response<List<RequestoDTO>>> GetAllRequestsAsync();

            /// <summary>Obtiene todas las solicitudes de un estudiante específico.</summary>
            Task<Response<List<RequestoDTO>>> GetRequestsByUserAsync(Guid idUsuario);

            /// <summary>Obtiene una solicitud por su ID.</summary>
            Task<Response<RequestoDTO>> GetRequestByIdAsync(Guid idSolicitud);

            /// <summary>Obtiene solicitudes filtradas por estado (Pendiente, Aprobada, Rechazada).</summary>
            Task<Response<List<RequestoDTO>>> GetRequestsByStatusAsync(string estado);

        // ── CICLO DE VIDA ────────────────────────────────────────────

        /// <summary>
        /// El estudiante crea una solicitud de reserva para un dispositivo.
        /// Estado inicial: "Pendiente".
        /// Dispara Observer → notifica cambio.
        /// </summary>
        Task<Response<List<RequestoDTO>>> GetRequestsByUserAndStatusAsync(Guid idUsuario, string estado);
        Task<Response<RequestoDTO>> CreateRequestAsync(RequestoDTO dto, Guid idDispo);

            /// <summary>
            /// El Prestamista / SuperAdmin aprueba o rechaza una solicitud.
            /// Dispara Observer → notifica cambio a todos los observadores.
            /// </summary>
            Task<Response<RequestoDTO>> ReviewRequestAsync(RequestoDTO dto, Guid? idDispo);

            /// <summary>
            /// El estudiante cancela su propia solicitud (solo si está Pendiente).
            /// Dispara Observer → notifica cambio.
            /// </summary>
            Task<Response<bool>> CancelRequestAsync(Guid idSolicitud, Guid idUsuario);

            /// <summary>Elimina una solicitud (solo SuperAdmin).</summary>
            Task<Response<bool>> DeleteRequestAsync(Guid idSolicitud);
        }
}
