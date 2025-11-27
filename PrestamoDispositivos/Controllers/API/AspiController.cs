using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PrestamoDispositivos.Core;

namespace PrestamoDispositivos.Controllers.API
{
    [AllowAnonymous]
    [Authorize(Roles = "DeviceManAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AspiController : ControllerBase
    {
        public static ObjectResult ControllerBasicValidation<T>(Response<T> response, ModelStateDictionary? modelState = null, int? statusCode = null)
        {
            if (modelState is not null && !modelState.IsValid)
            {
                List<string> errors = modelState.Values.SelectMany(v => v.Errors)
                                                       .Select(e => e.ErrorMessage)
                                                       .ToList();

                return new ObjectResult(Response<T>.Failure("Debe ajustar los errores de validación", errors))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            if (statusCode is not null && statusCode >= 100 && statusCode <= 599)
            {
                return new ObjectResult(response)
                {
                    StatusCode = statusCode
                };
            }

            if (response.IsSuccess)
            {
                return new ObjectResult(response)
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }

            return new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}
