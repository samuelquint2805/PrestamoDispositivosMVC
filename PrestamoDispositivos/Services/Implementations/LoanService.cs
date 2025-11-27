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


                // Validar que el dispositivo exista y esté disponible
                var device = await _context.Dispositivos.FindAsync(dto.IdDispo);


                // Crear el préstamo
                dto.IdPrestamos = Guid.NewGuid();
                Loan loan = _mapper.Map<Loan>(dto);
                await _context.Prestamos.AddAsync(loan);

                // Actualizar el estado del dispositivo a "Prestado"
                device.EstadoDisp = "Prestado";
                _context.Dispositivos.Update(device);

                // Guardar cambios
                await _context.SaveChangesAsync();

                return Response<LoanDTO>.Success(dto, "Préstamo creado correctamente");
            }
            catch (Exception ex)
            {
                return Response<LoanDTO>.Failure($"Error al crear el préstamo: {ex.Message}");
            }
        }

        // Actualizar préstamo
        public async Task<Response<LoanDTO>> UpdateLoanAsync(Guid id, LoanDTO dto)
        {
            try
            {
                var loan = await _context.Prestamos.FirstOrDefaultAsync(x => x.IdPrestamos == id);
                if (loan == null)
                    return Response<LoanDTO>.Failure(" Préstamo no encontrado");

                loan.IdEstudiante = dto.IdEstudiante;
                loan.IdDispo = dto.IdDispo;
                loan.IdAdminDev = dto.IdAdminDev;
                loan.IdEvento = dto.IdEvento;

                _context.Prestamos.Update(loan);
                await _context.SaveChangesAsync();

                return Response<LoanDTO>.Success(dto, " Préstamo actualizado correctamente");
            }
            catch (Exception )
            {
                return Response<LoanDTO>.Failure(" Error al actualizar el préstamo");
            }
        }

        // Eliminar préstamo
        public async Task<Response<bool>> DeleteLoanAsync(Guid id)
        {
            try
            {
                var loan = await _context.Prestamos.FindAsync(id);
                if (loan == null)
                    return Response<bool>.Failure(" Préstamo no encontrado");

                _context.Prestamos.Remove(loan);
                await _context.SaveChangesAsync();

                return Response<bool>.Success(true, " Préstamo eliminado correctamente");
            }
            catch (Exception)
            {
                return Response<bool>.Failure(" Error al eliminar el préstamo");
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
                    return Response<LoanDTO>.Failure(" Préstamo no encontrado");


                var loanDto = _mapper.Map<LoanDTO>(loan);

                return Response<LoanDTO>.Success(loanDto, "Préstamo encontrado correctamente");
            }
            catch (Exception)
            {
                return Response<LoanDTO>.Failure(" Error al obtener el préstamo");
            }
        }

        // Obtener todos los préstamos
        public async Task<Response<List<LoanDTO>>> GetAllLoansAsync()
        {
            try
            {
                List<Loan> loans = await _context.Prestamos
                    .Include(l => l.Estudiante)
                    .Include(l => l.Dispositivo)
                    .Include(l => l.DeviceManager)
            .ToListAsync();
                List<LoanDTO> dtoList = _mapper.Map<List<LoanDTO>>(loans);

                return Response<List<LoanDTO>>.Success(dtoList, " Lista de préstamos obtenida correctamente");
            }
            catch (Exception)
            {
                return Response<List<LoanDTO>>.Failure("Error al obtener los préstamos");
            }
        }

        // Cambiar estado del préstamo
        public async Task<Response<object>> ToggleLoanStatusAsync(ToggleLoanStatusDTO dto)
        {
            try
            {
                var loan = await _context.Prestamos.FindAsync(dto.LoanId);
                if (loan == null)
                    return Response<object>.Failure(" Préstamo no encontrado");

                loan.IdEvento = dto.NewStatus;
                await _context.SaveChangesAsync();

                return Response<object>.Success(true, " Estado del préstamo actualizado correctamente");
            }
            catch (Exception)
            {
                return Response<object>.Failure("Error al cambiar el estado del préstamo");
            }
        }
        // Obtener todos los estudiantes
        public async Task<Response<List<StudentDTO>>> GetAllStudentsAsync()
        {
            try
            {
                var targetStatusId = Guid.Parse("1EAA1209-075C-4E29-91C9-33824518AD93");
                var students = await _context.Estudiante
                    .Where(s => s.EstadoEstId == targetStatusId) // Solo estudiantes activos
                    .OrderBy(s => s.Nombre)
                    .ToListAsync();

                var dtoList = _mapper.Map<List<StudentDTO>>(students);
                return Response<List<StudentDTO>>.Success(dtoList, "Estudiantes obtenidos correctamente");
            }
            catch (Exception )
            {
                return Response<List<StudentDTO>>.Failure("Error al obtener estudiantes");
            }
        }

        // Obtener dispositivos disponibles
        public async Task<Response<List<deviceDTO>>> GetAvailableDevicesAsync()
        {
            try
            {
                var devices = await _context.Dispositivos
                    .Where(d => d.EstadoDisp == "Nuevo")
                    .OrderBy(d => d.Tipo)
                    .ToListAsync();

                var dtoList = _mapper.Map<List<deviceDTO>>(devices);
                return Response<List<deviceDTO>>.Success(dtoList, "Dispositivos obtenidos correctamente");
            }
            catch (Exception )
            {
                return Response<List<deviceDTO>>.Failure("Error al obtener dispositivos");
            }
        }

        // Obtener todos los administradores
        public async Task<Response<List<deviceManagerDTO>>> GetAllAdministratorsAsync()
        {
            try
            {
                var admins = await _context.AdminDisp
                    .OrderBy(a => a.Nombre)
                    .ToListAsync();

                var dtoList = _mapper.Map<List<deviceManagerDTO>>(admins);
                return Response<List<deviceManagerDTO>>.Success(dtoList, "Administradores obtenidos correctamente");
            }
            catch (Exception)
            {
                return Response<List<deviceManagerDTO>>.Failure("Error al obtener administradores");
            }
        }

        // Obtener todos los eventos de préstamo
        public async Task<Response<List<LoanEventDTO>>> GetAllLoanEventsAsync()
        {
            try
            {
                var events = await _context.EventoPrestamos
                    .OrderBy(e => e.TipoPrestamos)
                    .ToListAsync();

                var dtoList = _mapper.Map<List<LoanEventDTO>>(events);
                return Response<List<LoanEventDTO>>.Success(dtoList, "Eventos obtenidos correctamente");
            }
            catch (Exception )
            {
                return Response<List<LoanEventDTO>>.Failure("Error al obtener eventos");
            }
        }
    }
}
