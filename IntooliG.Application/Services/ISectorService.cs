using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Catalogo;

namespace IntooliG.Application.Services;

public interface ISectorService
{
    Task<PagedListResult<SectorDto>> ListAsync(int clienteId, string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<SectorDto?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default);
    Task<SectorDto> CreateAsync(int clienteId, SectorCreateRequest request, CancellationToken cancellationToken = default);
    Task<SectorDto?> UpdateAsync(int id, int clienteId, SectorUpdateRequest request, CancellationToken cancellationToken = default);
    Task<(bool Eliminado, bool Conflicto)> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default);
}
