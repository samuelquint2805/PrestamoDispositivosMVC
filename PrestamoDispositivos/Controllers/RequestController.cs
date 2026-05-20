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
            // 1. Normalizar el estado: si es nulo o "Todos", lo tratamos igual
            string estadoFiltro = (string.IsNullOrEmpty(estado) || estado == "Todos") ? null : estado;

            Response<List<RequestoDTO>> response;

            if (User.IsInRole("SuperAdmin") || User.IsInRole("Lender"))
            {
                ViewBag.IsManager = true;
                // Si hay estado, filtramos; si no, traemos todo
                response = string.IsNullOrEmpty(estadoFiltro)
                    ? await _requestService.GetAllRequestsAsync()
                    : await _requestService.GetRequestsByStatusAsync(estadoFiltro);
            }
            else
            {
                ViewBag.IsManager = false;
                var userId = GetCurrentUserId();
                if (userId == null) return RedirectToAction("Login", "Account");

                // IMPORTANTE: El estudiante también debería poder filtrar sus propias solicitudes
                // Si no tienes un método "GetRequestsByUserAndStatusAsync", 
                // puedes filtrar el resultado en memoria o pedirle al servicio uno nuevo.
                response = string.IsNullOrEmpty(estadoFiltro)
                    ? await _requestService.GetRequestsByUserAsync(userId.Value)
                    : await _requestService.GetRequestsByUserAndStatusAsync(userId.Value, estadoFiltro);

                // Si decides filtrar en memoria (rápido y efectivo):
                if (response.IsSuccess && !string.IsNullOrEmpty(estadoFiltro))
                {
                    response.Result = response.Result
                        .Where(r => r.EstadoSolicitud == estadoFiltro)
                        .ToList();
                }
            }

            // 2. Manejo unificado de errores
            if (!response.IsSuccess)
            {
                _notyf.Error("Error: " + (response.Message ?? "No se pudieron cargar las solicitudes."));
                return View(new List<RequestoDTO>());
            }

            ViewBag.FiltroEstado = estado ?? "Todos";
            return View(response.Result ?? new List<RequestoDTO>());
        }

        // ════════════════════════════════════════════
        //  RESERVAR (Student crea solicitud)
        //  GET: muestra confirmación del dispositivo
        // ════════════════════════════════════════════
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Reserve(Guid idDevice)
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

            if (!response.IsSuccess || response.Result == null)
            {
                _notyf.Error(response.Message ?? "Solicitud no encontrada.");
                return RedirectToAction(nameof(Index));
            }

            // Cargar dispositivos disponibles para el dropdown de aprobación
            var devicesResponse = await _deviceService.GetAllDeviceAsync();
            ViewBag.DisponiblesDevice = devicesResponse.IsSuccess
                ? devicesResponse.Result?.Where(d => d.EstadoEquipo?.ToLower() == "disponible").ToList()
                : new List<deviceDTO>();

            return View(response.Result);
        }

        // ════════════════════════════════════════════
        //  REVIEW POST: Lender aprueba o rechaza
        // ════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Lender")]
        public async Task<IActionResult> Review(
           [FromForm] Guid IdSolicitud,
           [FromForm] string NuevoEstado,
           [FromForm] Guid? IdDispo,
           [FromForm] string? Observacion)
        {
            if (string.IsNullOrEmpty(NuevoEstado))
            {
                _notyf.Error("Debes elegir Aprobar o Rechazar.");
                return RedirectToAction(nameof(Review), new { id = IdSolicitud });
            }

            if (NuevoEstado == "Aprobada" && IdDispo == null)
            {
                _notyf.Error("Debes seleccionar el dispositivo a entregar.");
                return RedirectToAction(nameof(Review), new { id = IdSolicitud });
            }

            var dto = new RequestoDTO
            {
                IdSolicitud = IdSolicitud,
                NuevoEstado = NuevoEstado,
                EstadoSolicitud = NuevoEstado,
                Observacion = Observacion
            };

            // idDispo solo se pasa al servicio si se está aprobando
            Guid? dispositivoId = NuevoEstado == "Aprobada" ? IdDispo : null;

            var response = await _requestService.ReviewRequestAsync(dto, dispositivoId);

            if (!response.IsSuccess)
            {
                _notyf.Error(response.Message ?? "Error al procesar la solicitud.");
                return RedirectToAction(nameof(Review), new { id = IdSolicitud });
            }

            _notyf.Success(response.Message ?? $"Solicitud {NuevoEstado.ToLower()} correctamente.");
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
