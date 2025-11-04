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
    public class DeviceService : IDeviceService
    {
        private readonly DatacontextPres _context;
        private readonly IMapper _mapper;

        public DeviceService(DatacontextPres context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Obtener todos los dispositivos
        public async Task<Response<List<deviceDTO>>> GetAllDeviceAsync()
        {
            try
            {
                var devicesDV = await _context.Dispositivos
                    .ToListAsync();

                var deviceDTO = _mapper.Map<List<deviceDTO>>(devicesDV);

                return new Response<List<deviceDTO>>(
                    deviceDTO,
                    "Dispositivos obtenidos correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<List<deviceDTO>>(
                    "Error al obtener lista de dispositivos",
                    new List<string> { ex.Message }
                );
            }
        }

        // Obtener Dispositivo por ID
        public async Task<Response<deviceDTO>> GetDeviceByIdAsync(Guid id)
        {
            try
            {
                var devices = await _context.Dispositivos
                    .Include(x => x.Prestamos)
                    .FirstOrDefaultAsync(x => x.IdDisp == Guid.Parse(id.ToString()));

                if (devices == null)
                    return new Response<deviceDTO>("Dispositivo no encontrado");

                var devicesDto = _mapper.Map<deviceDTO>(devices);

                return new Response<deviceDTO>(
                    devicesDto,
                    "Dispositivo encontrado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<deviceDTO>(
                    "Error al obtener el Dispositivo",
                    new List<string> { ex.Message }
                );
            }
        }

        // Crear nuevo Dispositivo
        public async Task<Response<deviceDTO>> CreateDeviceAsync(deviceDTO devicDto)
        {
            try
            {

                // Verificar si el usuario ya existe
                var existingUser = await _context.Dispositivos
                    .FirstOrDefaultAsync(x => x.IdDisp == devicDto.IdDisp);

                if (existingUser != null)
                    return new Response<deviceDTO>("El Dispositivo ya existe");

                // Mapear DTO a modelo
                var devices = _mapper.Map<Device>(devicDto);
                devices.IdDisp= Guid.NewGuid();

                // Guardar en base de datos
                _context.Dispositivos.Add(devices);
                await _context.SaveChangesAsync();

                // Mapear resultado
                var resultDto = _mapper.Map<deviceDTO>(devices);

                return new Response<deviceDTO>(
                    resultDto,
                    "Dispositivo creado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<deviceDTO>(
                    "Error al crear el Dispositivo",
                    new List<string> { ex.Message }
                );
            }
        }

        // Actualizar Dispositivo
        public async Task<Response<deviceDTO>> UpdateDeviceAsync(Guid id, deviceDTO devicDto)
        {
            try
            {
                var devic = await _context.Dispositivos
                    .Include(x => x.Prestamos)
                    .FirstOrDefaultAsync(x => x.IdDisp == Guid.Parse(id.ToString()));

                if (devic == null)
                    return new Response<deviceDTO>("Dispositivo no encontrado");

                //// Validar datos

                // Verificar si el Dispositivo ya existe (excepto el actual)
                var existingDev = await _context.Dispositivos
                    .FirstOrDefaultAsync(x => x.IdDisp == devic.IdDisp);

                if (existingDev != null)
                    return new Response<deviceDTO>("El Dispositivo ya existe");

                // Actualizar propiedades

                    _mapper.Map(devicDto, devic);

                _context.Dispositivos.Update(devic);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<deviceDTO>(devic);

                return new Response<deviceDTO>(
                    resultDto,
                    "Dispositivo actualizado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new Response<deviceDTO>(
                    "Error al actualizar el Dispositivo",
                    new List<string> { ex.Message }
                );
            }
        }

        // Eliminar Dispositivo
        public async Task<Response<bool>> DeleteDeviceAsync(Guid id)
        {
            try
            {
                var devic = await _context.Dispositivos
                    .Include(x => x.Prestamos)
                    .FirstOrDefaultAsync(x => x.IdDisp== Guid.Parse(id.ToString()));

                if (devic == null)
                    return new Response<bool>("Dispositivo no encontrado");

                // Validar si tiene préstamos asociados
                if (devic.Prestamos != null && devic.Prestamos.Any())
                {
                    return new Response<bool>(
                        "No se puede eliminar el Dispositivo porque tiene préstamos asociados"
                    );
                }

                _context.Dispositivos.Remove(devic);
                await _context.SaveChangesAsync();

                return new Response<bool>(true, "Dispositivo eliminado correctamente");
            }
            catch (Exception ex)
            {
                return new Response<bool>(
                    "Error al eliminar el dispositivo",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}