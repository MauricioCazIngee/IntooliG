using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Medios;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/medios-cliente")]
[Authorize]
public class MediosClienteController : ControllerBase
{
    private readonly IMedioClienteService _service;

    public MediosClienteController(IMedioClienteService service)
    {
        _service = service;
    }

    [HttpGet("por-cliente/{clienteId:int}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MedioClientePorClienteDto>>>> PorCliente(
        int clienteId,
        CancellationToken cancellationToken)
    {
        var items = await _service.ListByClienteAsync(clienteId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<MedioClientePorClienteDto>>.Ok(items));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<MedioClienteDto>>>> List(
        [FromQuery] string? search = null,
        [FromQuery] int? clienteId = null,
        [FromQuery] int? medioId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(search, clienteId, medioId, page, pageSize, cancellationToken);
        return Ok(ApiResponse<PagedListResult<MedioClienteDto>>.Ok(data));
    }

    [HttpGet("{medioId:int}/{clienteId:int}")]
    public async Task<ActionResult<ApiResponse<MedioClienteDto>>> GetByKey(
        int medioId,
        int clienteId,
        CancellationToken cancellationToken)
    {
        var dto = await _service.GetByKeyAsync(medioId, clienteId, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<MedioClienteDto>.Fail("Asignación no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<MedioClienteDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<MedioClienteDto>>> Create(
        [FromBody] MedioClienteCreateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _service.CreateAsync(request, cancellationToken);
            return Ok(ApiResponse<MedioClienteDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<MedioClienteDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPut("{medioId:int}/{clienteId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<MedioClienteDto>>> Update(
        int medioId,
        int clienteId,
        [FromBody] MedioClienteUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await _service.UpdateAsync(medioId, clienteId, request, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<MedioClienteDto>.Fail("Asignación no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<MedioClienteDto>.Ok(dto));
    }

    [HttpDelete("{medioId:int}/{clienteId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int medioId, int clienteId, CancellationToken cancellationToken)
    {
        var ok = await _service.DeleteAsync(medioId, clienteId, cancellationToken);
        if (!ok)
            return NotFound(ApiResponse<object>.Fail("Asignación no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<object>.Ok(new { }, "Asignación eliminada."));
    }
}
