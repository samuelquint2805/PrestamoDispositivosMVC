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

        public LoanService(DatacontextPres context)
        {
            _context = context;
        }

        // Crear préstamo
        public async Task<Response<LoanDTO>> CreateLoanAsync(LoanDTO dto)
        {
            try
            {
                var loan = new Loan
                {
                    IdPrestamos = Guid.NewGuid(),
                    IdEst = dto.IdEst,
                    IdDispositivo = dto.IdDispositivo,
                    IdAdminDis = dto.IdAdminDis,
                    EstadoPrestamos = dto.EstadoPrestamos
                };

                _context.Prestamos.Add(loan);
                await _context.SaveChangesAsync();

                dto.IdPrestamos = loan.IdPrestamos;

                return new Response<LoanDTO>(dto, " Préstamo creado correctamente");
            }
            catch (Exception ex)
            {
                return new Response<LoanDTO>(" Error al crear el préstamo", new List<string> { ex.Message });
            }
        }

        // Actualizar préstamo
        public async Task<Response<LoanDTO>> UpdateLoanAsync(Guid id, LoanDTO dto)
        {
            try
            {
                var loan = await _context.Prestamos.FirstOrDefaultAsync(x => x.IdPrestamos == id);
                if (loan == null)
                    return new Response<LoanDTO>(" Préstamo no encontrado");

                loan.IdEst = dto.IdEst;
                loan.IdDispositivo = dto.IdDispositivo;
                loan.IdAdminDis = dto.IdAdminDis;
                loan.EstadoPrestamos = dto.EstadoPrestamos;

                _context.Prestamos.Update(loan);
                await _context.SaveChangesAsync();

                return new Response<LoanDTO>(dto, " Préstamo actualizado correctamente");
            }
            catch (Exception ex)
            {
                return new Response<LoanDTO>(" Error al actualizar el préstamo", new List<string> { ex.Message });
            }
        }

        // Eliminar préstamo
        public async Task<Response<bool>> DeleteLoanAsync(Guid id)
        {
            try
            {
                var loan = await _context.Prestamos.FindAsync(id);
                if (loan == null)
                    return new Response<bool>(" Préstamo no encontrado");

                _context.Prestamos.Remove(loan);
                await _context.SaveChangesAsync();

                return new Response<bool>(true, " Préstamo eliminado correctamente");
            }
            catch (Exception ex)
            {
                return new Response<bool>(" Error al eliminar el préstamo", new List<string> { ex.Message });
            }
        }

        // Obtener préstamo por Id
        public async Task<Response<LoanDTO>> GetLoanByIdAsync(Guid id)
        {
            try
            {
                var loan = await _context.Prestamos
                    .Include(x => x.Estudiante)
                    .Include(x => x.Dispositivo)
                    .Include(x => x.AdminDisp)
                    .Include(x => x.EventoPrestamos)
                    .FirstOrDefaultAsync(x => x.IdPrestamos == id);

                if (loan == null)
                    return new Response<LoanDTO>(" Préstamo no encontrado");

                var dto = new LoanDTO
                {
                    IdPrestamos = loan.IdPrestamos,
                    IdEst = loan.IdEst,
                    IdDispositivo = loan.IdDispositivo,
                    IdAdminDis = loan.IdAdminDis,
                    EstadoPrestamos = loan.EstadoPrestamos,
                    Estudiante = loan.Estudiante,
                    Dispositivo = loan.Dispositivo,
                    AdminDisp = loan.AdminDisp,
                    EventoPrestamos = loan.EventoPrestamos
                };

                return new Response<LoanDTO>(dto, "Préstamo encontrado correctamente");
            }
            catch (Exception ex)
            {
                return new Response<LoanDTO>(" Error al obtener el préstamo", new List<string> { ex.Message });
            }
        }

        // Obtener todos los préstamos
        public async Task<Response<List<LoanDTO>>> GetAllLoansAsync()
        {
            try
            {
                var loans = await _context.Prestamos
                    .Include(x => x.Estudiante)
                    .Include(x => x.Dispositivo)
                    .Include(x => x.AdminDisp)
                    .Include(x => x.EventoPrestamos)
                    .ToListAsync();

                var dtoList = loans.Select(x => new LoanDTO
                {
                    IdPrestamos = x.IdPrestamos,
                    IdEst = x.IdEst,
                    IdDispositivo = x.IdDispositivo,
                    IdAdminDis = x.IdAdminDis,
                    EstadoPrestamos = x.EstadoPrestamos,
                    Estudiante = x.Estudiante,
                    Dispositivo = x.Dispositivo,
                    AdminDisp = x.AdminDisp,
                    EventoPrestamos = x.EventoPrestamos
                }).ToList();

                return new Response<List<LoanDTO>>(dtoList, " Lista de préstamos obtenida correctamente");
            }
            catch (Exception ex)
            {
                return new Response<List<LoanDTO>>("Error al obtener los préstamos", new List<string> { ex.Message });
            }
        }

        // Cambiar estado del préstamo
        public async Task<Response<object>> ToggleLoanStatusAsync(ToggleLoanStatusDTO dto)
        {
            try
            {
                var loan = await _context.Prestamos.FindAsync(dto.LoanId);
                if (loan == null)
                    return new Response<object>(" Préstamo no encontrado");

                loan.EstadoPrestamos = dto.NewStatus;
                await _context.SaveChangesAsync();

                return new Response<object>(true, " Estado del préstamo actualizado correctamente");
            }
            catch (Exception ex)
            {
                return new Response<object>("Error al cambiar el estado del préstamo", new List<string> { ex.Message });
            }
        }
    }
}
