using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PrestamoDispositivos.Core;

namespace PrestamoDispositivos.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AspiController : ControllerBase
    {
        public static ObjectResult controllerBasecValidation <T>(Response<T> response, ModelStateDictionary? modelState = null, int? statuscode = null)
        {
            if (!modelState.IsValid && modelState is not null)
            {
                response.IsSuccess = false;
                response.Message = "Validation errors occurred.";
                response.Errors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return new ObjectResult(Response<T>.Failure("Debe Ajustar los errores de validación"));
            }
            return null; // No validation errors
        }
    }
}
