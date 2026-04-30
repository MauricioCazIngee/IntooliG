using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;

namespace IntooliG.Application.Services;

public interface IPoblacionService
{
    Task<PagedListResult<PoblacionDto>> ListAsync(
        string? search,
        int? paisId,
        int? estadoId,
        int? anio,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PoblacionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> GetAniosDisponiblesAsync(CancellationToken cancellationToken = default);

    Task<PoblacionDto> CreateAsync(PoblacionCreateRequest request, CancellationToken cancellationToken = default);

    Task<PoblacionDto?> UpdateAsync(int id, PoblacionUpdateRequest request, CancellationToken cancellationToken = default);

    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
}
