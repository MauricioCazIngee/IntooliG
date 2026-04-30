using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RubrosConceptos;

namespace IntooliG.Application.Services;

public interface IConceptoService
{
    Task<PagedListResult<ConceptoListItemDto>> ListAsync(
        int clienteId,
        int? categoriaId,
        int? rubroGeneralId,
        bool? activo,
        bool? top,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ConceptoDetailDto?> GetByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default);
    Task<ConceptoDetailDto> CreateAsync(int clienteId, ConceptoCreateRequest request, CancellationToken cancellationToken = default);
    Task<ConceptoDetailDto?> UpdateAsync(long id, int clienteId, ConceptoUpdateRequest request, CancellationToken cancellationToken = default);
    Task<(bool Eliminado, bool Conflicto)> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default);
}
