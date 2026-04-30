using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PoblacionesController : ControllerBase
{
    private readonly IPoblacionService _service;

    public PoblacionesController(IPoblacionService service)
    {
        _service = service;
    }

    [HttpGet("anios")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<int>>>> Anios(CancellationToken cancellationToken)
    {
        var list = await _service.GetAniosDisponiblesAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<int>>.Ok(list));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<PoblacionDto>>>> List(
        [FromQuery] string? search = null,
        [FromQuery] int? paisId = null,
        [FromQuery] int? estadoId = null,
        [FromQuery] int? anio = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(search, paisId, estadoId, anio, page, pageSize, cancellationToken);
        return Ok(ApiResponse<PagedListResult<PoblacionDto>>.Ok(data));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PoblacionDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<PoblacionDto>.Fail("Registro no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<PoblacionDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PoblacionDto>>> Create([FromBody] PoblacionCreateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _service.CreateAsync(request, cancellationToken);
            return Ok(ApiResponse<PoblacionDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<PoblacionDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PoblacionDto>>> Update(int id, [FromBody] PoblacionUpdateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _service.UpdateAsync(id, request, cancellationToken);
            if (dto is null)
                return NotFound(ApiResponse<PoblacionDto>.Fail("Registro no encontrado.", StatusCodes.Status404NotFound));
            return Ok(ApiResponse<PoblacionDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<PoblacionDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _service.EliminarAsync(id, cancellationToken);
        if (!ok)
            return NotFound(ApiResponse<object>.Fail("Registro no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<object>.Ok(new { }, "Eliminado."));
    }
}
