using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Models;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Services.Implementations
{
    public class lenderService : ILenderService
    {

        private readonly DatacontextPres _context;
        private readonly IMapper _mapper;

        public lenderService(DatacontextPres context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Response<lenderDTO>> CreateLenderAsync(lenderDTO dto)
        {
            try { 
            // Verificar si el usuario ya existe
            var existingUser = await _context.Prestamista
                .FirstOrDefaultAsync(x => x.idPres == dto.idPres);

            if (existingUser != null)
                return Response<lenderDTO>.Failure("El Prestamista ya existe");
                // Mapear DTO a modelo

                dto.idPres = Guid.NewGuid();
                var lenderUs = _mapper.Map<lender>(dto);


                // Guardar en base de datos
                _context.Prestamista.Add(lenderUs);
                await _context.SaveChangesAsync();

                // Mapear resultado
                var resultDto = _mapper.Map<lenderDTO>(lenderUs);

                return Response<lenderDTO>.Success(
                    resultDto,
                    "Prestamista creado correctamente"
                );
            }
            catch (Exception)
            {
                return Response<lenderDTO>.Failure(
                    "Error al crear el Prestamista"
                );
            }
        }

        public async Task<Response<bool>> DeleteLenderAsync(Guid id)
        {
            try
            {
                var LenderDt = await _context.Prestamista
                   .FirstOrDefaultAsync(x => x.idPres == id);

                if (LenderDt == null)
                    return Response<bool>.Failure("Prestamista no encontrado");

                // Validar si tiene préstamos asociados
                if (LenderDt.User.PrestamosUser != null && LenderDt.User.PrestamosUser.Any())
                {
                    return Response<bool>.Failure(
                        "No se puede eliminar el Prestamista porque tiene préstamos asociados"
                    );
                }

                _context.Prestamista.Remove(LenderDt);
                await _context.SaveChangesAsync();

                return Response<bool>.Success(true, "Prestamista eliminado correctamente");
            }
            catch (Exception)
            {
                return Response<bool>.Failure(
                    "Error al eliminar el Prestamista"
                );
            }
        }

        public async Task<Response<List<lenderDTO>>> GetAllLendersAsync()
        {
            try
            {

                var lenderDV = await _context.Prestamista
            .ToListAsync();

                var lenderDTO = _mapper.Map<List<lenderDTO>>(lenderDV);

                return Response<List<lenderDTO>>.Success(lenderDTO, "Lista de Prestamista obtenida correctamente");
            }
            catch (Exception)
            {
                return Response<List<lenderDTO>>.Failure(
                    "Error al obtener la lista de Prestamista"
                );
            }
        }

        public async Task<Response<lenderDTO>> GetLenderByIdAsync(Guid id)
        {
            try
            {
                var lenderGT = await _context.Prestamista
                    .FirstOrDefaultAsync(x => x.idPres == id);

                if (lenderGT == null)
                    return Response<lenderDTO>.Failure("Prestamista no encontrado");

                var LenderDTO = _mapper.Map<lenderDTO>(lenderGT);

                return Response<lenderDTO>.Success(LenderDTO, "Prestamista obtenido correctamente");
            }
            catch (Exception)
            {
                return Response<lenderDTO>.Failure(
                    "Error al obtener el Prestamista"
                );
            }
        }

        public async Task<Response<lenderDTO>> UpdateLenderAsync(Guid id, lenderDTO LenderDT)
        {
            try
            {
                var LenderUP = await _context.Prestamista
                    .FirstOrDefaultAsync(x => x.idPres == id);

                if (LenderUP == null)
                    return Response<lenderDTO>.Failure("Prestamista no encontrado");


                // Actualizar propiedades

                _mapper.Map(LenderUP, LenderDT );

                _context.Prestamista.Update(LenderUP);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<lenderDTO>(LenderUP);

                return Response<lenderDTO>.Success(
                    resultDto,
                    "Prestamista actualizado correctamente"
                );
            }
            catch (Exception)
            {
                return Response<lenderDTO>.Failure(
                    "Error al actualizar el Prestamista"
                );
            }
        }
    }
}
