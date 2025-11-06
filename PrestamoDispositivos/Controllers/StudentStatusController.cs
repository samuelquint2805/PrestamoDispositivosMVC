using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Controllers
{
    public class StudentStatusController : Controller
    {
        private readonly IStudentStatusService _StudentStatusService;
        private readonly INotyfService _notyfService;

        public StudentStatusController(IStudentStatusService StudentStatusService, INotyfService notyfService)
        {
            _StudentStatusService = StudentStatusService;
            _notyfService = notyfService;
        }

        // GET: DeviceController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Response<List<studentStatusDTO>> response = await _StudentStatusService.GetAllStudentStaAsync();

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
        public async Task<IActionResult> Create([FromForm] studentStatusDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Por favor, corrija los errores en el formulario.");
            }

            Response<studentStatusDTO> response = await _StudentStatusService.CreateStudentStaAsync(dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(dto);
            }

            _notyfService.Success("Estado del Estudiante creado exitosamente.");
            return RedirectToAction(nameof(Index));

        }


        [HttpGet]

        // GET: DeviceController/Edit/5
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            Response<studentStatusDTO> response = await _StudentStatusService.GetStudentStaByIdAsync(id);
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
        public async Task<IActionResult> Edit([FromRoute]Guid id, [FromForm] studentStatusDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Por favor, corrija los errores en el formulario.");
                return View(dto);
            }
            Response<studentStatusDTO> response = await _StudentStatusService.UpdateStudentStaAsync(id, dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(dto);
            }
            _notyfService.Success("Estado del estudiante actualizado exitosamente.");
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
            Response<bool> response = await _StudentStatusService.DeleteStudentStaAsync(id);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);

            }
            else
            {
                _notyfService.Success("Estado del estudiante borrado exitosamente.");
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
