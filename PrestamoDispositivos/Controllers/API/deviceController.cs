using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Controllers.API
{
    /// <summary>
    /// API para gestión de dispositivos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Por defecto requiere autenticación
    public class DeviceController : AspiController
    {
        private readonly IDeviceService _deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        /// <summary>
        /// Obtener todos los dispositivos
        /// </summary>
        /// <returns>Lista de dispositivos</returns>
        [HttpGet]
       
        public async Task<IActionResult> GetAllDevices()
        {
            Response<List<deviceDTO>> response = await _deviceService.GetAllDeviceAsync();
            return AspiController.ControllerBasicValidation(response);
        }

        /// <summary>
        /// Obtener dispositivo por ID
        /// </summary>
        /// <param name="id">ID del dispositivo</param>
        /// <returns>Dispositivo encontrado</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOneDevice(Guid id)
        {
            Response<deviceDTO> response = await _deviceService.GetDeviceByIdAsync(id);
            return AspiController.ControllerBasicValidation(response);
        }

        /// <summary>
        /// Crear nuevo dispositivo (solo administradores)
        /// </summary>
        /// <param name="deviceDto">Datos del dispositivo</param>
        /// <returns>Dispositivo creado</returns>
        [HttpPost]
        [Authorize(Roles = "DeviceManAdmin")]
       
        public async Task<IActionResult> CreateDevice([FromBody] deviceDTO deviceDto)
        {
            Response<deviceDTO> response = await _deviceService.CreateDeviceAsync(deviceDto);
            return AspiController.ControllerBasicValidation(response, ModelState);
        }

       
        /// <param name="id">ID del dispositivo</param>
        /// <param name="deviceDto">Datos actualizados</param>
        /// <returns>Dispositivo actualizado</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "DeviceManAdmin")]
        public async Task<IActionResult> UpdateDevice(Guid id, [FromBody] deviceDTO deviceDto)
        {
            Response<deviceDTO> response = await _deviceService.UpdateDeviceAsync(id, deviceDto);
            return AspiController.ControllerBasicValidation(response, ModelState);
        }

        /// <summary>
        /// Eliminar dispositivo (solo administradores)
        /// </summary>
        /// <param name="id">ID del dispositivo</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "DeviceManAdmin")]
        public async Task<IActionResult> DeleteDevice(Guid id)
        {
            Response<bool> response = await _deviceService.DeleteDeviceAsync(id);
            return AspiController.ControllerBasicValidation(response);
        }

        /// <summary>
        /// Obtener dispositivos disponibles
        /// </summary>
        /// <returns>Lista de dispositivos con estado "Disponible"</returns>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableDevices()
        {
            var response = await _deviceService.GetAllDeviceAsync();

            if (!response.IsSuccess)
                return AspiController.ControllerBasicValidation(response);

            var availableDevices = response.Result
                .Where(d => d.EstadoDisp == "Nuevo")
                .ToList();

            var filteredResponse = PrestamoDispositivos.Core.Response<List<deviceDTO>>.Success(
                availableDevices,
                $"Se encontraron {availableDevices.Count} dispositivos disponibles"
            );

            return AspiController.ControllerBasicValidation(filteredResponse);
        }

        /// <summary>
        /// Obtener estadísticas de dispositivos (solo administradores)
        /// </summary>
        /// <returns>Estadísticas agrupadas por estado</returns>
        [HttpGet("statistics")]
        [Authorize(Roles = "DeviceManAdmin")]
        public async Task<IActionResult> GetStatistics()
        {
            Response<List<deviceDTO>> response = await _deviceService.GetAllDeviceAsync();

            if (!response.IsSuccess)
                return AspiController.ControllerBasicValidation(response);

            var statistics = response.Result
                .GroupBy(d => d.EstadoDisp)
                .Select(g => new
                {
                    estado = g.Key,
                    cantidad = g.Count()
                })
                .ToList();

            var statsResponse = PrestamoDispositivos.Core.Response<object>.Success(
                new
                {
                    total = response.Result.Count,
                    porEstado = statistics
                },
                "Estadísticas obtenidas correctamente"
            );

            return AspiController.ControllerBasicValidation(statsResponse);
        }
    }
}