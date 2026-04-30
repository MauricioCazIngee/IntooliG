using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CiudadesController : ControllerBase
{
    private readonly ICiudadService _service;

    public CiudadesController(ICiudadService service)
    {
        _service = service;
    }

    [HttpGet("selector")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CiudadSelectorItemDto>>>> Selector(
        [FromQuery] int? paisId = null,
        [FromQuery] int? estadoId = null,
        CancellationToken cancellationToken = default)
    {
        var rows = await _service.ListForSelectorAsync(paisId, estadoId, cancellationToken);
        var list = rows.Select(x => new CiudadSelectorItemDto(x.Id, x.Nombre)).ToList();
        return Ok(ApiResponse<IReadOnlyList<CiudadSelectorItemDto>>.Ok(list));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<CiudadDto>>>> List(
        [FromQuery] string? search = null,
        [FromQuery] int? paisId = null,
        [FromQuery] int? estadoId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? soloActivos = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(search, paisId, estadoId, page, pageSize, soloActivos, cancellationToken);
        return Ok(ApiResponse<PagedListResult<CiudadDto>>.Ok(data));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<CiudadDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<CiudadDto>.Fail("Ciudad no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<CiudadDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CiudadDto>>> Create([FromBody] CiudadCreateRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse<CiudadDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CiudadDto>>> Update(int id, [FromBody] CiudadUpdateRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.UpdateAsync(id, request, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<CiudadDto>.Fail("Ciudad no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<CiudadDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _service.DesactivarAsync(id, cancellationToken);
        if (!ok)
            return NotFound(ApiResponse<object>.Fail("Ciudad no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<object>.Ok(new { }, "Registro desactivado."));
    }
}
