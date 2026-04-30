using IntooliG.Api.Extensions;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RubrosConceptos;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/conceptos")]
[Authorize]
public class ConceptosCatalogoController : ControllerBase
{
    private readonly IConceptoService _service;

    public ConceptosCatalogoController(IConceptoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<ConceptoListItemDto>>>> List(
        [FromQuery] int? categoriaId = null,
        [FromQuery] int? rubroGeneralId = null,
        [FromQuery] bool? activo = null,
        [FromQuery] bool? top = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var clienteId = User.GetClienteId();
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(
            clienteId, categoriaId, rubroGeneralId, activo, top, search, page, pageSize, cancellationToken);
        return Ok(ApiResponse<PagedListResult<ConceptoListItemDto>>.Ok(data));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<ConceptoDetailDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var dto = await _service.GetByIdAsync(id, clienteId, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<ConceptoDetailDto>.Fail("Concepto no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<ConceptoDetailDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ConceptoDetailDto>>> Create([FromBody] ConceptoCreateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var clienteId = User.GetClienteId();
            var dto = await _service.CreateAsync(clienteId, request, cancellationToken);
            return Ok(ApiResponse<ConceptoDetailDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ConceptoDetailDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ConceptoDetailDto>>> Update(long id, [FromBody] ConceptoUpdateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var clienteId = User.GetClienteId();
            var dto = await _service.UpdateAsync(id, clienteId, request, cancellationToken);
            if (dto is null)
                return NotFound(ApiResponse<ConceptoDetailDto>.Fail("Concepto no encontrado.", StatusCodes.Status404NotFound));
            return Ok(ApiResponse<ConceptoDetailDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ConceptoDetailDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var (eliminado, conflicto) = await _service.DeleteAsync(id, clienteId, cancellationToken);
        if (!eliminado && !conflicto)
            return NotFound(ApiResponse<object>.Fail("Concepto no encontrado.", StatusCodes.Status404NotFound));
        if (conflicto)
            return Conflict(ApiResponse<object>.Fail("No se puede eliminar: existen dependencias.", StatusCodes.Status409Conflict));
        return Ok(ApiResponse<object>.Ok(new { }, "Eliminado."));
    }
}
