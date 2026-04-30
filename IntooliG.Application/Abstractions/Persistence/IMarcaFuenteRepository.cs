using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IMarcaFuenteRepository
{
    Task<(IReadOnlyList<CatMarcaProductoFuente> Items, int Total)> ListAsync(
        int clienteId,
        string? search,
        int? fuenteId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<CatMarcaProductoFuente?> GetByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default);
    Task<CatMarcaProductoFuente> AddAsync(CatMarcaProductoFuente entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(CatMarcaProductoFuente entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CatMarcaProductoFuente>> ListByFuenteAsync(int fuenteId, int clienteId, CancellationToken cancellationToken = default);
}
