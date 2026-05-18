using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Controllers
{
    public class LenderController : Controller
    {
        private readonly ILenderService _LenderService;
        private readonly INotyfService _notyfService;

        public LenderController(ILenderService LenderService, INotyfService notyfService)
        {
            _LenderService = LenderService;
            _notyfService = notyfService;
        }

        // GET: DeviceController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Response<List<lenderDTO>> response = await _LenderService.GetAllLendersAsync();

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return RedirectToAction("Index", "Home");
            }

            return View(response.Result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] lenderDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Por favor, corrija los errores en el formulario.");
            }

            Response<lenderDTO> response = await _LenderService.CreateLenderAsync(dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(dto);
            }

            _notyfService.Success("Prestamista creado exitosamente.");
            return RedirectToAction(nameof(Index));

        }


        [HttpGet]

        // GET: DeviceController/Edit/5
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            Response<lenderDTO> response = await _LenderService.GetLenderByIdAsync(id);
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
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromForm] lenderDTO dto)
        {
            Response<lenderDTO> response = await _LenderService.UpdateLenderAsync(id, dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(dto);
            }
            _notyfService.Success("Prestamista actualizado exitosamente.");
            return RedirectToAction(nameof(Index));

        }



        // POST: DeviceController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Por favor, corrija los errores en el formulario.");
                RedirectToAction(nameof(Index));
            }
            Response<bool> response = await _LenderService.DeleteLenderAsync(id);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);

            }
            else
            {
                _notyfService.Success("Prestamista borrado exitosamente.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
