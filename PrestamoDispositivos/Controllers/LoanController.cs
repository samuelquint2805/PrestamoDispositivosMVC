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

        /// <summary>
        /// Muestra préstamos según el rol del usuario
        /// - Admin: Ve TODOS los préstamos
        /// - Estudiante: Ve SOLO sus propios préstamos
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                Response<List<LoanDTO>> response;

                // Verificar el rol del usuario actual
                if (User.IsInRole(""))
                {
                    // ADMIN: Mostrar TODOS los préstamos
                    response = await _loanService.GetAllLoansAsync();
                    ViewBag.IsAdmin = true;
                    ViewBag.ViewTitle = "Todos los Préstamos del Sistema";
                }
                else if (User.IsInRole("Estudiante"))
                {
                    // ESTUDIANTE: Mostrar SOLO sus préstamos
                    var userId = GetCurrentUserId();

                    if (userId == null)
                    {
                        _notyfService.Error("❌ No se pudo identificar tu usuario.");
                        return RedirectToAction("Login", "Account");
                    }

                    // Buscar el estudiante asociado a este usuario
                    var student = await _context.Estudiante
                        .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

                    if (student == null)
                    {
                        _notyfService.Warning("⚠️ No tienes un perfil de estudiante asociado.");
                        return View("NoProfile");
                    }

                    response = await _loanService.GetAllLoansPerStudentAsync(student.IdEst);
                    ViewBag.IsAdmin = false;
                    ViewBag.ViewTitle = "Mis Préstamos";
                    ViewBag.StudentName = student.Nombre;
                }
                else
                {
                    _notyfService.Error("❌ No tienes permisos para ver préstamos.");
                    return RedirectToAction("Index", "Home");
                }

                if (!response.IsSuccess || response.Result == null)
                {
                    _notyfService.Error(response.Message ?? "Error al obtener préstamos");
                    return View(new List<LoanDTO>()); // ← Lista vacía, NO null
                }

                // Mensaje informativo
                if (response.Result.Count == 0)
                {
                    if (User.IsInRole("Estudiante"))
                        _notyfService.Information("📋 No tienes préstamos registrados aún.");
                    else
                        _notyfService.Information("📋 No hay préstamos registrados en el sistema.");
                }

                return View(response.Result);
            }
            catch (Exception ex)
            {
                _notyfService.Error($"❌ Error inesperado: {ex.Message}");
                return View(new List<LoanDTO>());
            }
        }

        /// <summary>
        /// Obtiene el ID del usuario actualmente autenticado
        /// </summary>
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }

            return null;
        }

        // CREAR préstamo (formulario) - ESTUDIANTES Y ADMIN
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownData();

            // Si es estudiante, pre-seleccionar su propio ID
            if (User.IsInRole("Estudiante"))
            {
                var userId = GetCurrentUserId();
                if (userId != null)
                {
                    var student = await _context.Estudiante
                        .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

                    if (student != null)
                    {
                        ViewBag.CurrentStudentId = student.IdEst;
                        ViewBag.IsStudentCreating = true;
                    }
                }
            }

            return View();
        }

        // CREAR préstamo (POST) - ESTUDIANTES Y ADMIN
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] LoanDTO dto)
        {
            // Si es estudiante, forzar que el préstamo sea para sí mismo
            if (User.IsInRole("Estudiante"))
            {
                var userId = GetCurrentUserId();
                if (userId != null)
                {
                    var student = await _context.Estudiante
                        .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

                    if (student != null)
                    {
                        // Forzar que el préstamo sea para este estudiante
                        dto.User.studentUsuario.IdEst = student.IdEst;
                    }
                    else
                    {
                        _notyfService.Error("❌ No tienes un perfil de estudiante asociado.");
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

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

        // EDITAR préstamo (formulario) - ESTUDIANTES (solo los suyos) Y ADMIN (todos)
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            var response = await _loanService.GetLoanByIdAsync(id);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message ?? "❌ Préstamo no encontrado.");
                return RedirectToAction(nameof(Index));
            }

            // Si es estudiante, verificar que sea dueño del préstamo
            if (User.IsInRole("Estudiante"))
            {
                var userId = GetCurrentUserId();
                var student = await _context.Estudiante
                    .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

                if (student == null || response.Result.User.studentUsuario.IdEst != student.IdEst)
                {
                    _notyfService.Error("❌ No tienes permiso para editar este préstamo.");
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.IsStudentEditing = true;
            }

            await LoadDropdownData();
            return View(response.Result);
        }

        // EDITAR préstamo (POST) - ESTUDIANTES (solo los suyos) Y ADMIN (todos)
        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromForm] LoanDTO dto)
        {
            // Si es estudiante, verificar permisos
            if (User.IsInRole("Estudiante"))
            {
                var userId = GetCurrentUserId();
                var student = await _context.Estudiante
                    .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

                if (student == null || dto.User.studentUsuario.IdEst != student.IdEst)
                {
                    _notyfService.Error("❌ No tienes permiso para editar este préstamo.");
                    return RedirectToAction(nameof(Index));
                }

                // Forzar que el estudiante no pueda cambiar a quién pertenece el préstamo
                dto.User.studentUsuario.IdEst = student.IdEst;
            }

            if (!ModelState.IsValid)
            {
                _notyfService.Error("⚠️ Corrige los errores antes de guardar.");
                await LoadDropdownData();
                return View(dto);
            }

            var response = await _loanService.UpdateLoanAsync(id, dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message ?? "❌ Error al actualizar el préstamo.");
                await LoadDropdownData();
                return View(dto);
            }

            _notyfService.Success(response.Message ?? "✅ Préstamo actualizado correctamente.");
            return RedirectToAction(nameof(Index));
        }

        // ELIMINAR préstamo - SOLO ADMIN
        [Authorize(Roles = "DeviceManAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _loanService.DeleteLoanAsync(id);

            if (!response.IsSuccess)
                _notyfService.Error(response.Message ?? "❌ Error al eliminar el préstamo.");
            else
                _notyfService.Success(response.Message ?? "✅ Préstamo eliminado correctamente.");

            return RedirectToAction(nameof(Index));
        }

        // DEVOLVER DISPOSITIVO - ADMIN o el ESTUDIANTE dueño
        [HttpPost]
        public async Task<IActionResult> ReturnDevice([FromRoute] Guid id)
        {
            // Verificar permisos
            if (!User.IsInRole("DeviceManAdmin"))
            {
                // Si es estudiante, verificar que sea dueño del préstamo
                var loan = await _context.Prestamos
                    .Include(l => l.User.studentUsuario)
                    .FirstOrDefaultAsync(l => l.IdPrestamos == id);

                if (loan != null)
                {
                        _notyfService.Error("❌ No tienes permiso para devolver este dispositivo.");
                        return RedirectToAction(nameof(Index));
                }
            }

            var response = await _loanService.ReturnDeviceAsync(id);

            if (!response.IsSuccess)
                _notyfService.Error(response.Message ?? "❌ Error al devolver el dispositivo.");
            else
                _notyfService.Success(response.Message ?? "✅ Dispositivo devuelto correctamente.");

            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para cargar datos de dropdowns
        private async Task LoadDropdownData()
        {
            var studentsResponse = await _loanService.GetAllStudentsAsync();
            var devicesResponse = await _loanService.GetAvailableDevicesAsync();
            var adminsResponse = await _loanService.GetAllAdministratorsAsync();
           

            ViewBag.Students = studentsResponse.IsSuccess ? studentsResponse.Result : new List<StudentDTO>();
            ViewBag.Devices = devicesResponse.IsSuccess ? devicesResponse.Result : new List<deviceDTO>();
        }
    }
}