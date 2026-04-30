using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;

namespace IntooliG.Application.Services;

public interface IRegionService
{
    Task<PagedListResult<RegionDto>> ListAsync(
        int clienteId,
        string? search,
        int? paisId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default);

    Task<RegionDto?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default);

    Task<RegionDto> CreateAsync(int clienteId, RegionCreateRequest request, CancellationToken cancellationToken = default);

    Task<RegionDto?> UpdateAsync(int id, int clienteId, RegionUpdateRequest request, CancellationToken cancellationToken = default);

    Task<bool> DesactivarAsync(int id, int clienteId, CancellationToken cancellationToken = default);
}
