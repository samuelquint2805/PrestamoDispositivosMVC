using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DataContext.Sections;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;
using PrestamoDispositivos.Services.Implementations;
using System.Security.Claims;

namespace PrestamoDispositivos.Controllers
{
    
    public class RequestController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly IDeviceService _deviceService;
        private readonly INotyfService _notyf;
        private readonly DatacontextPres _context;

        public RequestController(IRequestService requestService, IDeviceService deviceService, INotyfService notyf, DatacontextPres context)
        {
            _requestService = requestService;
            _deviceService = deviceService;
            _notyf = notyf;
            _context = context;
        }

        // ════════════════════════════════════════════
        //  INDEX
        //  SuperAdmin / Lender → todas las solicitudes
        //  Student             → solo las suyas
        // ════════════════════════════════════════════
        [HttpGet]

        [Authorize(Roles = "SuperAdmin,Lender,Student")]
        public async Task<IActionResult> Index(string? estado = null)
        {
            Response<List<RequestoDTO>> response;

            if (User.IsInRole("SuperAdmin") || User.IsInRole("Lender"))
            {

                response = string.IsNullOrEmpty(estado)
                ? await _requestService.GetAllRequestsAsync()
                    : await _requestService.GetRequestsByStatusAsync(estado);

                ViewBag.IsManager = true;
            }
            else
            {
                var userId = GetCurrentUserId();
                if (userId == null) return RedirectToAction("Login", "Account");

                response = await _requestService.GetRequestsByUserAsync(userId.Value);
                ViewBag.IsManager = false;
            }

            ViewBag.FiltroEstado = estado ?? "Todos";

            if (!response.IsSuccess)
            {
                _notyf.Error(response.Message ?? "Error al cargar solicitudes.");
                return View(new List<RequestoDTO>());
            }

            return View(response.Result ?? new List<RequestoDTO>());
        }

        // ════════════════════════════════════════════
        //  RESERVAR (Student crea solicitud)
        //  GET: muestra confirmación del dispositivo
        // ════════════════════════════════════════════
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Reserve([FromForm]Guid idDevice)
        {
            var deviceResponse = await _deviceService.GetDeviceByIdAsync(idDevice);

            if (!deviceResponse.IsSuccess)
            {
                _notyf.Error("Dispositivo no encontrado.");
                return RedirectToAction("Index", "Device");
            }

            if (deviceResponse.Result?.EstadoEquipo?.ToLower() != "disponible")
            {
                _notyf.Warning("Este dispositivo no está disponible para reserva.");
                return RedirectToAction("Index", "Device");
            }

            ViewBag.Device = deviceResponse.Result;
            return View();
        }

        // ════════════════════════════════════════════
        //  RESERVAR POST: Student confirma la reserva
        // ════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Reserve(Guid idDevice, string confirm)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var dto = new RequestoDTO
            {
                idUser = userId.Value
            };
            

            var response = await _requestService.CreateRequestAsync(dto, idDevice);

            if (!response.IsSuccess)
            {
                _notyf.Error(response.Message ?? "No se pudo crear la solicitud.");
                return RedirectToAction("Index", "Device");
            }

            _notyf.Success("¡Solicitud enviada! El prestamista la revisará pronto.");
            return RedirectToAction(nameof(Index));
        }

        // ════════════════════════════════════════════
        //  REVIEW GET: Lender / SuperAdmin ve detalle
        // ════════════════════════════════════════════
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Lender")]
        public async Task<IActionResult> Review(Guid id)
        {
            var response = await _requestService.GetRequestByIdAsync(id);

            if (!response.IsSuccess)
            {
                _notyf.Error(response.Message ?? "Solicitud no encontrada.");
                return RedirectToAction(nameof(Index));
            }

            return View(response.Result);
        }

        // ════════════════════════════════════════════
        //  REVIEW POST: Lender aprueba o rechaza
        // ════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Lender")]
        public async Task<IActionResult> Review(RequestoDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Datos inválidos.");
                return RedirectToAction(nameof(Index));
            }

            var response = await _requestService.ReviewRequestAsync(dto);

            if (!response.IsSuccess)
            {
                _notyf.Error(response.Message ?? "Error al procesar la solicitud.");
                return RedirectToAction(nameof(Review), new { id = dto.IdSolicitud });
            }

            string msg = dto.EstadoSolicitud == "Aprobada"
                ? "Solicitud aprobada correctamente."
                : "Solicitud rechazada.";

            _notyf.Success(msg);
            return RedirectToAction(nameof(Index));
        }

        // ════════════════════════════════════════════
        //  CANCEL POST: Student cancela su solicitud
        // ════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var response = await _requestService.CancelRequestAsync(id, userId.Value);

            if (!response.IsSuccess)
                _notyf.Error(response.Message ?? "No se pudo cancelar la solicitud.");
            else
                _notyf.Success("Solicitud cancelada.");

            return RedirectToAction(nameof(Index));
        }

        // ════════════════════════════════════════════
        //  DELETE POST: solo SuperAdmin
        // ════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _requestService.DeleteRequestAsync(id);

            if (!response.IsSuccess)
                _notyf.Error(response.Message ?? "Error al eliminar.");
            else
                _notyf.Success("Solicitud eliminada.");

            return RedirectToAction(nameof(Index));
        }

        // ════════════════════════════════════════════
        //  HELPERS
        // ════════════════════════════════════════════
        private Guid? GetCurrentUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(idStr, out var guid) ? guid : null;
        }
    }
}
