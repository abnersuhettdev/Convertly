using Microsoft.AspNetCore.Mvc;

namespace Convertly.Api.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Ok(new
        {
            success = true,
            data = new
            {
                status = "Healthy",
                service = "Convertly.Api"
            },
            message = "Healthy",
            errors = Array.Empty<string>()
        });
    }
}
