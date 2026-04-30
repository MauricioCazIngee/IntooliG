using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IFuenteRepository
{
    Task<(IReadOnlyList<CatFuente> Items, int Total)> ListAsync(
        string? search,
        int? paisId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default);

    Task<CatFuente?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CatFuente> AddAsync(CatFuente entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(CatFuente entity, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<(int Id, string Nombre, bool Activo)>> ListActivosLookupAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CatFuente>> ListByPaisAsync(int paisId, CancellationToken cancellationToken = default);
}
