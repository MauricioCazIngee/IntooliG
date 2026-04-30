using IntooliG.Application.Features.RubrosConceptos;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IRubroRepository
{
    Task<(IReadOnlyList<RubroListRow> Items, int Total)> ListAsync(
        int clienteId,
        int? categoriaId,
        bool? activo,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<CatRubro?> GetByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default);

    Task<RubroListRow?> GetListRowByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default);
    Task<bool> ExistsCombinacionAsync(int rubroGeneralId, int categoriaId, int clienteId, long? exceptId, CancellationToken cancellationToken = default);
    Task<CatRubro> AddAsync(CatRubro entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(CatRubro entity, CancellationToken cancellationToken = default);
    Task<(bool Eliminado, bool Conflicto)> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default);
}
