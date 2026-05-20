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

                    // Mapeo manual usando tu helper
                    var dtoList = list.Select(MapToDto).ToList();

                    // PASA EL DTO AL RESULTADO
                    return Response<List<RequestoDTO>>.Success(dtoList, "Lista de solicitudes obtenida");
                }
                catch (Exception ex)
                {
                    return Response<List<RequestoDTO>>.Failure($"Error: {ex.Message}");
                }
            }

        public async Task<Response<List<RequestoDTO>>> GetRequestsByUserAsync(Guid idUsuario)
        {
            try
            {
                // 1. Obtener datos de la base de datos
                var list = await _context.solicitud
                    .AsNoTracking()
                    .Include(r => r.User)
                    .Where(r => r.idUser == idUsuario)
                    .OrderByDescending(r => r.FechaSolicitud)
                    .ToListAsync();

                // 2. Mapear a DTO usando tu helper privado MapToDto
                var dtoList = list.Select(MapToDto).ToList();

                // 3. RETORNO CORREGIDO: Pasamos la lista mapeada como primer argumento
                return Response<List<RequestoDTO>>.Success(dtoList, "Solicitudes obtenidas correctamente");
            }
            catch (Exception ex)
            {
                return Response<List<RequestoDTO>>.Failure($"Error al obtener solicitudes del usuario: {ex.Message}");
            }
        }

        public async Task<Response<List<RequestoDTO>>> GetRequestsByUserAndStatusAsync(Guid idUsuario, string estado)
        {
            try
            {
                var list = await _context.solicitud
                    .AsNoTracking()
                    .Include(r => r.User)
                    .Where(r => r.idUser == idUsuario && r.EstadoSolicitud == estado)
                    .OrderByDescending(r => r.FechaSolicitud)
                    .ToListAsync();

                return Response<List<RequestoDTO>>.Success(list.Select(MapToDto).ToList(), "Solicitudes filtradas");
            }
            catch (Exception ex)
            {
                return Response<List<RequestoDTO>>.Failure($"Error: {ex.Message}");
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

                return Response<RequestoDTO>.Success(MapToDto(req),"Solicitud obtenida correctamente");
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

        public async Task<Response<RequestoDTO>> ReviewRequestAsync(RequestoDTO dto, Guid? idDispo = null)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                string nuevoEstado = dto.NuevoEstado ?? dto.EstadoSolicitud ?? "";

                if (nuevoEstado != ESTADO_APROBADA && nuevoEstado != ESTADO_RECHAZADA)
                    return Response<RequestoDTO>.Failure("Estado no válido. Use 'Aprobada' o 'Rechazada'.");

                // ── Cargar la solicitud con su usuario ──
                var solicitud = await _context.solicitud
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.IdSolicitud == dto.IdSolicitud);

                if (solicitud == null)
                    return Response<RequestoDTO>.Failure("Solicitud no encontrada.");

                if (solicitud.EstadoSolicitud != ESTADO_PENDIENTE)
                    return Response<RequestoDTO>.Failure(
                        $"Solo se pueden revisar solicitudes Pendientes. Estado actual: {solicitud.EstadoSolicitud}.");

                // ── 1. Actualizar estado de la solicitud ──
                solicitud.EstadoSolicitud = nuevoEstado;
                solicitud.FechaAprobacion = DateTime.UtcNow;
                _context.solicitud.Update(solicitud);

                // ── 2. Si se APRUEBA: crear Loan y marcar dispositivo ──
                if (nuevoEstado == ESTADO_APROBADA)
                {
                    if (idDispo == null)
                        return Response<RequestoDTO>.Failure(
                            "Debes seleccionar el dispositivo a prestar al aprobar la solicitud.");

                    var device = await _context.Dispositivos
                        .FirstOrDefaultAsync(d => d.IdDisp == idDispo);

                    if (device == null)
                        return Response<RequestoDTO>.Failure("Dispositivo no encontrado.");

                    if (device.EstadoEquipo?.ToLower() != "disponible")
                        return Response<RequestoDTO>.Failure(
                            $"El dispositivo ya no está disponible (Estado: {device.EstadoEquipo}).");

                    // Crear el préstamo — relacionado con el User de la solicitud
                    var nuevoPrestamo = new Loan
                    {
                        IdPrestamos = Guid.NewGuid(),
                        FechaEvento = DateTime.UtcNow,
                        EstadoPrestamo = "Prestado",
                        IdDispo = device.IdDisp,
                        IdUser = solicitud.idUser   // mismo user de la solicitud
                    };

                    _context.Prestamos.Add(nuevoPrestamo);

                    // Marcar el dispositivo como prestado
                    device.EstadoEquipo = "Prestado";
                    _context.Dispositivos.Update(device);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _observable.NotifyAll(
                    solicitud.IdSolicitud, nuevoEstado,
                    $"Solicitud {nuevoEstado.ToLower()} por el prestamista.");

                string mensaje = nuevoEstado == ESTADO_APROBADA
                    ? "Solicitud aprobada y préstamo creado correctamente."
                    : "Solicitud rechazada correctamente.";

                return Response<RequestoDTO>.Success(MapToDto(solicitud), mensaje);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
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
