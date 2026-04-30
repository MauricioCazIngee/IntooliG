using IntooliG.Application.Features.Radar.Dtos;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IGenerarEvaluacionRepository
{
    // Listado principal "Campaña Alta-Baja-Cambios"
    Task<List<CampaniaEvaluacionDto>> GetCampaniasEvaluacionAsync(
        int? anio = null,
        int? categoriaId = null,
        bool? activo = null,
        string? search = null,
        int page = 1,
        int pageSize = 10);

    Task<int> GetCampaniasEvaluacionCountAsync(
        int? anio = null,
        int? categoriaId = null,
        bool? activo = null,
        string? search = null);

    // Administración de Campañas
    Task<List<AdministracionCampaniaDto>> GetAdministracionCampaniasAsync(
        int? anio = null,
        int? sectorId = null,
        string? search = null,
        int page = 1,
        int pageSize = 10);

    Task<int> GetAdministracionCampaniasCountAsync(
        int? anio = null,
        int? sectorId = null,
        string? search = null);
}
