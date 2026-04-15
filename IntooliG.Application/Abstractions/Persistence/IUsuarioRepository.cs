using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IUsuarioRepository
{
    Task<IReadOnlyList<Usuario>> ListAsync(CancellationToken cancellationToken = default);
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<Usuario> AddAsync(Usuario usuario, CancellationToken cancellationToken = default);
    Task UpdateAsync(Usuario usuario, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
