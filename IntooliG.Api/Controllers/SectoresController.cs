using IntooliG.Api.Extensions;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Catalogo;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SectoresController : ControllerBase
{
    private readonly ISectorService _service;

    public SectoresController(ISectorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<SectorDto>>>> List(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var clienteId = User.GetClienteId();
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(clienteId, search, page, pageSize, cancellationToken);
        return Ok(ApiResponse<PagedListResult<SectorDto>>.Ok(data));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<SectorDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var dto = await _service.GetByIdAsync(id, clienteId, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<SectorDto>.Fail("Sector no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<SectorDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SectorDto>>> Create([FromBody] SectorCreateRequest request, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var dto = await _service.CreateAsync(clienteId, request, cancellationToken);
        return Ok(ApiResponse<SectorDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SectorDto>>> Update(int id, [FromBody] SectorUpdateRequest request, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var dto = await _service.UpdateAsync(id, clienteId, request, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<SectorDto>.Fail("Sector no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<SectorDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var (eliminado, conflicto) = await _service.DeleteAsync(id, clienteId, cancellationToken);
        if (!eliminado && !conflicto)
            return NotFound(ApiResponse<object>.Fail("Sector no encontrado.", StatusCodes.Status404NotFound));
        if (conflicto)
            return Conflict(ApiResponse<object>.Fail("No se puede eliminar: existen dependencias (p. ej. BU).", StatusCodes.Status409Conflict));
        return Ok(ApiResponse<object>.Ok(new { }, "Eliminado."));
    }
}
