using IntooliG.Application.Features.Radar.Dtos;

namespace IntooliG.Application.Services;

public interface IGenerarEvaluacionService
{
    Task<(List<CampaniaEvaluacionDto> Items, int Total)> GetCampaniasEvaluacionAsync(
        int? anio = null,
        int? categoriaId = null,
        bool? activo = null,
        string? search = null,
        int page = 1,
        int pageSize = 10);

    Task<(List<AdministracionCampaniaDto> Items, int Total)> GetAdministracionCampaniasAsync(
        int? anio = null,
        int? sectorId = null,
        string? search = null,
        int page = 1,
        int pageSize = 10);
}
