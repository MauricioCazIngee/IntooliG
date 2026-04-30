using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.TipoCambio;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/tipo-cambio")]
[Authorize]
public class TipoCambioController : ControllerBase
{
    private readonly ITipoCambioService _service;

    public TipoCambioController(ITipoCambioService service)
    {
        _service = service;
    }

    [HttpGet("anios")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TipoCambioAnioDto>>>> ListAnios(CancellationToken cancellationToken)
    {
        var items = await _service.ListAniosAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TipoCambioAnioDto>>.Ok(items));
    }

    [HttpGet("meses")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TipoCambioMesDto>>>> ListMeses(CancellationToken cancellationToken)
    {
        var items = await _service.ListMesesAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TipoCambioMesDto>>.Ok(items));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<TipoCambioDto>>>> List(
        [FromQuery] string? search = null,
        [FromQuery] int? paisId = null,
        [FromQuery] int? anio = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(search, paisId, anio, page, pageSize, cancellationToken);
        return Ok(ApiResponse<PagedListResult<TipoCambioDto>>.Ok(data));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<TipoCambioDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<TipoCambioDto>.Fail("Tipo de cambio no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<TipoCambioDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<TipoCambioDto>>> Create([FromBody] TipoCambioRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _service.CreateAsync(request, cancellationToken);
            return Ok(ApiResponse<TipoCambioDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<TipoCambioDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<TipoCambioDto>>> Update(
        int id,
        [FromBody] TipoCambioRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _service.UpdateAsync(id, request, cancellationToken);
            if (dto is null)
                return NotFound(ApiResponse<TipoCambioDto>.Fail("Tipo de cambio no encontrado.", StatusCodes.Status404NotFound));
            return Ok(ApiResponse<TipoCambioDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<TipoCambioDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _service.DeleteAsync(id, cancellationToken);
        if (!ok)
            return NotFound(ApiResponse<object>.Fail("Tipo de cambio no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<object>.Ok(new { }, "Registro eliminado."));
    }
}
