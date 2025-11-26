using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;
using PrestamoDispositivos.Services.Implementations;

namespace PrestamoDispositivos.Controllers
{
    public class DeviceController : Controller
    {
        private readonly IDeviceService _deviceService;
        private readonly INotyfService _notyfService;

        public DeviceController(IDeviceService deviceService, INotyfService notyfService)
        {
            _deviceService = deviceService;
            _notyfService = notyfService;
        }

        // GET: DeviceController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Response<List<deviceDTO>> response = await _deviceService.GetAllDeviceAsync();

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(new List<deviceDTO>());
            }
            
            return View(response.Result ?? new List<deviceDTO>());
        }

        [HttpGet]
        [Authorize(Roles = "DeviceManagerAdmin,DeviceManAdmin")]
        public IActionResult Create()
        {
               return View();
        }

        // POST: DeviceController/Create
        [HttpPost]
        [Authorize(Roles = "DeviceManagerAdmin,DeviceManAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( [FromForm] deviceDTO dto)
        {
            if(!ModelState.IsValid)
            {
                _notyfService.Error("Por favor, corrija los errores en el formulario.");
                return View(dto);
            }

            Response <deviceDTO> response = await _deviceService.CreateDeviceAsync(dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(dto);
            }

            _notyfService.Success("Dispositivo creado exitosamente.");
            return RedirectToAction(nameof(Index));

        }



        [HttpGet]
        [Authorize(Roles = "DeviceManagerAdmin,DeviceManAdmin")]
        // GET: DeviceController/Edit/5
        public async Task <IActionResult> Edit([FromRoute] Guid id)
        {
            Response<deviceDTO> response =  await _deviceService.GetDeviceByIdAsync(id);
            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return RedirectToAction(nameof(Index));
            }
            return View(response.Result);
        }

        // POST: DeviceController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "DeviceManagerAdmin,DeviceManAdmin")]
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromForm] deviceDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("⚠️ Corrige los errores antes de guardar.");
                return View(dto);
            }

            var response = await _deviceService.UpdateDeviceAsync(id, dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message ?? "❌ Error al actualizar el préstamo.");
                return View(dto);
            }

            _notyfService.Success(response.Message ?? "✅ Préstamo actualizado correctamente.");
            return RedirectToAction(nameof(Index));

        }

        

        // POST: DeviceController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "DeviceManagerAdmin,DeviceManAdmin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Por favor, corrija los errores en el formulario.");
                RedirectToAction(nameof(Index));
            }
            Response<bool> response = await _deviceService.DeleteDeviceAsync(id);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);

            }
            else
            {
                    _notyfService.Success("Dispositivo borrado exitosamente.");
            }
                
            return RedirectToAction(nameof(Index));
        }


    }
}
