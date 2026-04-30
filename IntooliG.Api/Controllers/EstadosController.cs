using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EstadosController : ControllerBase
{
    private readonly IEstadoService _service;

    public EstadosController(IEstadoService service)
    {
        _service = service;
    }

    [HttpGet("codigos-mapa")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CodigoMapaLookupDto>>>> CodigosMapa(CancellationToken cancellationToken)
    {
        var rows = await _service.GetCodigosMapaAsync(cancellationToken);
        var list = rows.Select(x => new CodigoMapaLookupDto(x.Id, x.Nombre)).ToList();
        return Ok(ApiResponse<IReadOnlyList<CodigoMapaLookupDto>>.Ok(list));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<EstadoDto>>>> List(
        [FromQuery] string? search = null,
        [FromQuery] int? paisId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? soloActivos = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(search, paisId, page, pageSize, soloActivos, cancellationToken);
        return Ok(ApiResponse<PagedListResult<EstadoDto>>.Ok(data));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<EstadoDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<EstadoDto>.Fail("Estado no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<EstadoDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<EstadoDto>>> Create([FromBody] EstadoCreateRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse<EstadoDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<EstadoDto>>> Update(int id, [FromBody] EstadoUpdateRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.UpdateAsync(id, request, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<EstadoDto>.Fail("Estado no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<EstadoDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _service.DesactivarAsync(id, cancellationToken);
        if (!ok)
            return NotFound(ApiResponse<object>.Fail("Estado no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<object>.Ok(new { }, "Registro desactivado."));
    }
}
