using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Catalogo;

namespace IntooliG.Application.Services;

public interface IBUService
{
    Task<PagedListResult<BUDto>> ListAsync(int clienteId, int? sectorId, string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<BUDto?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default);
    Task<BUDto> CreateAsync(int clienteId, BUCreateRequest request, CancellationToken cancellationToken = default);
    Task<BUDto?> UpdateAsync(int id, int clienteId, BUUpdateRequest request, CancellationToken cancellationToken = default);
    Task<(bool Eliminado, bool Conflicto)> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default);
}
