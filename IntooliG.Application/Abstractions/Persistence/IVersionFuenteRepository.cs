using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IVersionFuenteRepository
{
    Task<(IReadOnlyList<CatVersionFuente> Items, int Total)> ListAsync(
        int clienteId,
        string? search,
        int? fuenteId,
        int? categoriaId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default);

    Task<CatVersionFuente?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default);
    Task<CatVersionFuente> AddAsync(CatVersionFuente entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(CatVersionFuente entity, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CatVersionFuente>> ListByFuenteAsync(int fuenteId, int clienteId, CancellationToken cancellationToken = default);
}
