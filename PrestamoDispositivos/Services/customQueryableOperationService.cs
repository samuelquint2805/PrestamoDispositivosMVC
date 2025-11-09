using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Services
{
    public class customQueryableOperationService
    {

        private readonly DatacontextPres _context;
        private readonly IMapper _mapper;

        public customQueryableOperationService(DatacontextPres context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response<TDTO>> CreateGenericAsync< TEntity, TDTO>( TDTO tDto) where TEntity : iID
        {
            try
            {
                TEntity entity = _mapper.Map <TEntity>(tDto);

                Guid id = Guid.NewGuid();

                entity.ID = id;

                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();

                //entity.ID = id;
                return new Response<TDTO>(
                    tDto,
                    "Registro creado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<TDTO>(
                    "Error al crear el registro",
                    new List<string> { ex.Message }
                );
            }
        }
        public async Task<Response<TDTO>> UpdateGenericAsync<TEntity, TDTO>(Guid id, TDTO tDto) where TEntity : iID
        {
            try
            {
                TEntity entity = _mapper.Map<TEntity>(tDto);
                // Actualizar propiedades
                entity.ID = id;

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TDTO resultDto = _mapper.Map<TDTO>(entity);

                return new Response<TDTO>(
                    resultDto,
                    "Registro actualizado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<TDTO>(
                    "Error al actualizar el Registro",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<Response<object>> DeleteGenericAsync <TEntity>(Guid id) where TEntity : class, iID
        {
            try
            {
                TEntity? entity = await _context.Set<TEntity>()
                    .FirstOrDefaultAsync(e => e.ID == id);

                if(entity == null)
                    {
                    return new Response<object>(
                        "Error al eliminar el registro",
                        new List<string> { "Registro no encontrado" }
                    );
                }
                _context.Remove(entity);
                await _context.SaveChangesAsync();
                return new Response<object>(
                    null,
                    "Registro eliminado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<object>(
                    "Error al eliminar el registro",
                    new List<string> { ex.Message }
                );

            }
        }
        public async Task<Response<TDTO>> getOneGenericAsync<TEntity, TDTO>(Guid id) where TEntity : class, iID
        {
            try
            {
                TEntity? entity = await _context.Set<TEntity>()
                   .FirstOrDefaultAsync(e => e.ID == id);

                if (entity == null)
                {
                    return new Response<TDTO>(
                        "Error al obtener el registro o no existe",
                        new List<string> { "Registro no encontrado" }
                    );
                }

                TDTO resultDto = _mapper.Map<TDTO>(entity);
                return new Response<TDTO>(
                    resultDto,
                    "Registro obtenido correctamente"
                );
            }
            catch (Exception ex)
            {

               return new Response<TDTO>(
                    "Error al obtener el registro",
                    new List<string> { ex.Message }
                );
            }
        }
        public async Task<Response<TDTO>> getListGenericAsync<TEntity, TDTO>(IQueryable<TEntity> query = null) where TEntity : class, iID
        {
            try
            {
                if( query is null)
                {
                    query = _context.Set<TEntity>();
                }

                List<TEntity> entity = await query.ToListAsync();

                if (entity == null)
                {
                    return new Response<TDTO>(
                        "Error al obtener el obtener lista o no existe",
                        new List<string> { "Registro no encontrado" }
                    );
                }

                TDTO resultDto = _mapper.Map<TDTO>(entity);
                return new Response<TDTO>(
                    resultDto,
                    "Registro obtenido correctamente"
                );
            }
            catch (Exception ex)
            {

                return new Response<TDTO>(
                     "Error al obtener el registro",
                     new List<string> { ex.Message }
                 );
            }
        }
    }
}
