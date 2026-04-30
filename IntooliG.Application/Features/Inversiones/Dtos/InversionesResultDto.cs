namespace IntooliG.Application.Features.Inversiones;

/// <summary>Fila normalizada del UNPIVOT principal (marca x periodo).</summary>
public sealed class InversionesResultDto
{
    public string Marca { get; init; } = string.Empty;
    public string MesAnio { get; init; } = string.Empty;
    public decimal? Total { get; init; }
}
