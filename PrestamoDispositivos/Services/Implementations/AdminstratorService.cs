using AutoMapper;

using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Models;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Services.Implementations
{
    public class AdminstratorService : IAdministratorServicie
    {
        private readonly DatacontextPres _context;
        private readonly IMapper _mapper;

        public AdminstratorService(DatacontextPres context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Response<AdministratorDTO>> CreateAdministratorAsync(AdministratorDTO Admindto)
        {
            try
            {
                // verificar si el usuario ya existe
                var existinguser = await _context.Administradores
                    .FirstOrDefaultAsync(x => x.IdAdmin == Admindto.IdAdmin);


                if (existinguser != null)
                    return Response<AdministratorDTO>.Failure("el usuario ya existe");

                // mapear dto a modelo
                var manager = _mapper.Map<Administrator>(Admindto);
                manager.IdAdmin = Guid.NewGuid();

                // guardar en base de datos
                _context.Administradores.Add(manager);
                await _context.SaveChangesAsync();

                // mapear resultado
                var resultdto = _mapper.Map<AdministratorDTO>(manager);

                return Response<AdministratorDTO>.Success(
                    resultdto,
                    "administrador creado correctamente"
                );
            }
            catch (Exception ex)
            {
                return Response<AdministratorDTO>.Failure("Error al crear Administrador");
            }

        }

        public async Task<Response<bool>> DeleteAdministratorAsync(Guid id)
        {
            try
            {
                var manager = await _context.Administradores
                    .FirstOrDefaultAsync(x => x.IdAdmin == id);

                if (manager == null)
                    return Response<bool>.Failure("administrador no encontrado");

                // validar si tiene préstamos asociados (futuro requisito)


                _context.Administradores.Remove(manager);
                await _context.SaveChangesAsync();

                return Response<bool>.Success(true, "administrador eliminado correctamente");
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(
                    "error al eliminar el administrador"
                );
            }
        }

        public async Task<Response<List<AdministratorDTO>>> GetAllAdministratorAsync()
        {
            try
            {
                var managers = await _context.Administradores
                    .ToListAsync();

                var managersdto = _mapper.Map<List<AdministratorDTO>>(managers);

                return Response<List<AdministratorDTO>>.Success(
                    managersdto,
                    "administradores obtenidos correctamente"
                );
            }
            catch (Exception ex)
            {
                return Response<List<AdministratorDTO>>.Failure(
                    "error al obtener los administradores"
                );
            }
        }

        public async Task<Response<AdministratorDTO>> GetAdministratorByIdAsync(Guid id)
        {
            try
            {
                var manager = await _context.Administradores
                    .FirstOrDefaultAsync(x => x.IdAdmin == id);

                if (manager == null)
                    return Response<AdministratorDTO>.Failure("administrador no encontrado");

                var managerdto = _mapper.Map<AdministratorDTO>(manager);

                return Response<AdministratorDTO>.Success(
                    managerdto,
                    "administrador encontrado correctamente"
                );
            }
            catch (Exception ex)
            {
                return Response<AdministratorDTO>.Failure(
                    "error al obtener el administrador"
                );
            }
        }

        public async Task<Response<AdministratorDTO>> UpdateAdministratorAsync(Guid id, AdministratorDTO Admindto)
        {
            try
            {
                var manager = await _context.Administradores
                    .FirstOrDefaultAsync(x => x.IdAdmin == id);

                if (manager == null)
                    return Response<AdministratorDTO>.Failure("administrador no encontrado");

                // actualizar propiedades
                manager.Nombre = Admindto.Nombre;
                manager.numeroCelular = Admindto.numeroCelular;

                _context.Administradores.Update(manager);
                await _context.SaveChangesAsync();

                var resultdto = _mapper.Map<AdministratorDTO>(manager);

                return Response<AdministratorDTO>.Success(
                    resultdto,
                    "administrador actualizado correctamente"
                );
            }

            catch (Exception ex)
            {
                return Response<AdministratorDTO>.Failure(
                    "error al actualizar el administrador");
            } 
        }
    }
}
