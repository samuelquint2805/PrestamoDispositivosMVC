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



        // Crear préstamo
        public async Task<Response<LoanDTO>> CreateLoanAsync(LoanDTO dto)
        {
            try
            {
                Loan loan = _mapper.Map<Loan>(dto);
                await _context.Prestamos.AddAsync(loan);
                await _context.SaveChangesAsync();


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

                loan.IdEstudiante = dto.IdEstudiante;
                loan.IdDispo = dto.IdDispo;
                loan.IdAdminDev = dto.IdAdminDev;
                loan.IdEvento = dto.IdEvento;

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
                //metodo con mapper pero sin incluir busqueda por ID
                // List<Loan> loans = await _context.Prestamos.ToListAsync();
                //List<LoanDTO> dtoList = _mapper.Map<List<LoanDTO>>(loans);

                //metodo de busqueda por ID incluyendo las relaciones usando DTO
                var loan = await _context.Prestamos
                    .Include(x => x.Estudiante)
                    .Include(x => x.Dispositivo)
                    .Include(x => x.DeviceManager)
                    .Include(x => x.EventoPrestamos)
                    .FirstOrDefaultAsync(x => x.IdPrestamos == id);

                if (loan == null)
                    return new Response<LoanDTO>(" Préstamo no encontrado");

              
                    var loanDto = _mapper.Map<LoanDTO>(loan);

                return new Response<LoanDTO>(loanDto, "Préstamo encontrado correctamente");
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
                List<Loan> loans =await _context.Prestamos.ToListAsync();
                List<LoanDTO> dtoList = _mapper.Map<List<LoanDTO>>(loans);

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

                loan.IdEvento = dto.NewStatus;
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
