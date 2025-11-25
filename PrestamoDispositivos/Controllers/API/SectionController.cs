using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PrestamoDispositivos.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
       
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "Section API is working!" });
        }

        [HttpPost]
        public IActionResult Post([FromBody] object data)
        {
            return Ok(new { message = "Data received", receivedData = data });
        }
    }
}
