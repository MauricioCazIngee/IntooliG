using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Fuentes;

namespace IntooliG.Application.Services;

public interface IFuenteService
{
    Task<PagedListResult<FuenteDto>> ListAsync(
        string? search,
        int? paisId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default);

    Task<FuenteDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FuenteDto> CreateAsync(FuenteCreateRequest request, CancellationToken cancellationToken = default);
    Task<FuenteDto?> UpdateAsync(int id, FuenteUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DesactivarAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FuenteLookupDto>> ListActivosLookupAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FuenteDto>> ListByPaisAsync(int paisId, CancellationToken cancellationToken = default);
}
