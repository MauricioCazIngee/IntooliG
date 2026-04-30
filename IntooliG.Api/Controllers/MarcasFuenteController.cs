using IntooliG.Api.Extensions;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Fuentes;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/marcas-fuente")]
[Authorize]
public class MarcasFuenteController : ControllerBase
{
    private readonly IMarcaFuenteService _service;

    public MarcasFuenteController(IMarcaFuenteService service)
    {
        _service = service;
    }

    [HttpGet("por-fuente/{fuenteId:int}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MarcaFuenteDto>>>> PorFuente(int fuenteId, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var items = await _service.ListByFuenteAsync(fuenteId, clienteId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<MarcaFuenteDto>>.Ok(items));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<MarcaFuenteDto>>>> List(
        [FromQuery] string? search = null,
        [FromQuery] int? fuenteId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var clienteId = User.GetClienteId();
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(clienteId, search, fuenteId, page, pageSize, cancellationToken);
        return Ok(ApiResponse<PagedListResult<MarcaFuenteDto>>.Ok(data));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<MarcaFuenteDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var dto = await _service.GetByIdAsync(id, clienteId, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<MarcaFuenteDto>.Fail("Marca fuente no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<MarcaFuenteDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<MarcaFuenteDto>>> Create([FromBody] MarcaFuenteCreateRequest request, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        try
        {
            var dto = await _service.CreateAsync(clienteId, request, cancellationToken);
            return Ok(ApiResponse<MarcaFuenteDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<MarcaFuenteDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<MarcaFuenteDto>>> Update(
        long id,
        [FromBody] MarcaFuenteUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        try
        {
            var dto = await _service.UpdateAsync(id, clienteId, request, cancellationToken);
            if (dto is null)
                return NotFound(ApiResponse<MarcaFuenteDto>.Fail("Marca fuente no encontrada.", StatusCodes.Status404NotFound));
            return Ok(ApiResponse<MarcaFuenteDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<MarcaFuenteDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id, CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var ok = await _service.DeleteAsync(id, clienteId, cancellationToken);
        if (!ok)
            return NotFound(ApiResponse<object>.Fail("Marca fuente no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<object>.Ok(new { }, "Registro eliminado."));
    }
}
