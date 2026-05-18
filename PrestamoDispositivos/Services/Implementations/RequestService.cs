using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Models;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Services.Implementations
{
    public class RequestService : IRequestService
    {
        private readonly DatacontextPres _context;
        private readonly IRequestObservable _observable;

        // Estados válidos del sistema
        public const string ESTADO_PENDIENTE = "Pendiente";
        public const string ESTADO_APROBADA = "Aprobada";
        public const string ESTADO_RECHAZADA = "Rechazada";
        public const string ESTADO_CANCELADA = "Cancelada";

        public RequestService(DatacontextPres context, IRequestObservable observable)
        {
            _context = context;
            _observable = observable;
        }

        // ── CONSULTAS ────────────────────────────────────────────────

        public async Task<Response<List<RequestoDTO>>> GetAllRequestsAsync()
        {
            try
            {
                var list = await _context.solicitud
                    .AsNoTracking()
                    .Include(r => r.User)
                    .OrderByDescending(r => r.FechaSolicitud)
                    .ToListAsync();

                return Response<List<RequestoDTO>>.Success("Lista de solicitudes obtenida ");
            }
            catch (Exception ex)
            {
                return Response<List<RequestoDTO>>.Failure($"Error al obtener solicitudes: {ex.Message}");
            }
        }

        public async Task<Response<List<RequestoDTO>>> GetRequestsByUserAsync(Guid idUsuario)
        {
            try
            {
                var list = await _context.solicitud
                    .AsNoTracking()
                    .Include(r => r.User)
                    .Where(r => r.idUser == idUsuario)
                    .OrderByDescending(r => r.FechaSolicitud)
                    .ToListAsync();

                return Response<List<RequestoDTO>>.Success("Solicitud obtenida correctamente");
            }
            catch (Exception ex)
            {
                return Response<List<RequestoDTO>>.Failure($"Error al obtener solicitudes del usuario: {ex.Message}");
            }
        }

        public async Task<Response<RequestoDTO>> GetRequestByIdAsync(Guid idSolicitud)
        {
            try
            {
                var req = await _context.solicitud
                    .AsNoTracking()
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.IdSolicitud == idSolicitud);

                if (req == null)
                    return Response<RequestoDTO>.Failure("Solicitud no encontrada.");

                return Response<RequestoDTO>.Success("Solicitud obtenida correctamente");
            }
            catch (Exception ex)
            {
                return Response<RequestoDTO>.Failure($"Error al obtener solicitud: {ex.Message}");
            }
        }

        public async Task<Response<List<RequestoDTO>>> GetRequestsByStatusAsync(string estado)
        {
            try
            {
                var list = await _context.solicitud
                    .AsNoTracking()
                    .Include(r => r.User)
                    .Where(r => r.EstadoSolicitud == estado)
                    .OrderByDescending(r => r.FechaSolicitud)
                    .ToListAsync();

                return Response<List<RequestoDTO>>.Success("Solicitud obtenida correctamente");
            }
            catch (Exception ex)
            {
                return Response<List<RequestoDTO>>.Failure($"Error al filtrar solicitudes: {ex.Message}");
            }
        }

        // ── CICLO DE VIDA ────────────────────────────────────────────

        public async Task<Response<RequestoDTO>> CreateRequestAsync(RequestoDTO dto, Guid idDispo)
        {
            try
            {
                // Verificar que el dispositivo existe y está disponible
                var device = await _context.Dispositivos
                    .FirstOrDefaultAsync(x => x.IdDisp == idDispo);

                if (device == null)
                    return Response<RequestoDTO>.Failure("Dispositivo no encontrado.");

                if (device.EstadoEquipo?.ToLower() != "disponible")
                    return Response<RequestoDTO>.Failure(
                        $"El dispositivo no está disponible. Estado actual: {device.EstadoEquipo}.");

                // Verificar que el usuario no tenga ya una solicitud pendiente para este mismo dispositivo
                // (regla de negocio: un estudiante no puede pedir el mismo equipo dos veces)
                var yaExiste = await _context.solicitud
                    .AnyAsync(r => r.idUser == dto.idUser
                               && r.EstadoSolicitud == ESTADO_PENDIENTE);

                if (yaExiste)
                    return Response<RequestoDTO>.Failure(
                        "Ya tienes una solicitud pendiente. Espera a que sea procesada antes de hacer otra.");

                var nuevaSolicitud = new Request
                {
                    IdSolicitud = Guid.NewGuid(),
                    FechaSolicitud = DateTime.UtcNow,
                    FechaAprobacion = DateTime.MinValue,   // se asigna al aprobar/rechazar
                    EstadoSolicitud = ESTADO_PENDIENTE,
                    idUser = dto.idUser
                };

                _context.solicitud.Add(nuevaSolicitud);
                await _context.SaveChangesAsync();

                // ── PATRÓN OBSERVER: notificar creación ──────────────
                await _observable.NotifyAll(
                    nuevaSolicitud.IdSolicitud,
                    ESTADO_PENDIENTE,
                    $"Nueva solicitud creada por el usuario {dto.idUser} para el dispositivo Seleccionado."
                );

                return Response<RequestoDTO>.Success(MapToDto(nuevaSolicitud));
            }
            catch (Exception ex)
            {
                return Response<RequestoDTO>.Failure($"Error al crear solicitud: {ex.Message}");
            }
        }

        public async Task<Response<RequestoDTO>> ReviewRequestAsync(RequestoDTO dto)
        {
            try
            {
                // Validar estado nuevo
                if (dto.EstadoSolicitud != ESTADO_APROBADA && dto.EstadoSolicitud != ESTADO_RECHAZADA)
                    return Response<RequestoDTO>.Failure(
                        "Estado no válido. Use 'Aprobada' o 'Rechazada'.");

                var solicitud = await _context.solicitud
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.IdSolicitud == dto.IdSolicitud);

                if (solicitud == null)
                    return Response<RequestoDTO>.Failure("Solicitud no encontrada.");

                if (solicitud.EstadoSolicitud != ESTADO_PENDIENTE)
                    return Response<RequestoDTO>.Failure(
                        $"Solo se pueden revisar solicitudes Pendientes. Estado actual: {solicitud.EstadoSolicitud}.");

                solicitud.EstadoSolicitud = dto.EstadoSolicitud;
                solicitud.FechaAprobacion = DateTime.UtcNow;

                _context.solicitud.Update(solicitud);
                await _context.SaveChangesAsync();

                // ── PATRÓN OBSERVER: notificar revisión ──────────────
                string obs = string.IsNullOrWhiteSpace(dto.EstadoSolicitud)
                    ? $"Solicitud {dto.EstadoSolicitud.ToLower()} por el prestamista."
                    : dto.EstadoSolicitud;

                await _observable.NotifyAll(solicitud.IdSolicitud, dto.EstadoSolicitud, obs);

                return Response<RequestoDTO>.Success(MapToDto(solicitud));
            }
            catch (Exception ex)
            {
                return Response<RequestoDTO>.Failure($"Error al revisar solicitud: {ex.Message}");
            }
        }

        public async Task<Response<bool>> CancelRequestAsync(Guid idSolicitud, Guid idUsuario)
        {
            try
            {
                var solicitud = await _context.solicitud
                    .FirstOrDefaultAsync(r => r.IdSolicitud == idSolicitud && r.idUser == idUsuario);

                if (solicitud == null)
                    return Response<bool>.Failure("Solicitud no encontrada o no pertenece a este usuario.");

                if (solicitud.EstadoSolicitud != ESTADO_PENDIENTE)
                    return Response<bool>.Failure(
                        "Solo puedes cancelar solicitudes que estén Pendientes.");

                solicitud.EstadoSolicitud = ESTADO_CANCELADA;
                solicitud.FechaAprobacion = DateTime.UtcNow;

                _context.solicitud.Update(solicitud);
                await _context.SaveChangesAsync();

                // ── PATRÓN OBSERVER: notificar cancelación ───────────
                await _observable.NotifyAll(
                    idSolicitud,
                    ESTADO_CANCELADA,
                    $"Solicitud cancelada por el estudiante {idUsuario}."
                );

                return Response<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure($"Error al cancelar solicitud: {ex.Message}");
            }
        }

        public async Task<Response<bool>> DeleteRequestAsync(Guid idSolicitud)
        {
            try
            {
                var solicitud = await _context.solicitud.FindAsync(idSolicitud);
                if (solicitud == null)
                    return Response<bool>.Failure("Solicitud no encontrada.");

                _context.solicitud.Remove(solicitud);
                await _context.SaveChangesAsync();

                return Response<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure($"Error al eliminar solicitud: {ex.Message}");
            }
        }

        // ── HELPER PRIVADO ───────────────────────────────────────────

        private static RequestoDTO MapToDto(Request r) => new RequestoDTO
        {
            IdSolicitud = r.IdSolicitud,
            FechaSolicitud = r.FechaSolicitud,
            FechaAprobacion = r.FechaAprobacion,
            EstadoSolicitud = r.EstadoSolicitud,
            idUser = r.idUser,
            User = r.User == null ? null : new ApplicationUserDTO
            {
                idUsuario = r.User.idUsuario,
                usuario = r.User.usuario,
                CorreoElectrónico = r.User.CorreoElectrónico,
                Estado = r.User.Estado,
                fechaRegistro = r.User.fechaRegistro,
                PasswordHash = r.User.PasswordHash,
                codigo2FA = r.User.codigo2FA
            }
        };
    }
}
