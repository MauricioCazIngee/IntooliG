using System.Text.Json.Serialization;

namespace IntooliG.Application.Features.Inversiones;

public sealed class InversionesFiltrosDto
{
    public IReadOnlyList<PaisDto> Paises { get; init; } = Array.Empty<PaisDto>();
    public IReadOnlyList<RegionDto> Regiones { get; init; } = Array.Empty<RegionDto>();
    public IReadOnlyList<SectorDto> Sectores { get; init; } = Array.Empty<SectorDto>();

    [JsonPropertyName("bus")]
    public IReadOnlyList<BUDto> BUs { get; init; } = Array.Empty<BUDto>();
    public IReadOnlyList<CategoriaDto> Categorias { get; init; } = Array.Empty<CategoriaDto>();
    public IReadOnlyList<MarcaFiltroDto> Marcas { get; init; } = Array.Empty<MarcaFiltroDto>();
    public IReadOnlyList<PeriodTypeDto> Periodos { get; init; } = Array.Empty<PeriodTypeDto>();
    public IReadOnlyList<TarifaDto> Tarifas { get; init; } = Array.Empty<TarifaDto>();
    public IReadOnlyList<VistaDto> Vistas { get; init; } = Array.Empty<VistaDto>();
    public IReadOnlyList<ExchangeRateDto> ExchangeRates { get; init; } = Array.Empty<ExchangeRateDto>();
}
