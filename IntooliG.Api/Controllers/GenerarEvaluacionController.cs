using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Radar.Dtos;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GenerarEvaluacionController : ControllerBase
{
    private readonly IGenerarEvaluacionService _service;
    private readonly ILogger<GenerarEvaluacionController> _logger;

    public GenerarEvaluacionController(
        IGenerarEvaluacionService service,
        ILogger<GenerarEvaluacionController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("campanias")]
    [ProducesResponseType(typeof(ApiResponse<PagedListResult<CampaniaEvaluacionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagedListResult<CampaniaEvaluacionDto>>>> Campanias(
        [FromQuery] int? anio = null,
        [FromQuery] int? categoriaId = null,
        [FromQuery] bool? activo = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var (items, total) = await _service.GetCampaniasEvaluacionAsync(
                anio, categoriaId, activo, search, page, pageSize);

            var payload = new PagedListResult<CampaniaEvaluacionDto>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = ComputeTotalPages(total, pageSize)
            };

            return Ok(ApiResponse<PagedListResult<CampaniaEvaluacionDto>>.Ok(payload));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error al listar campañas evaluación. anio={Anio}, categoriaId={CategoriaId}, activo={Activo}, page={Page}",
                anio, categoriaId, activo, page);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<PagedListResult<CampaniaEvaluacionDto>>.Fail(
                    "Error al consultar campañas (Alta-Baja-Cambios).",
                    StatusCodes.Status500InternalServerError));
        }
    }

    [HttpGet("administracion")]
    [ProducesResponseType(typeof(ApiResponse<PagedListResult<AdministracionCampaniaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagedListResult<AdministracionCampaniaDto>>>> Administracion(
        [FromQuery] int? anio = null,
        [FromQuery] int? sectorId = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var (items, total) = await _service.GetAdministracionCampaniasAsync(
                anio, sectorId, search, page, pageSize);

            var payload = new PagedListResult<AdministracionCampaniaDto>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = ComputeTotalPages(total, pageSize)
            };

            return Ok(ApiResponse<PagedListResult<AdministracionCampaniaDto>>.Ok(payload));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error al listar administración campañas. anio={Anio}, sectorId={SectorId}, page={Page}",
                anio, sectorId, page);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<PagedListResult<AdministracionCampaniaDto>>.Fail(
                    "Error al consultar administración de campañas.",
                    StatusCodes.Status500InternalServerError));
        }
    }

    private static int ComputeTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0)
            pageSize = 10;
        if (total <= 0)
            return 0;
        return (total + pageSize - 1) / pageSize;
    }
}
