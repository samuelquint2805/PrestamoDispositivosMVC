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

                return new Response<List<LoanEventDTO>>(
                    LoanEventoGTDTO,
                    "Eventos del prestamo obtenidos correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<List<LoanEventDTO>>(
                    "Error al obtener lista de eventos del prestamos",
                    new List<string> { ex.Message }
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
                    return new Response<LoanEventDTO>("el evento del prestamo no ha sido encontrado, probablemente no existe");

                var LoanEventoGTDTO = _mapper.Map<LoanEventDTO>(LoanEventGT);

                return new Response<LoanEventDTO>(
                    LoanEventoGTDTO,
                    "evento del prestamo encontrado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<LoanEventDTO>(
                    "Error al obtener el evento del prestamo ",
                    new List<string> { ex.Message }
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
                    return new Response<LoanEventDTO>("El Evento del prestamo ya existe");

                // Mapear DTO a modelo
                var LoanEvent= _mapper.Map<LoanEvent>(LoanEvenDto);
                LoanEvenDto.IdEvento= Guid.NewGuid();

                // Guardar en base de datos
                _context.EventoPrestamos.Add(LoanEvent);
                await _context.SaveChangesAsync();

                // Mapear resultado
                var resultDto = _mapper.Map<LoanEventDTO>(LoanEvenDto);

                return new Response<LoanEventDTO>(
                    resultDto,
                    "Evento del prestamo creado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<LoanEventDTO>(
                    "Error al crear evento del prestamo",
                    new List<string> { ex.Message }
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
                    return new Response<LoanEventDTO>("Evento del prestamo no encontrado");

                

                // Verificar si el Dispositivo ya existe (excepto el actual)
                var existinLoanEv= await _context.EventoPrestamos
                    .FirstOrDefaultAsync(x => x.IdEvento== LoanEventUP.IdEvento);

                if (existinLoanEv != null)
                    return new Response<LoanEventDTO>("El evento del prestamo ya existe");

                // Actualizar propiedades

                _mapper.Map(LoanEvenDtoo, LoanEventUP);

                _context.EventoPrestamos.Update(LoanEventUP);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<LoanEventDTO>(LoanEventUP);

                return new Response<LoanEventDTO>(
                    resultDto,
                    "El evento del prestamo ha sido actualizado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<LoanEventDTO>(
                    "Error al actualizar el evento del prestamo",
                    new List<string> { ex.Message }
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
                    return new Response<bool>("Evento del prestamo no encontrado");

                // Validar si tiene préstamos asociados
                if (LoanEventDL.EventosPrestamos != null && LoanEventDL.EventosPrestamos.Any())
                {
                    return new Response<bool>(
                        "No se puede eliminar el evento del prestamo porque tiene préstamos asociados"
                    );
                }

                _context.EventoPrestamos.Remove(LoanEventDL);
                await _context.SaveChangesAsync();

                return new Response<bool>(true, "El evento del prestamo ha sido eliminado correctamente");
            }
            catch (Exception ex)
            {
                return new Response<bool>(
                    "Error al eliminar el evento del prestamo",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}