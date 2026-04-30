using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface ICategoriaRepository
{
    Task<(IReadOnlyList<CatCategoria> Items, int Total)> ListAsync(
        int clienteId,
        int? buId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<CatCategoria?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default);

    Task<CatCategoria> AddAsync(CatCategoria entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(CatCategoria entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default);
}
