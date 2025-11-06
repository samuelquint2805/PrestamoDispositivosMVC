using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _StudentService;
        private readonly INotyfService _notyfService;

        public StudentController(IStudentService StudentService, INotyfService notyfService)
        {
            _StudentService = StudentService;
            _notyfService = notyfService;
        }

        // GET: DeviceController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Response<List<StudentDTO>> response = await _StudentService.GetAllStudentsAsync(); 

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
        public async Task<IActionResult> Create([FromForm] StudentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Por favor, corrija los errores en el formulario.");
            }

            Response<StudentDTO> response = await _StudentService.CreateStudentAsync(dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(dto);
            }

            _notyfService.Success("Estudiante creado exitosamente.");
            return RedirectToAction(nameof(Index));

        }


        [HttpGet]

        // GET: DeviceController/Edit/5
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            Response<StudentDTO> response = await _StudentService.GetStudentByIdAsync(id);
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
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromForm] StudentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Por favor, corrija los errores en el formulario.");
                return View(dto);
            }
            Response<StudentDTO> response = await _StudentService.UpdateStudentAsync(id, dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(dto);
            }
            _notyfService.Success("Estudiante actualizado exitosamente.");
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
            Response<bool> response = await _StudentService.DeleteStudentAsync(id);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);

            }
            else
            {
                _notyfService.Success("Estudiante borrado exitosamente.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
