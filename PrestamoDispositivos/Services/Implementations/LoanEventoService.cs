using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.Data;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Models;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Services.Implementations
{
    public class LoanEventoService : ILoanEventService
    {
        private readonly DatacontextPres _context;
        private readonly IMapper _mapper;

        public LoanEventoService(DatacontextPres context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Obtener todos los administradores de dispositivos
        public async Task<Response<List<LoanEventDTO>>> GetAllLoanEventoAsync()
        {
            try
            {
                var LoanEventoGT = await _context.EventoPrestamos
                    .ToListAsync();

                var LoanEventoGTDTO = _mapper.Map<List<LoanEventDTO>>(LoanEventoGT);

                return  Response<List<LoanEventDTO>>.Success(
                    LoanEventoGTDTO,
                    "Eventos del prestamo obtenidos correctamente"
                );
            }
            catch (Exception)
            {
                return  Response<List<LoanEventDTO>>.Failure(
                    "Error al obtener lista de eventos del prestamos"
                );
            }
        }

        // Obtener administrador por ID
        public async Task<Response<LoanEventDTO>> GetLoanEventoByIdAsync(Guid id)
        {
            try
            {
                var LoanEventGT = await _context.EventoPrestamos
                    .Include(x => x.EventosPrestamos)
                    .FirstOrDefaultAsync(x => x.IdEvento == Guid.Parse(id.ToString()));

                if (LoanEventGT == null)
                    return  Response<LoanEventDTO>.Failure("el evento del prestamo no ha sido encontrado, probablemente no existe");

                var LoanEventoGTDTO = _mapper.Map<LoanEventDTO>(LoanEventGT);

                return  Response<LoanEventDTO>.Success(
                    LoanEventoGTDTO,
                    "evento del prestamo encontrado correctamente"
                );
            }
            catch (Exception)
            {
                return  Response<LoanEventDTO>.Failure(
                    "Error al obtener el evento del prestamo "
                );
            }
        }

        // Crear nuevo administrador
        public async Task<Response<LoanEventDTO>> CreateLoanEventoAsync(LoanEventDTO LoanEvenDto)
        {
            try
            {

                // Verificar si el usuario ya existe
                var existingLoanEvent= await _context.EventoPrestamos

                    .FirstOrDefaultAsync(x => x.IdEvento== LoanEvenDto.IdEvento);

                if (existingLoanEvent != null)
                    return  Response<LoanEventDTO>.Failure("El Evento del prestamo ya existe");

                // Mapear DTO a modelo
                var LoanEvent= _mapper.Map<LoanEvent>(LoanEvenDto);
                LoanEvenDto.IdEvento= Guid.NewGuid();

                // Guardar en base de datos
                _context.EventoPrestamos.Add(LoanEvent);
                await _context.SaveChangesAsync();

                // Mapear resultado
                var resultDto = _mapper.Map<LoanEventDTO>(LoanEvenDto);

                return  Response<LoanEventDTO>.Success(
                    resultDto,
                    "Evento del prestamo creado correctamente"
                );
            }
            catch (Exception)
            {
                return  Response<LoanEventDTO>.Failure(
                    "Error al crear evento del prestamo"
                );
            }
        }

        // Actualizar administrador
        public async Task<Response<LoanEventDTO>> UpdateLoanEventoAsync(Guid id, LoanEventDTO LoanEvenDtoo)
        {
            try
            {
                var LoanEventUP = await _context.EventoPrestamos
                    .Include(x => x.EventosPrestamos)
                    .FirstOrDefaultAsync(x => x.IdEvento== Guid.Parse(id.ToString()));

                if (LoanEventUP == null)
                    return  Response<LoanEventDTO>.Failure("Evento del prestamo no encontrado");


                // Actualizar propiedades

                _mapper.Map(LoanEvenDtoo, LoanEventUP);

                _context.EventoPrestamos.Update(LoanEventUP);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<LoanEventDTO>(LoanEventUP);

                return  Response<LoanEventDTO>.Success(
                    resultDto,
                    "El evento del prestamo ha sido actualizado correctamente"
                );
            }
            catch (Exception)
            {
                return  Response<LoanEventDTO>.Failure(
                    "Error al actualizar el evento del prestamo"
                );
            }
        }

        // Eliminar Dispositivo
        public async Task<Response<bool>> DeleteLoanEventoAsync(Guid id)
        {
            try
            {
                
                var LoanEventDL = await _context.EventoPrestamos
                    .Include(x => x.EventosPrestamos)
                    .FirstOrDefaultAsync(x => x.IdEvento== Guid.Parse(id.ToString()));

                if (LoanEventDL== null)
                    return  Response<bool>.Failure("Evento del prestamo no encontrado");

                // Validar si tiene préstamos asociados
                if (LoanEventDL.EventosPrestamos != null && LoanEventDL.EventosPrestamos.Any())
                {
                    return  Response<bool>.Failure(
                        "No se puede eliminar el evento del prestamo porque tiene préstamos asociados"
                    );
                }

                _context.EventoPrestamos.Remove(LoanEventDL);
                await _context.SaveChangesAsync();

                return  Response<bool>.Success(true, "El evento del prestamo ha sido eliminado correctamente");
            }
            catch (Exception)
            {
                return  Response<bool>.Failure(
                    "Error al eliminar el evento del prestamo"
                );
            }
        }
    }
}