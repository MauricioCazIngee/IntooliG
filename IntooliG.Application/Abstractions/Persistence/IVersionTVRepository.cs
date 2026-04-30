using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IVersionTVRepository
{
    Task<IReadOnlyList<CatVersionTV>> ListActivosAsync(CancellationToken cancellationToken = default);
    Task<CatVersionTV?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}
