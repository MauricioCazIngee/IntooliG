using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Medios;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MediosController : ControllerBase
{
    private readonly IMedioService _service;

    public MediosController(IMedioService service)
    {
        _service = service;
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MedioLookupDto>>>> Lookup(CancellationToken cancellationToken)
    {
        var items = await _service.ListActivosLookupAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<MedioLookupDto>>.Ok(items));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<MedioDto>>>> List(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? soloActivos = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var data = await _service.ListAsync(search, page, pageSize, soloActivos, cancellationToken);
        return Ok(ApiResponse<PagedListResult<MedioDto>>.Ok(data));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<MedioDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<MedioDto>.Fail("Medio no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<MedioDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<MedioDto>>> Create([FromBody] MedioCreateRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse<MedioDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<MedioDto>>> Update(int id, [FromBody] MedioUpdateRequest request, CancellationToken cancellationToken)
    {
        var dto = await _service.UpdateAsync(id, request, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<MedioDto>.Fail("Medio no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<MedioDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _service.DesactivarAsync(id, cancellationToken);
        if (!ok)
            return NotFound(ApiResponse<object>.Fail("Medio no encontrado.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<object>.Ok(new { }, "Medio desactivado."));
    }
}
