using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface ICampaniaRepository
{
    Task<IReadOnlyList<Campania>> ListAsync(CancellationToken cancellationToken = default);
    Task<Campania?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Campania> AddAsync(Campania campania, CancellationToken cancellationToken = default);
    Task UpdateAsync(Campania campania, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
