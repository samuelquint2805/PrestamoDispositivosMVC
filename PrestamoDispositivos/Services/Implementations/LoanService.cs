using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Models;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Services.Implementations
{
    public class LoanService : ILoanService
    {
        private readonly DatacontextPres _context;
        private readonly IMapper _mapper;

        public LoanService(DatacontextPres context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }



        // ── OBTENER TODOS ────────────────────────────────────────────

        public async Task<Response<List<LoanDTO>>> GetAllLoansAsync()
        {
            try
            {
                var loans = await _context.Prestamos
                    .Include(l => l.User)
                    .Include(l => l.Dispositivo)
                    .OrderByDescending(l => l.FechaEvento)
                    .ToListAsync();

                return Response<List<LoanDTO>>.Success(
                    _mapper.Map<List<LoanDTO>>(loans), "Lista obtenida correctamente");
            }
            catch (Exception ex)
            {
                return Response<List<LoanDTO>>.Failure($"Error al obtener préstamos: {ex.Message}");
            }
        }

        // ── OBTENER POR USUARIO (Student ve los suyos) ───────────────
        // Busca por ApplicationUser.idUsuario directamente en Loan.IdUser
        public async Task<Response<List<LoanDTO>>> GetLoansByUserAsync(Guid idUser)
        {
            try
            {
                var loans = await _context.Prestamos
                    .Include(l => l.User)
                    .Include(l => l.Dispositivo)
                    .Where(l => l.IdUser == idUser)
                    .OrderByDescending(l => l.FechaEvento)
                    .ToListAsync();

                return Response<List<LoanDTO>>.Success(
                    _mapper.Map<List<LoanDTO>>(loans), "Préstamos del usuario obtenidos");
            }
            catch (Exception ex)
            {
                return Response<List<LoanDTO>>.Failure($"Error al obtener préstamos del usuario: {ex.Message}");
            }
        }

        // ── OBTENER POR ESTUDIANTE (legacy, por si lo usas en otro lado) ──
        public async Task<Response<List<LoanDTO>>> GetAllLoansPerStudentAsync(Guid idEst)
        {
            try
            {
                var loans = await _context.Prestamos
                    .Include(l => l.User.studentUsuario)
                    .Include(l => l.Dispositivo)
                    .Where(l => l.User.studentUsuario.IdEst == idEst)
                    .OrderByDescending(l => l.FechaEvento)
                    .ToListAsync();

                return Response<List<LoanDTO>>.Success(
                    _mapper.Map<List<LoanDTO>>(loans), "Préstamos del estudiante obtenidos");
            }
            catch (Exception ex)
            {
                return Response<List<LoanDTO>>.Failure($"Error: {ex.Message}");
            }
        }

        // ── OBTENER POR ID ───────────────────────────────────────────

        public async Task<Response<LoanDTO>> GetLoanByIdAsync(Guid id)
        {
            try
            {
                var loan = await _context.Prestamos
                    .Include(l => l.User)
                    .Include(l => l.Dispositivo)
                    .FirstOrDefaultAsync(l => l.IdPrestamos == id);

                if (loan == null)
                    return Response<LoanDTO>.Failure("Préstamo no encontrado.");

                return Response<LoanDTO>.Success(_mapper.Map<LoanDTO>(loan), "Préstamo encontrado");
            }
            catch (Exception ex)
            {
                return Response<LoanDTO>.Failure($"Error: {ex.Message}");
            }
        }

        // ── CREAR ─────────────────────────────────────────────────────
        // Se llama desde RequestService al aprobar una solicitud.
        // Regla: el usuario no puede tener más de 1 préstamo activo.

        public async Task<Response<LoanDTO>> CreateLoanAsync(LoanDTO dto)
        {
            try
            {
                // ── Regla de negocio: máximo 1 préstamo activo por usuario ──
                var tieneActivo = await _context.Prestamos
                    .AnyAsync(l => l.IdUser == dto.IdUser
                               && l.EstadoPrestamo != "Devuelto"
                               && l.EstadoPrestamo != "Finalizado"
                               && l.EstadoPrestamo != "Cancelar");

                if (tieneActivo)
                    return Response<LoanDTO>.Failure(
                        "El estudiante ya tiene un préstamo activo. Debe devolverlo antes de solicitar otro.");

                var device = await _context.Dispositivos.FindAsync(dto.IdDispo);
                if (device == null)
                    return Response<LoanDTO>.Failure("Dispositivo no encontrado.");

                if (device.EstadoEquipo?.ToLower() != "disponible")
                    return Response<LoanDTO>.Failure(
                        $"El dispositivo no está disponible. Estado: {device.EstadoEquipo}.");

                dto.IdPrestamos = Guid.NewGuid();
                var loan = _mapper.Map<Loan>(dto);

                await _context.Prestamos.AddAsync(loan);

                device.EstadoEquipo = "Prestado";
                _context.Dispositivos.Update(device);

                await _context.SaveChangesAsync();

                return Response<LoanDTO>.Success(dto, "Préstamo creado correctamente.");
            }
            catch (Exception ex)
            {
                return Response<LoanDTO>.Failure($"Error al crear el préstamo: {ex.Message}");
            }
        }

        // ── ACTUALIZAR ───────────────────────────────────────────────

        public async Task<Response<LoanDTO>> UpdateLoanAsync(Guid id, LoanDTO dto)
        {
            try
            {
                var loan = await _context.Prestamos.FirstOrDefaultAsync(l => l.IdPrestamos == id);
                if (loan == null)
                    return Response<LoanDTO>.Failure("Préstamo no encontrado.");

                _mapper.Map(dto, loan);
                _context.Prestamos.Update(loan);
                await _context.SaveChangesAsync();

                return Response<LoanDTO>.Success(dto, "Préstamo actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return Response<LoanDTO>.Failure($"Error al actualizar: {ex.Message}");
            }
        }

        // ── ELIMINAR ─────────────────────────────────────────────────

        public async Task<Response<bool>> DeleteLoanAsync(Guid id)
        {
            try
            {
                var loan = await _context.Prestamos.FindAsync(id);
                if (loan == null)
                    return Response<bool>.Failure("Préstamo no encontrado.");

                _context.Prestamos.Remove(loan);
                await _context.SaveChangesAsync();

                return Response<bool>.Success(true, "Préstamo eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure($"Error al eliminar: {ex.Message}");
            }
        }

        // ── DEVOLVER DISPOSITIVO ─────────────────────────────────────

        public async Task<Response<bool>> ReturnDeviceAsync(Guid loanId)
        {
            try
            {
                var loan = await _context.Prestamos
                    .Include(l => l.Dispositivo)
                    .FirstOrDefaultAsync(l => l.IdPrestamos == loanId);

                if (loan == null)
                    return Response<bool>.Failure("Préstamo no encontrado.");

                if (loan.EstadoPrestamo == "Finalizado" || loan.EstadoPrestamo == "Devuelto")
                    return Response<bool>.Failure("Este préstamo ya fue finalizado.");

                loan.EstadoPrestamo = "Finalizado";

                if (loan.Dispositivo != null)
                {
                    loan.Dispositivo.EstadoEquipo = "Disponible";
                    _context.Dispositivos.Update(loan.Dispositivo);
                }

                _context.Prestamos.Update(loan);
                await _context.SaveChangesAsync();

                return Response<bool>.Success(true, "Dispositivo devuelto correctamente.");
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure($"Error al devolver: {ex.Message}");
            }
        }

        // ── AUXILIARES ───────────────────────────────────────────────

        public async Task<Response<List<StudentDTO>>> GetAllStudentsAsync()
        {
            try
            {
                var list = await _context.Estudiante.ToListAsync();
                return Response<List<StudentDTO>>.Success(
                    _mapper.Map<List<StudentDTO>>(list), "Estudiantes obtenidos");
            }
            catch (Exception ex) { return Response<List<StudentDTO>>.Failure($"Error: {ex.Message}"); }
        }

        public async Task<Response<List<deviceDTO>>> GetAvailableDevicesAsync()
        {
            try
            {
                var list = await _context.Dispositivos
                    .Where(d => d.EstadoEquipo == "Disponible")
                    .ToListAsync();

                return Response<List<deviceDTO>>.Success(
                    _mapper.Map<List<deviceDTO>>(list), "Dispositivos disponibles obtenidos");
            }
            catch (Exception ex) { return Response<List<deviceDTO>>.Failure($"Error: {ex.Message}"); }
        }

        public async Task<Response<List<AdministratorDTO>>> GetAllAdministratorsAsync()
        {
            try
            {
                var list = await _context.Administradores.ToListAsync();
                return Response<List<AdministratorDTO>>.Success(
                    _mapper.Map<List<AdministratorDTO>>(list), "Administradores obtenidos");
            }
            catch (Exception ex) { return Response<List<AdministratorDTO>>.Failure($"Error: {ex.Message}"); }
        }
    }
}
