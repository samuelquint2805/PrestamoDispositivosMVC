using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Controllers
{
    public class LoanEventController : Controller
    {
        private readonly ILoanEventService _LoanEventeService;
        private readonly INotyfService _notyfService;

        public LoanEventController(ILoanEventService LoanEventService, INotyfService notyfService)
        {
            _LoanEventeService = LoanEventService;
            _notyfService = notyfService;
        }

        // GET: DeviceController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Response<List<LoanEventDTO>> response = await _LoanEventeService.GetAllLoanEventoAsync();

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


        // GET: DeviceController/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] LoanEventDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Por favor, corrija los errores en el formulario.");
            }

            Response<LoanEventDTO> response = await _LoanEventeService.CreateLoanEventoAsync(dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(dto);
            }

            _notyfService.Success("Evento de Prestamo creado exitosamente.");
            return RedirectToAction(nameof(Index));

        }


        [HttpGet]

        // GET: DeviceController/Edit/5
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            Response<LoanEventDTO> response = await _LoanEventeService.GetLoanEventoByIdAsync(id);
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
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromForm] LoanEventDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Por favor, corrija los errores en el formulario.");
                return View(dto);
            }
            Response<LoanEventDTO> response = await _LoanEventeService.UpdateLoanEventoAsync(id, dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(dto);
            }
            _notyfService.Success("Evento de Prestamo actualizado exitosamente.");
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
            Response<bool> response = await _LoanEventeService.DeleteLoanEventoAsync(id);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);

            }
            else
            {
                _notyfService.Success("Evento de Prestamo borrado exitosamente.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
