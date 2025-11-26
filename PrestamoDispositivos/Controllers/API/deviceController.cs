using Microsoft.AspNetCore.Mvc;
using PrestamoDispositivos.Core.Pagination;
using PrestamoDispositivos.Services.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using PrestamoDispositivos.Core;

namespace PrestamoDispositivos.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class deviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public deviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

       
    }
}
