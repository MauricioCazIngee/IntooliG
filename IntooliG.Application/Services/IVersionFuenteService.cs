using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Fuentes;

namespace IntooliG.Application.Services;

public interface IVersionFuenteService
{
    Task<PagedListResult<VersionFuenteDto>> ListAsync(
        int clienteId,
        string? search,
        int? fuenteId,
        int? categoriaId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default);

    Task<VersionFuenteDto?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default);
    Task<VersionFuenteDto> CreateAsync(int clienteId, VersionFuenteCreateRequest request, CancellationToken cancellationToken = default);
    Task<VersionFuenteDto?> UpdateAsync(int id, int clienteId, VersionFuenteUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DesactivarAsync(int id, int clienteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VersionFuenteDto>> ListByFuenteAsync(int fuenteId, int clienteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VersionTVLookupDto>> ListVersionTVLookupAsync(CancellationToken cancellationToken = default);
}
