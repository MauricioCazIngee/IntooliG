using IntooliG.Api.Extensions;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.InformationLoad;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/information-load/carga-global")]
[Authorize]
public class InformationLoadController : ControllerBase
{
    private readonly IInformationLoadService _service;

    public InformationLoadController(IInformationLoadService service)
    {
        _service = service;
    }

    [HttpGet("filtros")]
    public async Task<ActionResult<ApiResponse<CargaGlobalFiltrosDto>>> Filtros(
        [FromQuery] int? paisId = null,
        CancellationToken cancellationToken = default)
    {
        var clienteId = User.GetClienteId();
        var data = await _service.GetCargaGlobalFiltrosAsync(clienteId, paisId, cancellationToken);
        return Ok(ApiResponse<CargaGlobalFiltrosDto>.Ok(data));
    }

    [HttpGet("log")]
    public async Task<ActionResult<ApiResponse<CargaGlobalLogDto>>> UltimoLog(
        [FromQuery] int paisId,
        CancellationToken cancellationToken = default)
    {
        var clienteId = User.GetClienteId();
        var data = await _service.GetUltimoLogProcesoAsync(clienteId, paisId, cancellationToken);
        if (data is null)
            return Ok(ApiResponse<CargaGlobalLogDto>.Ok(new CargaGlobalLogDto { Mensaje = "Sin ejecuciones registradas." }));
        return Ok(ApiResponse<CargaGlobalLogDto>.Ok(data));
    }

    [HttpGet("estado-catalogacion")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CargaGlobalEstadoCatalogacionDto>>>> EstadoCatalogacion(
        [FromQuery] int paisId,
        CancellationToken cancellationToken = default)
    {
        var data = await _service.GetEstadoCatalogacionPaisAsync(paisId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CargaGlobalEstadoCatalogacionDto>>.Ok(data));
    }

    [HttpPost("upload")]
    [RequestSizeLimit(1024L * 1024L * 512L)]
    public async Task<ActionResult<ApiResponse<CargaGlobalUploadResultDto>>> Upload(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length <= 0)
            return BadRequest(ApiResponse<CargaGlobalUploadResultDto>.Fail("Debe enviar un archivo.", StatusCodes.Status400BadRequest));

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await _service.GuardarArchivoTemporalAsync(file.FileName, stream, file.Length, cancellationToken);
            return Ok(ApiResponse<CargaGlobalUploadResultDto>.Ok(result, "Archivo cargado correctamente."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CargaGlobalUploadResultDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPost("ejecutar-proceso")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CargaGlobalProcessResultDto>>> EjecutarProceso(
        [FromBody] CargaGlobalProcessRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var clienteId = User.GetClienteId();
        var usuarioId = User.GetUserId();

        try
        {
            var result = await _service.EjecutarProcesoAsync(clienteId, usuarioId, request, cancellationToken);
            if (!result.Exito)
                return BadRequest(ApiResponse<CargaGlobalProcessResultDto>.Fail(result.Mensaje, StatusCodes.Status400BadRequest, result));
            return Ok(ApiResponse<CargaGlobalProcessResultDto>.Ok(result));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CargaGlobalProcessResultDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPost("finalizar-catalogacion/{paisId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> FinalizarCatalogacion(
        int paisId,
        CancellationToken cancellationToken = default)
    {
        var usuarioId = User.GetUserId();
        try
        {
            var done = await _service.FinalizarCatalogacionPaisAsync(paisId, usuarioId, cancellationToken);
            if (!done)
                return BadRequest(ApiResponse<object>.Fail("No fue posible finalizar la catalogación.", StatusCodes.Status400BadRequest));
            return Ok(ApiResponse<object>.Ok(new { paisId, finalizado = true }, "Catalogación finalizada."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }
}
