using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IBURepository
{
    Task<(IReadOnlyList<(CatBU Bu, string SectorNombre)> Items, int Total)> ListAsync(
        int clienteId,
        int? sectorId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<(CatBU? Bu, string? SectorNombre)> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default);

    Task<CatBU> AddAsync(CatBU entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(CatBU entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default);
}
