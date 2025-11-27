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
               
                return  Response<List<deviceDTO>>.Success(
                    deviceDTO,
                    "Dispositivos obtenidos correctamente"
                );
            }
            catch (Exception)
            {
                return  Response<List<deviceDTO>>.Failure(
                    "Error al obtener lista de dispositivos"
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
                    return  Response<deviceDTO>.Failure("Dispositivo no encontrado");

                var devicesDto = _mapper.Map<deviceDTO>(devices);

                return  Response<deviceDTO>.Success(
                    devicesDto,
                    "Dispositivo encontrado correctamente"
                );
            }
            catch (Exception)
            {
                return  Response<deviceDTO>.Failure(
                    "Error al obtener el Dispositivo"
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
                    return  Response<deviceDTO>.Failure("El Dispositivo ya existe");

                // Mapear DTO a modelo
                var devices = _mapper.Map<Device>(devicDto);
                devices.IdDisp= Guid.NewGuid();

                // Guardar en base de datos
                _context.Dispositivos.Add(devices);
                await _context.SaveChangesAsync();

                // Mapear resultado
                var resultDto = _mapper.Map<deviceDTO>(devices);

                return  Response<deviceDTO>.Success(
                    resultDto,
                    "Dispositivo creado correctamente"
                );
            }
            catch (Exception)
            {
                return  Response<deviceDTO>.Failure(
                    "Error al crear el Dispositivo"
                );
            }
        }

        // Actualizar Dispositivo
        public async Task<Response<deviceDTO>> UpdateDeviceAsync(Guid id, deviceDTO devicDto)
        {
            try
            {
                var devic = await _context.Dispositivos
                    
                    .FirstOrDefaultAsync(x => x.IdDisp == Guid.Parse(id.ToString()));

                if (devic == null)
                    return  Response<deviceDTO>.Failure("Dispositivo no encontrado");

                

                // Actualizar propiedades

                    _mapper.Map(devicDto, devic);

                _context.Dispositivos.Update(devic);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<deviceDTO>(devic);

                return  Response<deviceDTO>.Success(
                    resultDto,
                    "Dispositivo actualizado correctamente"
                );
            }
            catch (Exception)
            {
                return  Response<deviceDTO>.Failure(
                    "Error al actualizar el Dispositivo"
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
                    return  Response<bool>.Failure("Dispositivo no encontrado");

                // Validar si tiene préstamos asociados
                if (devic.Prestamos != null && devic.Prestamos.Any())
                {
                    return  Response<bool>.Failure(
                        "No se puede eliminar el Dispositivo porque tiene préstamos asociados"
                    );
                }

                _context.Dispositivos.Remove(devic);
                await _context.SaveChangesAsync();

                return  Response<bool>.Success(true, "Dispositivo eliminado correctamente");
            }
            catch (Exception)
            {
                return  Response<bool>.Failure(
                    "Error al eliminar el dispositivo"
                );
            }
        }
    }
}