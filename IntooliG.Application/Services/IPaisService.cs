using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;

namespace IntooliG.Application.Services;

public interface IPaisService
{
    Task<PagedListResult<PaisDto>> ListAsync(
        string? search,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default);

    Task<PaisDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PaisDto> CreateAsync(PaisCreateRequest request, CancellationToken cancellationToken = default);

    Task<PaisDto?> UpdateAsync(int id, PaisUpdateRequest request, CancellationToken cancellationToken = default);

    Task<bool> DesactivarAsync(int id, CancellationToken cancellationToken = default);
}
