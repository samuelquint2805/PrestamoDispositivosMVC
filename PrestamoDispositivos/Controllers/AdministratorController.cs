using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly IAdministratorServicie _AdminService;
        private readonly INotyfService _notyfService;

        public AdministratorController(IAdministratorServicie AdminService, INotyfService notyfService)
        {
            _AdminService = AdminService;
            _notyfService = notyfService;
        }

        // GET: DeviceController
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Response<List<AdministratorDTO>> response = await _AdminService.GetAllAdministratorAsync();

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(new List<AdministratorDTO>());
            }

            return View(response.Result ?? new List<AdministratorDTO>());
        }

        [HttpGet]
        [Authorize(Roles = "")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: DeviceController/Create
        [HttpPost]
        [Authorize(Roles = "")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] AdministratorDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Error. Por favor corrija los errores en el formulario.");
                return View(dto);
            }

            Response<AdministratorDTO> response = await _AdminService.CreateAdministratorAsync(dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(dto);
            }

            _notyfService.Success("Administrador creado exitosamente.");
            return RedirectToAction(nameof(Index));

        }



        [HttpGet]
        [Authorize(Roles = "")]
        // GET: DeviceController/Edit/5
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            Response<AdministratorDTO> response = await _AdminService.GetAdministratorByIdAsync(id);
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
        [Authorize(Roles = "")]
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromForm] AdministratorDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("⚠️ Corrige los errores antes de guardar.");
                return View(dto);
            }

            var response = await _AdminService.UpdateAdministratorAsync(id, dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message ?? "❌ Error al actualizar el Administrador.");
                return View(dto);
            }

            _notyfService.Success(response.Message ?? "✅ Administrador actualizado correctamente.");
            return RedirectToAction(nameof(Index));

        }



        // POST: DeviceController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Por favor, corrija los errores en el formulario.");
                RedirectToAction(nameof(Index));
            }
            Response<bool> response = await _AdminService.DeleteAdministratorAsync(id);

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
