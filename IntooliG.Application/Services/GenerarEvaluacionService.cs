using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Features.Radar.Dtos;

namespace IntooliG.Application.Services;

public class GenerarEvaluacionService : IGenerarEvaluacionService
{
    private readonly IGenerarEvaluacionRepository _repository;

    public GenerarEvaluacionService(IGenerarEvaluacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<(List<CampaniaEvaluacionDto> Items, int Total)> GetCampaniasEvaluacionAsync(
        int? anio = null,
        int? categoriaId = null,
        bool? activo = null,
        string? search = null,
        int page = 1,
        int pageSize = 10)
    {
        // No paralelizar: el mismo DbContext/comparte una sola conexión; dos comandos a la vez dejan el estado "connecting".
        var items = await _repository.GetCampaniasEvaluacionAsync(anio, categoriaId, activo, search, page, pageSize);
        var total = await _repository.GetCampaniasEvaluacionCountAsync(anio, categoriaId, activo, search);
        return (items, total);
    }

    public async Task<(List<AdministracionCampaniaDto> Items, int Total)> GetAdministracionCampaniasAsync(
        int? anio = null,
        int? sectorId = null,
        string? search = null,
        int page = 1,
        int pageSize = 10)
    {
        var items = await _repository.GetAdministracionCampaniasAsync(anio, sectorId, search, page, pageSize);
        var total = await _repository.GetAdministracionCampaniasCountAsync(anio, sectorId, search);
        return (items, total);
    }
}
