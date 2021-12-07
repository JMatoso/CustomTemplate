using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CustomTemplate.Controllers
{
    /// <summary>
    /// Error handler.
    /// </summary>
    [ApiController]
    [Route("api/error")]
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// Error.
        /// </summary>
        [HttpGet("/error")]
        public ActionResult Error() => Problem();

        /// <summary>
        /// Development error.
        /// </summary>
        [HttpPost("/error-local-development")]
        public ActionResult ErrorLocalDevelopment([FromServices]IWebHostEnvironment webHostEnvironment)
        {
            if(webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments."
                );
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            return Problem(
                detail: context?.Error.StackTrace,
                title: context?.Error.Message
            );
        }
    }
}