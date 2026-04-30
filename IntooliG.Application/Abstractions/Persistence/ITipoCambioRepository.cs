using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface ITipoCambioRepository
{
    Task<(IReadOnlyList<CatTipoCambio> Items, int Total)> ListAsync(
        string? search,
        int? paisId,
        int? anio,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<CatTipoCambio?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<CatTipoCambio> AddAsync(CatTipoCambio entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(CatTipoCambio entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int paisId, int anio, int mes, int? excludeId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> ListAniosAsync(CancellationToken cancellationToken = default);
}
