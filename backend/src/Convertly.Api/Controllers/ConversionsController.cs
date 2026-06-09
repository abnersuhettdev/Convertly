using Convertly.Application.Common;
using Convertly.Application.Conversions;
using Convertly.Application.Conversions.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Convertly.Api.Controllers;

[ApiController]
[Authorize]
[Route("/conversions")]
public sealed class ConversionsController(IConversionService conversionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ConversionListResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var response = await conversionService.GetConversionsAsync(page, pageSize, status, cancellationToken);

        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ConversionDetailResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await conversionService.GetConversionAsync(id, cancellationToken);

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var response = await conversionService.DownloadConversionAsync(id, cancellationToken);

        if (!response.Success || response.Data is null)
        {
            if (response.Message == "Conversion not found")
            {
                return NotFound(response);
            }

            return BadRequest(response);
        }

        return File(response.Data.File, response.Data.ContentType, response.Data.FileName);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<CreateConversionResponse>>> Create(
        [FromForm] CreateConversionFormRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File is null)
        {
            var missingFileResponse = ApiResponse<CreateConversionResponse>.Fail(
                "Validation failed",
                "File is required");

            return BadRequest(missingFileResponse);
        }

        await using var fileStream = request.File.OpenReadStream();
        var response = await conversionService.CreateConversionAsync(
            new CreateConversionRequest(
                fileStream,
                request.File.FileName,
                request.File.ContentType,
                request.File.Length,
                request.TargetFormat),
            cancellationToken);

        if (response.Success)
        {
            return Ok(response);
        }

        if (response.Message == "Monthly limit reached")
        {
            return UnprocessableEntity(response);
        }

        return BadRequest(response);
    }
}

public sealed class CreateConversionFormRequest
{
    public IFormFile? File { get; set; }
    public string TargetFormat { get; set; } = string.Empty;
}
