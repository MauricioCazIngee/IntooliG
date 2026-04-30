using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaisesController : ControllerBase
{
    private readonly IPaisService _service;

    public PaisesController(IPaisService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<PaisDto>>>> List(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? soloActivos = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(search, page, pageSize, soloActivos, cancellationToken);
        return Ok(ApiResponse<PagedListResult<PaisDto>>.Ok(data));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PaisDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<PaisDto>.Fail("País no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<PaisDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PaisDto>>> Create([FromBody] PaisCreateRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse<PaisDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PaisDto>>> Update(int id, [FromBody] PaisUpdateRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.UpdateAsync(id, request, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<PaisDto>.Fail("País no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<PaisDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _service.DesactivarAsync(id, cancellationToken);
        if (!ok)
            return NotFound(ApiResponse<object>.Fail("País no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<object>.Ok(new { }, "Registro desactivado."));
    }
}
