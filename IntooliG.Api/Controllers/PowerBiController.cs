using IntooliG.Api.Extensions;
using IntooliG.Application.Abstractions.Services;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.PowerBi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/power-bi")]
[Authorize]
public class PowerBiController : ControllerBase
{
    private readonly IPowerBiEmbedService _powerBi;

    public PowerBiController(IPowerBiEmbedService powerBi)
    {
        _powerBi = powerBi;
    }

    /// <summary>Compat: mismo informe por config; rol PBI y RLS desde <see cref="GetReportEmbedForUserAsync"/> (legado <c>EmbedReport</c> simple).</summary>
    [HttpGet("embed")]
    [ProducesResponseType(typeof(ApiResponse<PowerBiEmbedResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PowerBiEmbedResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PowerBiEmbedResponseDto>>> Embed(
        [FromQuery] bool rdl = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var id = User.GetUserId();
            var data = await _powerBi.GetReportEmbedForUserAsync(id, null, rdl, cancellationToken)
                .ConfigureAwait(false);
            return Ok(ApiResponse<PowerBiEmbedResponseDto>.Ok(data, "Configuración de embebido lista."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                ApiResponse<PowerBiEmbedResponseDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    /// <summary>Informe con <paramref name="reportId"/> y RDL/RLS; equivale a <c>PowerBIController.EmbedReport</c> del legado.</summary>
    [HttpGet("embed-report")]
    [ProducesResponseType(typeof(ApiResponse<PowerBiEmbedResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PowerBiEmbedResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PowerBiEmbedResponseDto>>> EmbedReport(
        [FromQuery] string? reportId = null,
        [FromQuery] bool rdl = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var id = User.GetUserId();
            var data = await _powerBi.GetReportEmbedForUserAsync(id, reportId, rdl, cancellationToken)
                .ConfigureAwait(false);
            return Ok(ApiResponse<PowerBiEmbedResponseDto>.Ok(data, "Informe listo para embebido."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                ApiResponse<PowerBiEmbedResponseDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpGet("embed-dashboard")]
    [ProducesResponseType(typeof(ApiResponse<PowerBiEmbedResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PowerBiEmbedResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PowerBiEmbedResponseDto>>> EmbedDashboard(
        [FromQuery] string? dashboardId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var data = await _powerBi.GetDashboardEmbedAsync(dashboardId, cancellationToken)
                .ConfigureAwait(false);
            return Ok(ApiResponse<PowerBiEmbedResponseDto>.Ok(data, "Panel listo para embebido."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                ApiResponse<PowerBiEmbedResponseDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpGet("embed-tile")]
    [ProducesResponseType(typeof(ApiResponse<PowerBiEmbedResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PowerBiEmbedResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PowerBiEmbedResponseDto>>> EmbedTile(
        [FromQuery] string? dashboardId = null,
        [FromQuery] string? tileId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var data = await _powerBi.GetTileEmbedAsync(dashboardId, tileId, cancellationToken)
                .ConfigureAwait(false);
            return Ok(ApiResponse<PowerBiEmbedResponseDto>.Ok(data, "Mosaico listo para embebido."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                ApiResponse<PowerBiEmbedResponseDto>.Fail(ex.Message, StatusCodes.Status400BadRequest));
        }
    }
}
