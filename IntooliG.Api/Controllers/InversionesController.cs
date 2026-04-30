using IntooliG.Api.Extensions;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Inversiones;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/inversiones")]
[Authorize]
public class InversionesController : ControllerBase
{
    private readonly IInversionesService _service;

    public InversionesController(IInversionesService service)
    {
        _service = service;
    }

    [HttpGet("filtros")]
    public async Task<ActionResult<ApiResponse<InversionesFiltrosDto>>> Filtros(CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var userId = User.GetUserId();
        var data = await _service.GetFiltrosAsync(clienteId, userId, cancellationToken);
        return Ok(ApiResponse<InversionesFiltrosDto>.Ok(data));
    }

    [HttpGet("filtros/ciudades")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CiudadFiltroDto>>>> CiudadesPorRegion(
        [FromQuery] int regionId,
        CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var userId = User.GetUserId();
        var data = await _service.GetCiudadesPorRegionAsync(clienteId, userId, regionId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CiudadFiltroDto>>.Ok(data));
    }

    [HttpPost("consultar")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<InversionesResultDto>>>> Consultar(
        [FromBody] InversionesRequestDto request,
        CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var userId = User.GetUserId();
        try
        {
            var data = await _service.ConsultarAsync(clienteId, userId, request, cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<InversionesResultDto>>.Ok(data));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<IReadOnlyList<InversionesResultDto>>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPost("barras")]
    public async Task<ActionResult<ApiResponse<InversionesTablaDinamicaDto>>> Barras(
        [FromBody] InversionesRequestDto request,
        CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var userId = User.GetUserId();
        try
        {
            var data = await _service.GetBarrasAsync(clienteId, userId, request, cancellationToken);
            return Ok(ApiResponse<InversionesTablaDinamicaDto>.Ok(data));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<InversionesTablaDinamicaDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPost("pie-categoria")]
    public async Task<ActionResult<ApiResponse<InversionesTablaDinamicaDto>>> PieCategoria(
        [FromBody] InversionesRequestDto request,
        CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var userId = User.GetUserId();
        try
        {
            var data = await _service.GetPieCategoriaAsync(clienteId, userId, request, cancellationToken);
            return Ok(ApiResponse<InversionesTablaDinamicaDto>.Ok(data));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<InversionesTablaDinamicaDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPost("pie-marca")]
    public async Task<ActionResult<ApiResponse<InversionesTablaDinamicaDto>>> PieMarca(
        [FromBody] InversionesRequestDto request,
        CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var userId = User.GetUserId();
        try
        {
            var data = await _service.GetPieMarcaAsync(clienteId, userId, request, cancellationToken);
            return Ok(ApiResponse<InversionesTablaDinamicaDto>.Ok(data));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<InversionesTablaDinamicaDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPost("pie-medio")]
    public async Task<ActionResult<ApiResponse<InversionesTablaDinamicaDto>>> PieMedio(
        [FromBody] InversionesRequestDto request,
        CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var userId = User.GetUserId();
        try
        {
            var data = await _service.GetPieMedioAsync(clienteId, userId, request, cancellationToken);
            return Ok(ApiResponse<InversionesTablaDinamicaDto>.Ok(data));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<InversionesTablaDinamicaDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }
}
