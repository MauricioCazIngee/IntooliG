using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IMedioRepository
{
    Task<(IReadOnlyList<CatMedio> Items, int Total)> ListAsync(
        string? search,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default);

    Task<CatMedio?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<CatMedio> AddAsync(CatMedio entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(CatMedio entity, CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Medios activos para selectores.</summary>
    Task<IReadOnlyList<(int Id, string Nombre)>> ListActivosLookupAsync(CancellationToken cancellationToken = default);
}
