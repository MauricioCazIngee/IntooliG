namespace IntooliG.Application.Features.Inversiones;

/// <summary>Resultado genérico de SP para gráficos (columnas variables).</summary>
public sealed class InversionesTablaDinamicaDto
{
    public IReadOnlyList<string> Columnas { get; init; } = Array.Empty<string>();
    public IReadOnlyList<IReadOnlyDictionary<string, object?>> Filas { get; init; } =
        Array.Empty<IReadOnlyDictionary<string, object?>>();
}
