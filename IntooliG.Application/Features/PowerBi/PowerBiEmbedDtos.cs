namespace IntooliG.Application.Features.PowerBi;

/// <summary>Configuración para embeber report, dashboard o tile (powerbi-client en el front).</summary>
public sealed class PowerBiEmbedResponseDto
{
    public string EmbedToken { get; set; } = string.Empty;
    public string TokenExpiry { get; set; } = string.Empty;

    /// <summary>Id del informe, dashboard o tile, según <see cref="Type"/>.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Compat: mismo valor que <see cref="Id"/> para <see cref="Type"/>= report.</summary>
    public string ReportId { get; set; } = string.Empty;

    public string EmbedUrl { get; set; } = string.Empty;

    /// <summary>report | dashboard | tile</summary>
    public string Type { get; set; } = "report";

    /// <summary>Padre del mosaico (solo <see cref="Type"/> = tile).</summary>
    public string? DashboardId { get; set; }

    public bool IsPaginatedReport { get; set; }
}
