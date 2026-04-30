using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface ISectorRepository
{
    Task<(IReadOnlyList<(CatSector Sector, string ClienteNombre)> Items, int Total)> ListAsync(
        int clienteId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<(CatSector? Sector, string? ClienteNombre)> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default);

    Task<CatSector> AddAsync(CatSector entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(CatSector entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default);
}
