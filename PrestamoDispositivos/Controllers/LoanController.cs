using Microsoft.AspNetCore.Mvc;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.DataContext.Sections;

namespace PrestamoDispositivos.Controllers
{
    [Authorize] // Requiere autenticación
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly INotyfService _notyfService;
        private readonly DatacontextPres _context;

        public LoanController(
            ILoanService loanService,
            INotyfService notyfService,
            DatacontextPres context)
        {
            _loanService = loanService;
            _notyfService = notyfService;
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Lender,Student")]
        public async Task<IActionResult> Index()
        {
            try
            {
                Response<List<LoanDTO>> response;

                if (User.IsInRole("SuperAdmin") || User.IsInRole("Lender"))
                {
                    response = await _loanService.GetAllLoansAsync();
                    ViewBag.IsAdmin = true;
                }
                else
                {
                    // Student: préstamos ligados a su ApplicationUser
                    var userId = GetCurrentUserId();
                    if (userId == null)
                    {
                        _notyfService.Error("No se pudo identificar tu usuario.");
                        return RedirectToAction("Login", "Account");
                    }

                    response = await _loanService.GetLoansByUserAsync(userId.Value);
                    ViewBag.IsAdmin = false;
                }

                if (!response.IsSuccess || response.Result == null)
                {
                    _notyfService.Error(response.Message ?? "Error al obtener préstamos.");
                    return View(new List<LoanDTO>());
                }

                return View(response.Result);
            }
            catch (Exception ex)
            {
                _notyfService.Error($"Error inesperado: {ex.Message}");
                return View(new List<LoanDTO>());
            }
        }

        // ════════════════════════════════════════════
        //  EDIT GET — solo Admin/Lender
        // ════════════════════════════════════════════
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Lender")]
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            var response = await _loanService.GetLoanByIdAsync(id);
            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message ?? "Préstamo no encontrado.");
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdownData();
            return View(response.Result);
        }

        // ════════════════════════════════════════════
        //  EDIT POST — solo Admin/Lender
        // ════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Lender")]
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromForm] LoanDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Corrige los errores antes de guardar.");
                await LoadDropdownData();
                return View(dto);
            }

            var response = await _loanService.UpdateLoanAsync(id, dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message ?? "Error al actualizar el préstamo.");
                await LoadDropdownData();
                return View(dto);
            }

            _notyfService.Success(response.Message ?? "Préstamo actualizado correctamente.");
            return RedirectToAction(nameof(Index));
        }

        // ════════════════════════════════════════════
        //  DELETE POST — solo SuperAdmin/Lender
        // ════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Lender")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _loanService.DeleteLoanAsync(id);

            if (!response.IsSuccess)
                _notyfService.Error(response.Message ?? "Error al eliminar el préstamo.");
            else
                _notyfService.Success(response.Message ?? "Préstamo eliminado correctamente.");

            return RedirectToAction(nameof(Index));
        }

        // ════════════════════════════════════════════
        //  RETURN DEVICE POST — Admin/Lender
        // ════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Lender")]
        public async Task<IActionResult> ReturnDevice([FromRoute] Guid id)
        {
            var response = await _loanService.ReturnDeviceAsync(id);

            if (!response.IsSuccess)
                _notyfService.Error(response.Message ?? "Error al devolver el dispositivo.");
            else
                _notyfService.Success(response.Message ?? "Dispositivo devuelto correctamente.");

            return RedirectToAction(nameof(Index));
        }

        // ── HELPER ───────────────────────────────────────────────────
        private Guid? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && Guid.TryParse(claim.Value, out var guid) ? guid : null;
        }

        private async Task LoadDropdownData()
        {
            var devicesResponse = await _loanService.GetAvailableDevicesAsync();
            ViewBag.Devices = devicesResponse.IsSuccess ? devicesResponse.Result : new List<deviceDTO>();
        }
    }
}
