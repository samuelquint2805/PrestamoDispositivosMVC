using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;
using System.Security.Claims;

namespace PrestamoDispositivos.Controllers
{
    public class deviceManagerController : Controller
    {
        private readonly IDeviceManagerService _deviceManagerService;
        private readonly INotyfService _notyfService;

        public deviceManagerController(IDeviceManagerService deviceManagerService, INotyfService notyfService)
        {
            _deviceManagerService = deviceManagerService;
            _notyfService = notyfService;
        }
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _notyfService.Success("Sesión cerrada.");
            return RedirectToAction("Login");
        }

        // =====================
        // 🔹 REGISTRO
        // =====================
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] deviceManagerDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Corrige los errores del formulario.");
                return View(dto);
            }

            // Aquí llamas tu servicio que guarda el admin
            //dto.Rol = "DeviceManAdmin"; // Rol por defecto
            var response = await _deviceManagerService.CreateDeviceManagerAsync(dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(dto);
            }

            _notyfService.Success("Administrador registrado correctamente.");
            return RedirectToAction("Login");
        }

        // =====================
        // 🔹 ACCESS DENIED
        // =====================
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        // GET: DeviceController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Response<List<deviceManagerDTO>> response = await _deviceManagerService.GetAllDeviceManagerAsync();

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
        public async Task<IActionResult> Create([FromForm] deviceManagerDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Por favor, corrija los errores en el formulario.");
            }

            Response<deviceManagerDTO> response = await _deviceManagerService.CreateDeviceManagerAsync(dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(dto);
            }

            _notyfService.Success("Administrador creado exitosamente.");
            return RedirectToAction(nameof(Index));

        }


        [HttpGet]

        // GET: DeviceController/Edit/5
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            Response<deviceManagerDTO> response = await _deviceManagerService.GetDeviceManagerByIdAsync(id);
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
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromForm] deviceManagerDTO dto)
        {
           
            Response <deviceManagerDTO> response = await _deviceManagerService.UpdateDeviceManagerAsync(id, dto);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);
                return View(dto);
            }
            _notyfService.Success("Administrador actualizado exitosamente.");
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
            Response<bool> response = await _deviceManagerService.DeleteDeviceManagerAsync(id);

            if (!response.IsSuccess)
            {
                _notyfService.Error(response.Message);

            }
            else
            {
                _notyfService.Success("Administrador borrado exitosamente.");
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
