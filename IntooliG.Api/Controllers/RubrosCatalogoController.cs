using IntooliG.Api.Extensions;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RubrosConceptos;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

/// <summary>Combinaciones Rubro general + Categoría + Valor (tbCatRubro).</summary>
[ApiController]
[Route("api/rubros")]
[Authorize]
public class RubrosCatalogoController : ControllerBase
{
    private readonly IRubroService _service;

    public RubrosCatalogoController(IRubroService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<RubroCombinacionDto>>>> List(
        [FromQuery] int? categoriaId = null,
        [FromQuery] bool? activo = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var clienteId = User.GetClienteId();
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(clienteId, categoriaId, activo, search, page, pageSize, cancellationToken);
        return Ok(ApiResponse<PagedListResult<RubroCombinacionDto>>.Ok(data));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<RubroCombinacionDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var dto = await _service.GetByIdAsync(id, clienteId, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<RubroCombinacionDto>.Fail("Rubro no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<RubroCombinacionDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RubroCombinacionDto>>> Create([FromBody] RubroCombinacionCreateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var clienteId = User.GetClienteId();
            var dto = await _service.CreateAsync(clienteId, request, cancellationToken);
            return Ok(ApiResponse<RubroCombinacionDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<RubroCombinacionDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<RubroCombinacionDto>>> Update(long id, [FromBody] RubroCombinacionUpdateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var clienteId = User.GetClienteId();
            var dto = await _service.UpdateAsync(id, clienteId, request, cancellationToken);
            if (dto is null)
                return NotFound(ApiResponse<RubroCombinacionDto>.Fail("Rubro no encontrado.", StatusCodes.Status404NotFound));
            return Ok(ApiResponse<RubroCombinacionDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<RubroCombinacionDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var (eliminado, conflicto) = await _service.DeleteAsync(id, clienteId, cancellationToken);
        if (!eliminado && !conflicto)
            return NotFound(ApiResponse<object>.Fail("Rubro no encontrado.", StatusCodes.Status404NotFound));
        if (conflicto)
            return Conflict(ApiResponse<object>.Fail("No se puede eliminar: existen dependencias.", StatusCodes.Status409Conflict));
        return Ok(ApiResponse<object>.Ok(new { }, "Eliminado."));
    }
}
