using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Services.Implementations
{
    public class AppUser : IAppUser
    {
        private readonly DatacontextPres _context;
        private readonly IMapper _mapper;

        public AppUser(DatacontextPres context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response<List<ApplicationUserDTO>>> GetAllUserUsAsync()
        {
            try
            {

                var UserDV = await _context.Users
            .ToListAsync();

                var UsertDTO = _mapper.Map<List<ApplicationUserDTO>>(UserDV);

                return Response<List<ApplicationUserDTO>>.Success(UsertDTO, "Lista de usuarios obtenida correctamente");
            }
            catch (Exception)
            {
                return Response<List<ApplicationUserDTO>>.Failure(
                    "Error al obtener la lista de usuarios "
                );
            }
        }

        public async Task<Response<ApplicationUserDTO>> GetuserByIdAsync(Guid id)
        {
            try
            {
                var UserGT = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == Guid.Parse(id.ToString()));

                if (UserGT == null)
                    return Response<ApplicationUserDTO>.Failure("Usuario no encontrado");

                var userDto = _mapper.Map<ApplicationUserDTO>(UserGT);

                return Response<ApplicationUserDTO>.Success("Usuario no encontrado");
            }
            catch (Exception)
            {
                return Response<ApplicationUserDTO>.Failure(
                    "Error al obtener el Usuario"
                );
            }
        }

        public async Task<Response<ApplicationUserDTO>> UpdateUserUsAsync(Guid id, ApplicationUserDTO userdto)
        {
            try
            {
                var UserUP = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == Guid.Parse(id.ToString()));

                if (UserUP == null)
                    return Response<ApplicationUserDTO>.Failure("Usuario no encontrado");


                // Actualizar propiedades

                _mapper.Map(userdto, UserUP);

                _context.Users.Update(UserUP);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<ApplicationUserDTO>(UserUP);

                return Response<ApplicationUserDTO>.Success(
                    resultDto,
                    "Usuario actualizado correctamente"
                );
            }
            catch (Exception)
            {
                return Response<ApplicationUserDTO>.Failure(
                    "Error al actualizar el Usuario"
                );
            }
        }
        public async Task<Response<bool>> DeleteSUserUsAsync(Guid id)
        {
            try
            {
                // 1. Buscar el ApplicationUser (Usuario de Identity)
                // Usamos el Id directamente, ya que el parámetro 'id' y User.Id son ambos Guid.
                // También incluimos el rol para la lógica, aunque no es estrictamente necesario aquí.
                var appUser = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == id); // <--- CORRECCIÓN CLAVE: Buscar por GUID directamente

                if (appUser == null)
                    return Response<bool>.Failure("Usuario de Identity no encontrado");

                // 2. Intentar encontrar y eliminar el perfil de Estudiante
                // La FK es ApplicationUserId, que coincide con el ID del ApplicationUser
                var studentProfile = await _context.Estudiante
                    .Include(s => s.Prestamos)
                    .FirstOrDefaultAsync(s => s.ApplicationUserId == id);

                if (studentProfile != null)
                {
                    // A. Validación: Estudiante no debe tener préstamos asociados
                    if (studentProfile.Prestamos != null && studentProfile.Prestamos.Any())
                    {
                        return Response<bool>.Failure(
                            "No se puede eliminar el usuario porque tiene préstamos asociados"
                        );
                    }

                    // B. Eliminar el perfil de Estudiante
                    _context.Estudiante.Remove(studentProfile);
                }

                // 3. Intentar encontrar y eliminar el perfil de Administrador (deviceManager)
                var adminProfile = await _context.AdminDisp
                    .FirstOrDefaultAsync(a => a.ApplicationUserId == id);

                if (adminProfile != null)
                {
                    // C. Eliminar el perfil de Administrador
                    _context.AdminDisp.Remove(adminProfile);

                    // NOTA: Si DeviceManager puede tener Loans (DeviceManager?.Loans.Any()), 
                    // la validación de préstamos debe ir aquí también.
                }

                // 4. Eliminar el ApplicationUser de la tabla principal
                _context.Users.Remove(appUser);

                // 5. Guardar todos los cambios
                await _context.SaveChangesAsync();

                return Response<bool>.Success(true, "Usuario y perfiles asociados eliminados correctamente.");
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(
                    "Error al eliminar el Usuario: " + ex.Message
                );
            }
        }
    }
}
