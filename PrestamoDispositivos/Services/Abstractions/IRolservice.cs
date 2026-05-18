using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface IRolservice
    {
        
            /// <summary>Obtiene todos los roles disponibles.</summary>
            Task<Response<List<setRolDTO>>> GetAllRolesAsync();

            /// <summary>Obtiene un rol por su ID.</summary>
            Task<Response<setRolDTO>> GetRolByIdAsync(Guid idRol);

            /// <summary>Obtiene un rol por su nombre exacto (ej: "Student", "Lender", "SuperAdmin").</summary>
            Task<Response<setRolDTO>> GetRolByNameAsync(string nombreRol);

            /// <summary>Crea un nuevo rol.</summary>
            Task<Response<setRolDTO>> CreateRolAsync(setRolDTO dto);

            /// <summary>Actualiza un rol existente.</summary>
            Task<Response<setRolDTO>> UpdateRolAsync(Guid idRol, setRolDTO dto);

            /// <summary>Elimina un rol por su ID.</summary>
            Task<Response<bool>> DeleteRolAsync(Guid idRol);

            /// <summary>
            /// Asigna un rol a un usuario. Reemplaza el rol previo.
            /// </summary>
            Task<Response<bool>> AssignRolToUserAsync(Guid idUsuario, Guid idRol);

            /// <summary>
            /// Asigna un rol a un usuario por nombre del rol.
            /// Útil al crear usuarios (Student, Lender, SuperAdmin).
            /// </summary>
            Task<Response<bool>> AssignRolToUserByNameAsync(Guid idUsuario, string nombreRol);

            /// <summary>Remueve el rol de un usuario (deja RolUser en null).</summary>
            Task<Response<bool>> RemoveRolFromUserAsync(Guid idUsuario);

            /// <summary>
            /// Determina el rol según el correo electrónico.
            /// Regla de negocio: @superadmin.com → SuperAdmin; @prestamista.com → Lender; resto → Student.
            /// </summary>
            string DetermineRolByEmail(string email);

            /// <summary>Verifica si un usuario tiene un rol específico por nombre.</summary>
            Task<Response<bool>> UserHasRolAsync(Guid idUsuario, string nombreRol);

            /// <summary>Inicializa los roles base del sistema si no existen (seed).</summary>
            Task SeedDefaultRolesAsync();
        }
    }


