using Microsoft.AspNetCore.Mvc;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace PrestamoDispositivos.Controllers
{
    //[Authorize]
    //[ApiController]
    //[Route("api/[controller]")]

    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly INotyfService _notyfService;

        public LoanController(ILoanService loanService, INotyfService notyfService)
        {
            _loanService = loanService;
            _notyfService = notyfService;
        }

        // LISTAR todos los préstamos
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _loanService.GetAllLoansAsync();

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message ?? "Error al obtener los préstamos");
                return View(new List<LoanDTO>());
            }

            return View(response.Result);
        }
        private async Task LoadDropdownData()
        {
            var studentsResponse = await _loanService.GetAllStudentsAsync();
            var devicesResponse = await _loanService.GetAvailableDevicesAsync();
            var adminsResponse = await _loanService.GetAllAdministratorsAsync();
            var eventsResponse = await _loanService.GetAllLoanEventsAsync();

            ViewBag.Students = studentsResponse.IsSuccess ? studentsResponse.Result : new List<StudentDTO>();
            ViewBag.Devices = devicesResponse.IsSuccess ? devicesResponse.Result : new List<deviceDTO>();
            ViewBag.Administrators = adminsResponse.IsSuccess ? adminsResponse.Result : new List<deviceManagerDTO>();
            ViewBag.LoanEvents = eventsResponse.IsSuccess ? eventsResponse.Result : new List<LoanEventDTO>();
        }
        // CREAR préstamo (formulario)
      
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Cargar los datos para los dropdowns
            await LoadDropdownData();
            return View();
        }

        // CREAR préstamo (POST)
      
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] LoanDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("⚠️ Corrige los errores del formulario.");
                await LoadDropdownData();
                return View(dto);
            }

            var response = await _loanService.CreateLoanAsync(dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message ?? "❌ Error al crear el préstamo.");
                await LoadDropdownData();
                return View(dto);
            }

            _notyfService.Success(response.Message ?? "✅ Préstamo creado correctamente.");
            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para cargar datos de los dropdowns


        //  EDITAR préstamo (mostrar formulario)
       
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute]Guid id)
        {
            await LoadDropdownData();
            var response = await _loanService.GetLoanByIdAsync(id);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message ?? "❌ Préstamo no encontrado.");
                await LoadDropdownData();
                return RedirectToAction(nameof(Index));
            }

            return View(response.Result);
        }

        // EDITAR préstamo (guardar cambios)
        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromForm] LoanDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Corrige los errores antes de guardar.");
                return View(dto);
            }

            var response = await _loanService.UpdateLoanAsync(id, dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message ?? " Error al actualizar el préstamo.");
                return View(dto);
            }

            _notyfService.Success(response.Message ?? "Préstamo actualizado correctamente.");
            return RedirectToAction(nameof(Index));
        }

        //  ELIMINAR préstamo
    
        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var response = await _loanService.DeleteLoanAsync(id);

            if (!response.IsSuccess)
                _notyfService.Error(response.Message ?? "Error al eliminar el préstamo.");
            else
                _notyfService.Success(response.Message ?? "Préstamo eliminado correctamente.");

            return RedirectToAction(nameof(Index));
        }

        // TOGGLE estado del préstamo (activar / desactivar)
        [HttpPost]
        public async Task<IActionResult> Toggle([FromForm] ToggleLoanStatusDTO dto)
        {
            var response = await _loanService.ToggleLoanStatusAsync(dto);

            if (!response.IsSuccess)
                _notyfService.Error(response.Message ?? "Error al cambiar el estado del préstamo.");
            else
                _notyfService.Success(response.Message ?? "Estado actualizado correctamente.");

            return RedirectToAction(nameof(Index));
        }
    }
}
