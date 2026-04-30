using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IPaisRepository
{
    Task<(IReadOnlyList<CatPais> Items, int Total)> ListAsync(
        string? search,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default);

    Task<CatPais?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<CatPais> AddAsync(CatPais entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(CatPais entity, CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}
