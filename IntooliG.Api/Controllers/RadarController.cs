using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Radar;
using IntooliG.Application.Features.Radar.Dtos;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RadarController : ControllerBase
{
    private readonly IRadarService _radar;
    private readonly ILogger<RadarController> _logger;

    public RadarController(IRadarService radar, ILogger<RadarController> logger)
    {
        _radar = radar;
        _logger = logger;
    }

    [HttpGet("inversion-marca-medios")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RadarInversionMarcaMediosDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RadarInversionMarcaMediosDto>>>> InversionMarcaMedios(
        [FromQuery] int anio,
        [FromQuery] int semana,
        [FromQuery] int categoria,
        [FromQuery] string marca = "",
        [FromQuery] int pais = 1,
        CancellationToken cancellationToken = default)
    {
        var data = await _radar.GetInversionMarcaMediosAsync(anio, semana, categoria, marca, pais, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<RadarInversionMarcaMediosDto>>.Ok(data));
    }

    [HttpGet("inversion-marca-medios-version")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RadarInversionMarcaMediosVersionDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RadarInversionMarcaMediosVersionDto>>>> InversionMarcaMediosVersion(
        [FromQuery] int anio,
        [FromQuery] int semana,
        [FromQuery] int categoria,
        [FromQuery] string marca = "",
        [FromQuery] string medio = "",
        [FromQuery] int pais = 1,
        CancellationToken cancellationToken = default)
    {
        var data = await _radar.GetInversionMarcaMediosVersionAsync(anio, semana, categoria, marca, medio, pais, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<RadarInversionMarcaMediosVersionDto>>.Ok(data));
    }

    [HttpGet("marca-factores-riesgo")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RadarMarcaFactoresRiesgoDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RadarMarcaFactoresRiesgoDto>>>> MarcaFactoresRiesgo(
        [FromQuery] int anio,
        [FromQuery] int semana,
        [FromQuery] int categoria,
        [FromQuery] int rubro,
        [FromQuery] string marca = "",
        [FromQuery] int pais = 1,
        CancellationToken cancellationToken = default)
    {
        var data = await _radar.GetMarcaFactoresRiesgoAsync(anio, semana, categoria, rubro, marca, pais, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<RadarMarcaFactoresRiesgoDto>>.Ok(data));
    }

    [HttpGet("marca-valores-agregados")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RadarMarcaValoresAgregadosDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RadarMarcaValoresAgregadosDto>>>> MarcaValoresAgregados(
        [FromQuery] int anio,
        [FromQuery] int semana,
        [FromQuery] int categoria,
        [FromQuery] string marca = "",
        [FromQuery] int pais = 1,
        CancellationToken cancellationToken = default)
    {
        var data = await _radar.GetMarcaValoresAgregadosAsync(anio, semana, categoria, marca, pais, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<RadarMarcaValoresAgregadosDto>>.Ok(data));
    }

    [HttpGet("mrca-weeks")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RadarMrcaWeeksDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RadarMrcaWeeksDto>>>> MrcaWeeks(
        [FromQuery] int anio,
        [FromQuery] int semana,
        [FromQuery] int categoria,
        [FromQuery] int pais = 1,
        CancellationToken cancellationToken = default)
    {
        var data = await _radar.GetMrcaWeeksAsync(anio, semana, categoria, pais, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<RadarMrcaWeeksDto>>.Ok(data));
    }

    [HttpGet("top5")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RadarTop5Dto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RadarTop5Dto>>>> Top5(
        [FromQuery] int anio,
        [FromQuery] int semana,
        [FromQuery] int categoria,
        CancellationToken cancellationToken = default)
    {
        var data = await _radar.GetTop5Async(anio, semana, categoria, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<RadarTop5Dto>>.Ok(data));
    }

    [HttpGet("estados")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RadarEstadoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RadarEstadoDto>>>> Estados(
        [FromQuery] int anio,
        [FromQuery] int semana,
        [FromQuery] int categoria,
        [FromQuery] string marca = "",
        [FromQuery] int pais = 1,
        CancellationToken cancellationToken = default)
    {
        if (anio <= 0 || semana <= 0 || categoria <= 0 || pais <= 0)
        {
            return BadRequest(ApiResponse<IReadOnlyList<RadarEstadoDto>>.Fail(
                "Parámetros inválidos. anio, semana, categoria y pais deben ser mayores a cero.",
                StatusCodes.Status400BadRequest));
        }

        try
        {
            var data = await _radar.GetEstadosAsync(anio, semana, categoria, marca, pais, cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<RadarEstadoDto>>.Ok(data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar SPWEBRadarEstados con anio={Anio}, semana={Semana}, categoria={Categoria}, marca={Marca}, pais={Pais}",
                anio, semana, categoria, marca, pais);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<IReadOnlyList<RadarEstadoDto>>.Fail("Error al consultar radar de comunicaciones.", StatusCodes.Status500InternalServerError));
        }
    }
}
