using IntooliG.Api.Extensions;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Fuentes;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/versiones-fuente")]
[Authorize]
public class VersionesFuenteController : ControllerBase
{
    private readonly IVersionFuenteService _service;

    public VersionesFuenteController(IVersionFuenteService service)
    {
        _service = service;
    }

    [HttpGet("version-tv/lookup")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<VersionTVLookupDto>>>> VersionTvLookup(CancellationToken cancellationToken)
    {
        var items = await _service.ListVersionTVLookupAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<VersionTVLookupDto>>.Ok(items));
    }

    [HttpGet("por-fuente/{fuenteId:int}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<VersionFuenteDto>>>> PorFuente(int fuenteId, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var items = await _service.ListByFuenteAsync(fuenteId, clienteId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<VersionFuenteDto>>.Ok(items));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<VersionFuenteDto>>>> List(
        [FromQuery] string? search = null,
        [FromQuery] int? fuenteId = null,
        [FromQuery] int? categoriaId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? soloActivos = null,
        CancellationToken cancellationToken = default)
    {
        var clienteId = User.GetClienteId();
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(clienteId, search, fuenteId, categoriaId, page, pageSize, soloActivos, cancellationToken);
        return Ok(ApiResponse<PagedListResult<VersionFuenteDto>>.Ok(data));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<VersionFuenteDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var dto = await _service.GetByIdAsync(id, clienteId, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<VersionFuenteDto>.Fail("Versión fuente no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<VersionFuenteDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<VersionFuenteDto>>> Create([FromBody] VersionFuenteCreateRequest request, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        try
        {
            var dto = await _service.CreateAsync(clienteId, request, cancellationToken);
            return Ok(ApiResponse<VersionFuenteDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<VersionFuenteDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<VersionFuenteDto>>> Update(int id, [FromBody] VersionFuenteUpdateRequest request, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        try
        {
            var dto = await _service.UpdateAsync(id, clienteId, request, cancellationToken);
            if (dto is null)
                return NotFound(ApiResponse<VersionFuenteDto>.Fail("Versión fuente no encontrada.", StatusCodes.Status404NotFound));
            return Ok(ApiResponse<VersionFuenteDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<VersionFuenteDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var ok = await _service.DesactivarAsync(id, clienteId, cancellationToken);
        if (!ok)
            return NotFound(ApiResponse<object>.Fail("Versión fuente no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<object>.Ok(new { }, "Versión fuente desactivada."));
    }
}
