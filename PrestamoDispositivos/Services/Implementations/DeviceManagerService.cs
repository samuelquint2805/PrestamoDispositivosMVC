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
    public class DeviceManagerService : IDeviceManagerService
    {
        private readonly DatacontextPres _context;
        private readonly IMapper _mapper;

        public DeviceManagerService(DatacontextPres context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Obtener todos los administradores de dispositivos
        public async Task<Response<List<deviceManagerDTO>>> GetAllDeviceManagerAsync()
        {
            try
            {
                var managers = await _context.AdminDisp
                    .Include(x => x.Loans)
                    .ToListAsync();

                var managersDto = _mapper.Map<List<deviceManagerDTO>>(managers);

                return new Response<List<deviceManagerDTO>>(
                    managersDto,
                    "Administradores obtenidos correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<List<deviceManagerDTO>>(
                    "Error al obtener los administradores",
                    new List<string> { ex.Message }
                );
            }
        }

        // Obtener administrador por ID
        public async Task<Response<deviceManagerDTO>> GetDeviceManagerByIdAsync(Guid id)
        {
            try
            {
                var manager = await _context.AdminDisp
                    .Include(x => x.Loans)
                    .FirstOrDefaultAsync(x => x.IdAdmin == Guid.Parse(id.ToString()));

                if (manager == null)
                    return new Response<deviceManagerDTO>("Administrador no encontrado");

                var managerDto = _mapper.Map<deviceManagerDTO>(manager);

                return new Response<deviceManagerDTO>(
                    managerDto,
                    "Administrador encontrado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<deviceManagerDTO>(
                    "Error al obtener el administrador",
                    new List<string> { ex.Message }
                );
            }
        }

        // Crear nuevo administrador
        public async Task<Response<deviceManagerDTO>> CreateDeviceManagerAsync(deviceManagerDTO devicManDto)
        {
            try
            {
                

                // Verificar si el usuario ya existe
                var existingUser = await _context.AdminDisp
                    .FirstOrDefaultAsync(x => x.IdAdmin == devicManDto.IdAdmin);

                if (existingUser != null)
                    return new Response<deviceManagerDTO>("El usuario ya existe");

                // Mapear DTO a modelo
                var manager = _mapper.Map<deviceManager>(devicManDto);
                manager.IdAdmin = Guid.NewGuid();

                // Guardar en base de datos
                _context.AdminDisp.Add(manager);
                await _context.SaveChangesAsync();

                // Mapear resultado
                var resultDto = _mapper.Map<deviceManagerDTO>(manager);

                return new Response<deviceManagerDTO>(
                    resultDto,
                    "Administrador creado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<deviceManagerDTO>(
                    "Error al crear el administrador",
                    new List<string> { ex.Message }
                );
            }
        }

        // Actualizar administrador
        public async Task<Response<deviceManagerDTO>> UpdateDeviceManagerAsync(Guid id, deviceManagerDTO devicManDto)
        {
            try
            {
                var manager = await _context.AdminDisp
                    .FirstOrDefaultAsync(x => x.IdAdmin == Guid.Parse(id.ToString()));

                if (manager == null)
                    return new Response<deviceManagerDTO>("Administrador no encontrado");

                // Verificar si el usuario ya existe (excepto el actual)
                var existingUser = await _context.AdminDisp
                    .FirstOrDefaultAsync(x => x.Usuario == devicManDto.Usuario && x.IdAdmin != manager.IdAdmin);

                if (existingUser != null)
                    return new Response<deviceManagerDTO>("El usuario ya existe");

                // Actualizar propiedades
                manager.Nombre = devicManDto.Nombre;
                manager.Usuario = devicManDto.Usuario;

                // Solo actualizar contraseña si se proporciona una nueva
                if (!string.IsNullOrWhiteSpace(devicManDto.Contraseña))
                    manager.Contraseña = devicManDto.Contraseña;

                _context.AdminDisp.Update(manager);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<deviceManagerDTO>(manager);

                return new Response<deviceManagerDTO>(
                    resultDto,
                    "Administrador actualizado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<deviceManagerDTO>(
                    "Error al actualizar el administrador",
                    new List<string> { ex.Message }
                );
            }
        }

        // Eliminar administrador
        public async Task<Response<bool>> DeleteDeviceManagerAsync(Guid id)
        {
            try
            {
                var manager = await _context.AdminDisp
                    .Include(x => x.Loans)
                    .FirstOrDefaultAsync(x => x.IdAdmin == Guid.Parse(id.ToString()));

                if (manager == null)
                    return new Response<bool>("Administrador no encontrado");

                // Validar si tiene préstamos asociados
                if (manager.Loans != null && manager.Loans.Any())
                {
                    return new Response<bool>(
                        "No se puede eliminar el administrador porque tiene préstamos asociados"
                    );
                }

                _context.AdminDisp.Remove(manager);
                await _context.SaveChangesAsync();

                return new Response<bool>(true, "Administrador eliminado correctamente");
            }
            catch (Exception ex)
            {
                return new Response<bool>(
                    "Error al eliminar el administrador",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}