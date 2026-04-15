using IntooliG.Application.Features.Radar;
using IntooliG.Application.Features.Radar.Dtos;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IRadarRepository
{
    Task<IReadOnlyList<RadarEstadoDto>> GetEstadosAsync(
        int anio,
        int semana,
        int categoria,
        string marca,
        int pais,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RadarInversionMarcaMediosDto>> GetInversionMarcaMediosAsync(
        int anio,
        int semana,
        int categoria,
        string marca,
        int pais,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RadarInversionMarcaMediosVersionDto>> GetInversionMarcaMediosVersionAsync(
        int anio,
        int semana,
        int categoria,
        string marca,
        string medio,
        int pais,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RadarMarcaFactoresRiesgoDto>> GetMarcaFactoresRiesgoAsync(
        int anio,
        int semana,
        int categoria,
        int rubro,
        string marca,
        int pais,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RadarMarcaValoresAgregadosDto>> GetMarcaValoresAgregadosAsync(
        int anio,
        int semana,
        int categoria,
        string marca,
        int pais,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RadarMrcaWeeksDto>> GetMrcaWeeksAsync(
        int anio,
        int semana,
        int categoria,
        int pais,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RadarTop5Dto>> GetTop5Async(
        int anio,
        int semana,
        int categoria,
        CancellationToken cancellationToken = default);
}
