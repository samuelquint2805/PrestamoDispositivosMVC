using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Models;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Services.Implementations
{
    /// <summary>
    /// Implementación del servicio de Roles.
    /// Gestiona los 3 roles del sistema: Student, Lender, SuperAdmin.
    /// </summary>
    public class RolService : IRolservice
    {
        private readonly DatacontextPres _context;

        // Nombres de rol del sistema (constantes para evitar typos)
        public const string ROL_STUDENT = "Student";
        public const string ROL_LENDER = "Lender";
        public const string ROL_SUPERADMIN = "SuperAdmin";

        // Dominios / correos que determinan el rol automáticamente
        private static readonly HashSet<string> SuperAdminDomains = new(StringComparer.OrdinalIgnoreCase)
        {
            "@superadmin.com"
        };

        private static readonly HashSet<string> LenderDomains = new(StringComparer.OrdinalIgnoreCase)
        {
            "@prestamista.com",
            "@lender.com"
        };

        private static readonly HashSet<string> ExplicitSuperAdminEmails = new(StringComparer.OrdinalIgnoreCase)
        {
            "superadmin@ejemplo.com",
            "root@ejemplo.com"
        };

        public RolService(DatacontextPres context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────────
        //  CRUD DE ROLES
        // ─────────────────────────────────────────────

        public async Task<Response<List<setRolDTO>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _context.Rol.AsNoTracking().ToListAsync();
                var dtos = roles.Select(MapToDto).ToList();
                return Response<List<setRolDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Response<List<setRolDTO>>.Failure($"Error al obtener roles: {ex.Message}");
            }
        }

        public async Task<Response<setRolDTO>> GetRolByIdAsync(Guid idRol)
        {
            try
            {
                var rol = await _context.Rol.AsNoTracking()
                                              .FirstOrDefaultAsync(r => r.idRol == idRol);
                if (rol == null)
                    return Response<setRolDTO>.Failure("Rol no encontrado.");

                return Response<setRolDTO>.Success(MapToDto(rol));
            }
            catch (Exception ex)
            {
                return Response<setRolDTO>.Failure($"Error al obtener rol: {ex.Message}");
            }
        }

        public async Task<Response<setRolDTO>> GetRolByNameAsync(string nombreRol)
        {
            try
            {
                var rol = await _context.Rol.AsNoTracking()
                                              .FirstOrDefaultAsync(r => r.nombreRol == nombreRol);
                if (rol == null)
                    return Response<setRolDTO>.Failure($"Rol '{nombreRol}' no encontrado.");

                return Response<setRolDTO>.Success(MapToDto(rol));
            }
            catch (Exception ex)
            {
                return Response<setRolDTO>.Failure($"Error al obtener rol por nombre: {ex.Message}");
            }
        }

        public async Task<Response<setRolDTO>> CreateRolAsync(setRolDTO dto)
        {
            try
            {
                var existe = await _context.Rol
                    .AnyAsync(r => r.nombreRol == dto.nombreRol);

                if (existe)
                    return Response<setRolDTO>.Failure($"Ya existe un rol con el nombre '{dto.nombreRol}'.");

                var nuevoRol = new setRol
                {
                    idRol = Guid.NewGuid(),
                    nombreRol = dto.nombreRol,
                    descripcion = dto.descripcion,
                    permisos = dto.permisos
                };

                _context.Rol.Add(nuevoRol);
                await _context.SaveChangesAsync();

                return Response<setRolDTO>.Success(MapToDto(nuevoRol));
            }
            catch (Exception ex)
            {
                return Response<setRolDTO>.Failure($"Error al crear rol: {ex.Message}");
            }
        }

        public async Task<Response<setRolDTO>> UpdateRolAsync(Guid idRol, setRolDTO dto)
        {
            try
            {
                var rol = await _context.Rol.FindAsync(idRol);
                if (rol == null)
                    return Response<setRolDTO>.Failure("Rol no encontrado.");

                // Evitar duplicado de nombre con otro rol distinto
                var duplicado = await _context.Rol
                    .AnyAsync(r => r.nombreRol == dto.nombreRol && r.idRol != idRol);

                if (duplicado)
                    return Response<setRolDTO>.Failure($"Ya existe otro rol con el nombre '{dto.nombreRol}'.");

                rol.nombreRol = dto.nombreRol;
                rol.descripcion = dto.descripcion;
                rol.permisos = dto.permisos;

                _context.Rol.Update(rol);
                await _context.SaveChangesAsync();

                return Response<setRolDTO>.Success(MapToDto(rol));
            }
            catch (Exception ex)
            {
                return Response<setRolDTO>.Failure($"Error al actualizar rol: {ex.Message}");
            }
        }

        public async Task<Response<bool>> DeleteRolAsync(Guid idRol)
        {
            try
            {
                var rol = await _context.Rol.FindAsync(idRol);
                if (rol == null)
                    return Response<bool>.Failure("Rol no encontrado.");

                // Verificar que no haya usuarios usando este rol
                var enUso = await _context.Users.AnyAsync(u => u.RolUser == idRol);
                if (enUso)
                    return Response<bool>.Failure("No se puede eliminar el rol porque está asignado a uno o más usuarios.");

                _context.Rol.Remove(rol);
                await _context.SaveChangesAsync();

                return Response<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure($"Error al eliminar rol: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────
        //  ASIGNACIÓN DE ROLES A USUARIOS
        // ─────────────────────────────────────────────

        public async Task<Response<bool>> AssignRolToUserAsync(Guid idUsuario, Guid idRol)
        {
            try
            {
                var user = await _context.Users.FindAsync(idUsuario);
                if (user == null)
                    return Response<bool>.Failure("Usuario no encontrado.");

                var rol = await _context.Rol.FindAsync(idRol);
                if (rol == null)
                    return Response<bool>.Failure("Rol no encontrado.");

                user.RolUser = idRol;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Response<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure($"Error al asignar rol: {ex.Message}");
            }
        }

        public async Task<Response<bool>> AssignRolToUserByNameAsync(Guid idUsuario, string nombreRol)
        {
            try
            {
                var rol = await _context.Rol
                    .FirstOrDefaultAsync(r => r.nombreRol == nombreRol);

                if (rol == null)
                    return Response<bool>.Failure($"Rol '{nombreRol}' no existe en el sistema.");

                return await AssignRolToUserAsync(idUsuario, rol.idRol);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure($"Error al asignar rol por nombre: {ex.Message}");
            }
        }

        public async Task<Response<bool>> RemoveRolFromUserAsync(Guid idUsuario)
        {
            try
            {
                var user = await _context.Users.FindAsync(idUsuario);
                if (user == null)
                    return Response<bool>.Failure("Usuario no encontrado.");

                user.RolUser = null;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Response<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure($"Error al remover rol: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────
        //  DETERMINACIÓN DE ROL POR EMAIL
        // ─────────────────────────────────────────────

        /// <summary>
        /// Reglas:
        ///  1. Email en lista de SuperAdmin explícitos       → SuperAdmin
        ///  2. Dominio en SuperAdminDomains (@superadmin.com)→ SuperAdmin
        ///  3. Dominio en LenderDomains (@prestamista.com)   → Lender
        ///  4. Resto                                         → Student
        /// </summary>
        public string DetermineRolByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return ROL_STUDENT;

            if (ExplicitSuperAdminEmails.Contains(email))
                return ROL_SUPERADMIN;

            foreach (var domain in SuperAdminDomains)
                if (email.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
                    return ROL_SUPERADMIN;

            foreach (var domain in LenderDomains)
                if (email.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
                    return ROL_LENDER;

            return ROL_STUDENT;
        }

        // ─────────────────────────────────────────────
        //  VERIFICACIÓN DE ROL
        // ─────────────────────────────────────────────

        public async Task<Response<bool>> UserHasRolAsync(Guid idUsuario, string nombreRol)
        {
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.idUsuario == idUsuario);

                if (user == null)
                    return Response<bool>.Failure("Usuario no encontrado.");

                bool tieneRol = user.Rol?.nombreRol?.Equals(nombreRol, StringComparison.OrdinalIgnoreCase) ?? false;
                return Response<bool>.Success(tieneRol);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure($"Error al verificar rol: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────
        //  SEED DE ROLES POR DEFECTO
        // ─────────────────────────────────────────────

        /// <summary>
        /// Inserta los 3 roles base si aún no existen en la BD.
        /// Llamar desde Program.cs o desde un middleware de inicialización.
        /// </summary>
        public async Task SeedDefaultRolesAsync()
        {
            var rolesBase = new[]
            {
                new setRol
                {
                    idRol       = Guid.NewGuid(),
                    nombreRol   = ROL_STUDENT,
                    descripcion = "Estudiante que puede solicitar préstamos de dispositivos.",
                    permisos    = "solicitar_prestamo,ver_prestamos_propios,ver_equipos"
                },
                new setRol
                {
                    idRol       = Guid.NewGuid(),
                    nombreRol   = ROL_LENDER,
                    descripcion = "Prestamista que gestiona y aprueba solicitudes de equipos.",
                    permisos    = "gestionar_prestamos,aprobar_solicitudes,ver_reportes,gestionar_equipos"
                },
                new setRol
                {
                    idRol       = Guid.NewGuid(),
                    nombreRol   = ROL_SUPERADMIN,
                    descripcion = "Administrador con acceso total al sistema.",
                    permisos    = "all"
                }
            };

            foreach (var rol in rolesBase)
            {
                var existe = await _context.Rol.AnyAsync(r => r.nombreRol == rol.nombreRol);
                if (!existe)
                    _context.Rol.Add(rol);
            }

            await _context.SaveChangesAsync();
        }

        // ─────────────────────────────────────────────
        //  HELPER PRIVADO
        // ─────────────────────────────────────────────

        private static setRolDTO MapToDto(setRol rol) => new setRolDTO
        {
            idRol = rol.idRol,
            nombreRol = rol.nombreRol,
            descripcion = rol.descripcion,
            permisos = rol.permisos
        };
    }
}