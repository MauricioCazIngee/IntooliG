using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Campanias;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CampaniasController : ControllerBase
{
    private readonly ICampaniaService _campanias;
    private readonly ILogger<CampaniasController> _logger;

    public CampaniasController(ICampaniaService campanias, ILogger<CampaniasController> logger)
    {
        _campanias = campanias;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CampaniaDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CampaniaDto>>>> List(CancellationToken cancellationToken)
    {
        var data = await _campanias.ListAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CampaniaDto>>.Ok(data));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CampaniaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CampaniaDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _campanias.GetByIdAsync(id, cancellationToken);
        if (item is null)
            return NotFound(ApiResponse<CampaniaDto>.Fail("Campaña no encontrada.", StatusCodes.Status404NotFound));

        return Ok(ApiResponse<CampaniaDto>.Ok(item));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CampaniaDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CampaniaDto>>> Create([FromBody] CreateCampaniaRequest request, CancellationToken cancellationToken)
    {
        var created = await _campanias.CreateAsync(request, cancellationToken);
        _logger.LogInformation("Campaña creada {Id} {Codigo}", created.Id, created.Codigo);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<CampaniaDto>.Ok(created, "Campaña creada.", StatusCodes.Status201Created));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CampaniaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CampaniaDto>>> Update(int id, [FromBody] UpdateCampaniaRequest request, CancellationToken cancellationToken)
    {
        var updated = await _campanias.UpdateAsync(id, request, cancellationToken);
        if (updated is null)
            return NotFound(ApiResponse<CampaniaDto>.Fail("Campaña no encontrada.", StatusCodes.Status404NotFound));

        return Ok(ApiResponse<CampaniaDto>.Ok(updated, "Campaña actualizada."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _campanias.DeleteAsync(id, cancellationToken);
        if (!ok)
            return NotFound(ApiResponse<bool>.Fail("Campaña no encontrada.", StatusCodes.Status404NotFound));

        return Ok(ApiResponse<bool>.Ok(true, "Campaña eliminada."));
    }
}
