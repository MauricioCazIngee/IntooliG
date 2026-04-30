using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RubrosConceptos;

namespace IntooliG.Application.Services;

public interface IRubroService
{
    Task<PagedListResult<RubroCombinacionDto>> ListAsync(
        int clienteId,
        int? categoriaId,
        bool? activo,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<RubroCombinacionDto?> GetByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default);
    Task<RubroCombinacionDto> CreateAsync(int clienteId, RubroCombinacionCreateRequest request, CancellationToken cancellationToken = default);
    Task<RubroCombinacionDto?> UpdateAsync(long id, int clienteId, RubroCombinacionUpdateRequest request, CancellationToken cancellationToken = default);
    Task<(bool Eliminado, bool Conflicto)> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default);
}
