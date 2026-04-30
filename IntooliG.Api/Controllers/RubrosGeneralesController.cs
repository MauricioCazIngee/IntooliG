using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RubrosConceptos;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/rubros-generales")]
[Authorize]
public class RubrosGeneralesController : ControllerBase
{
    private readonly IRubroGeneralService _service;

    public RubrosGeneralesController(IRubroGeneralService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<RubroGeneralDto>>>> List(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(search, page, pageSize, cancellationToken);
        return Ok(ApiResponse<PagedListResult<RubroGeneralDto>>.Ok(data));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<RubroGeneralDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<RubroGeneralDto>.Fail("Rubro general no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<RubroGeneralDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RubroGeneralDto>>> Create([FromBody] RubroGeneralCreateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _service.CreateAsync(request, cancellationToken);
            return Ok(ApiResponse<RubroGeneralDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<RubroGeneralDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RubroGeneralDto>>> Update(int id, [FromBody] RubroGeneralUpdateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _service.UpdateAsync(id, request, cancellationToken);
            if (dto is null)
                return NotFound(ApiResponse<RubroGeneralDto>.Fail("Rubro general no encontrado.", StatusCodes.Status404NotFound));
            return Ok(ApiResponse<RubroGeneralDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<RubroGeneralDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        var (eliminado, conflicto) = await _service.DeleteAsync(id, cancellationToken);
        if (!eliminado && !conflicto)
            return NotFound(ApiResponse<object>.Fail("Rubro general no encontrado.", StatusCodes.Status404NotFound));
        if (conflicto)
            return Conflict(ApiResponse<object>.Fail("No se puede eliminar: existen dependencias.", StatusCodes.Status409Conflict));
        return Ok(ApiResponse<object>.Ok(new { }, "Eliminado."));
    }
}
