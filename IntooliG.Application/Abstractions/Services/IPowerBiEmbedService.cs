using IntooliG.Application.Features.PowerBi;

namespace IntooliG.Application.Abstractions.Services;

public interface IPowerBiEmbedService
{
    /// <summary>Informe con RLS/rol (tbUser.FiRolPbiID / tbCatRolesPBI) y RDL vía rdl/Api.</summary>
    Task<PowerBiEmbedResponseDto> GetReportEmbedForUserAsync(
        int usuarioId,
        string? reportId,
        bool isRdl,
        CancellationToken cancellationToken = default);

    /// <summary>Embebido de panel (mismo criterio legado <c>EmbedDashboard</c>).</summary>
    Task<PowerBiEmbedResponseDto> GetDashboardEmbedAsync(
        string? dashboardId,
        CancellationToken cancellationToken = default);

    /// <summary>Embebido de mosaico (criterio legado <c>EmbedTile</c>).</summary>
    Task<PowerBiEmbedResponseDto> GetTileEmbedAsync(
        string? dashboardId,
        string? tileId,
        CancellationToken cancellationToken = default);
}
