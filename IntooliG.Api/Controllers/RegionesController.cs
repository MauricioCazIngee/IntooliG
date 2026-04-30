using IntooliG.Api.Extensions;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RegionesController : ControllerBase
{
    private readonly IRegionService _service;

    public RegionesController(IRegionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<RegionDto>>>> List(
        [FromQuery] string? search = null,
        [FromQuery] int? paisId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? soloActivos = null,
        CancellationToken cancellationToken = default)
    {
        var clienteId = User.GetClienteId();
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(clienteId, search, paisId, page, pageSize, soloActivos, cancellationToken);
        return Ok(ApiResponse<PagedListResult<RegionDto>>.Ok(data));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<RegionDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var dto = await _service.GetByIdAsync(id, clienteId, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<RegionDto>.Fail("Región no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<RegionDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RegionDto>>> Create([FromBody] RegionCreateRequest request, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var dto = await _service.CreateAsync(clienteId, request, cancellationToken);
        return Ok(ApiResponse<RegionDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RegionDto>>> Update(int id, [FromBody] RegionUpdateRequest request, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var dto = await _service.UpdateAsync(id, clienteId, request, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<RegionDto>.Fail("Región no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<RegionDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var ok = await _service.DesactivarAsync(id, clienteId, cancellationToken);
        if (!ok)
            return NotFound(ApiResponse<object>.Fail("Región no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<object>.Ok(new { }, "Registro desactivado."));
    }
}
