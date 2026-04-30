using IntooliG.Api.Extensions;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.DayPart;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/daypart")]
[Authorize]
public class DayPartController : ControllerBase
{
    private readonly IDayPartService _service;

    public DayPartController(IDayPartService service)
    {
        _service = service;
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<ApiResponse<DayPartLookupDto>>> Lookup(CancellationToken cancellationToken)
    {
        var data = await _service.GetLookupAsync(cancellationToken);
        return Ok(ApiResponse<DayPartLookupDto>.Ok(data));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<DayPartDto>>>> List(
        [FromQuery] string? search = null,
        [FromQuery] int? paisId = null,
        [FromQuery] int? medioId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var clienteId = User.GetClienteId();
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(clienteId, search, paisId, medioId, page, pageSize, cancellationToken);
        return Ok(ApiResponse<PagedListResult<DayPartDto>>.Ok(data));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<DayPartDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var dto = await _service.GetByIdAsync(id, clienteId, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<DayPartDto>.Fail("DayPart no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<DayPartDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<DayPartDto>>> Create([FromBody] DayPartRequestDto request, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        try
        {
            var dto = await _service.CreateAsync(clienteId, request, cancellationToken);
            return Ok(ApiResponse<DayPartDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<DayPartDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<DayPartDto>>> Update(long id, [FromBody] DayPartRequestDto request, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        try
        {
            var dto = await _service.UpdateAsync(id, clienteId, request, cancellationToken);
            if (dto is null)
                return NotFound(ApiResponse<DayPartDto>.Fail("DayPart no encontrado.", StatusCodes.Status404NotFound));
            return Ok(ApiResponse<DayPartDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<DayPartDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var (deleted, conflict) = await _service.DeleteAsync(id, clienteId, cancellationToken);
        if (!deleted && !conflict)
            return NotFound(ApiResponse<object>.Fail("DayPart no encontrado.", StatusCodes.Status404NotFound));
        if (conflict)
            return Conflict(ApiResponse<object>.Fail("No se puede eliminar el DayPart por dependencias.", StatusCodes.Status409Conflict));
        return Ok(ApiResponse<object>.Ok(new { }, "Registro eliminado."));
    }
}
