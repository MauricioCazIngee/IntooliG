using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Features.Radar;
using IntooliG.Application.Features.Radar.Dtos;

namespace IntooliG.Application.Services;

public class RadarService : IRadarService
{
    private readonly IRadarRepository _radar;

    public RadarService(IRadarRepository radar)
    {
        _radar = radar;
    }

    public Task<IReadOnlyList<RadarEstadoDto>> GetEstadosAsync(
        int anio,
        int semana,
        int categoria,
        string marca,
        int pais,
        CancellationToken cancellationToken = default)
    {
        var marcaNormalizada = string.IsNullOrWhiteSpace(marca) ? string.Empty : marca.Trim();
        return _radar.GetEstadosAsync(anio, semana, categoria, marcaNormalizada, pais, cancellationToken);
    }

    public Task<IReadOnlyList<RadarInversionMarcaMediosDto>> GetInversionMarcaMediosAsync(
        int anio,
        int semana,
        int categoria,
        string marca,
        int pais,
        CancellationToken cancellationToken = default)
    {
        var marcaNormalizada = string.IsNullOrWhiteSpace(marca) ? string.Empty : marca.Trim();
        return _radar.GetInversionMarcaMediosAsync(anio, semana, categoria, marcaNormalizada, pais, cancellationToken);
    }

    public Task<IReadOnlyList<RadarInversionMarcaMediosVersionDto>> GetInversionMarcaMediosVersionAsync(
        int anio,
        int semana,
        int categoria,
        string marca,
        string medio,
        int pais,
        CancellationToken cancellationToken = default)
    {
        var marcaNormalizada = string.IsNullOrWhiteSpace(marca) ? string.Empty : marca.Trim();
        var medioNormalizado = string.IsNullOrWhiteSpace(medio) ? string.Empty : medio.Trim();
        return _radar.GetInversionMarcaMediosVersionAsync(anio, semana, categoria, marcaNormalizada, medioNormalizado, pais, cancellationToken);
    }

    public Task<IReadOnlyList<RadarMarcaFactoresRiesgoDto>> GetMarcaFactoresRiesgoAsync(
        int anio,
        int semana,
        int categoria,
        int rubro,
        string marca,
        int pais,
        CancellationToken cancellationToken = default)
    {
        var marcaNormalizada = string.IsNullOrWhiteSpace(marca) ? string.Empty : marca.Trim();
        return _radar.GetMarcaFactoresRiesgoAsync(anio, semana, categoria, rubro, marcaNormalizada, pais, cancellationToken);
    }

    public Task<IReadOnlyList<RadarMarcaValoresAgregadosDto>> GetMarcaValoresAgregadosAsync(
        int anio,
        int semana,
        int categoria,
        string marca,
        int pais,
        CancellationToken cancellationToken = default)
    {
        var marcaNormalizada = string.IsNullOrWhiteSpace(marca) ? string.Empty : marca.Trim();
        return _radar.GetMarcaValoresAgregadosAsync(anio, semana, categoria, marcaNormalizada, pais, cancellationToken);
    }

    public Task<IReadOnlyList<RadarMrcaWeeksDto>> GetMrcaWeeksAsync(
        int anio,
        int semana,
        int categoria,
        int pais,
        CancellationToken cancellationToken = default) =>
        _radar.GetMrcaWeeksAsync(anio, semana, categoria, pais, cancellationToken);

    public Task<IReadOnlyList<RadarTop5Dto>> GetTop5Async(
        int anio,
        int semana,
        int categoria,
        CancellationToken cancellationToken = default) =>
        _radar.GetTop5Async(anio, semana, categoria, cancellationToken);
}
