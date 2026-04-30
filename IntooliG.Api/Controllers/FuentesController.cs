using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Fuentes;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FuentesController : ControllerBase
{
    private readonly IFuenteService _service;

    public FuentesController(IFuenteService service)
    {
        _service = service;
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<FuenteLookupDto>>>> Lookup(CancellationToken cancellationToken)
    {
        var items = await _service.ListActivosLookupAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<FuenteLookupDto>>.Ok(items));
    }

    [HttpGet("por-pais/{paisId:int}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<FuenteDto>>>> PorPais(int paisId, CancellationToken cancellationToken)
    {
        var items = await _service.ListByPaisAsync(paisId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<FuenteDto>>.Ok(items));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedListResult<FuenteDto>>>> List(
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
        return Ok(ApiResponse<PagedListResult<FuenteDto>>.Ok(data));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<FuenteDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        if (dto is null)
            return NotFound(ApiResponse<FuenteDto>.Fail("Fuente no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<FuenteDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<FuenteDto>>> Create([FromBody] FuenteCreateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _service.CreateAsync(request, cancellationToken);
            return Ok(ApiResponse<FuenteDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<FuenteDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<FuenteDto>>> Update(int id, [FromBody] FuenteUpdateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _service.UpdateAsync(id, request, cancellationToken);
            if (dto is null)
                return NotFound(ApiResponse<FuenteDto>.Fail("Fuente no encontrada.", StatusCodes.Status404NotFound));
            return Ok(ApiResponse<FuenteDto>.Ok(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<FuenteDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _service.DesactivarAsync(id, cancellationToken);
        if (!ok)
            return NotFound(ApiResponse<object>.Fail("Fuente no encontrada.", StatusCodes.Status404NotFound));
        return Ok(ApiResponse<object>.Ok(new { }, "Fuente desactivada."));
    }
}
