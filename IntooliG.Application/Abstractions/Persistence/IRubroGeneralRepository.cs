using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IRubroGeneralRepository
{
    Task<(IReadOnlyList<CatRubroGeneral> Items, int Total)> ListAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<CatRubroGeneral?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CatRubroGeneral> AddAsync(CatRubroGeneral entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(CatRubroGeneral entity, CancellationToken cancellationToken = default);
    Task<(bool Eliminado, bool Conflicto)> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
