using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _db;

    public UsuarioRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Usuario>> ListAsync(CancellationToken cancellationToken = default) =>
        await _db.Usuarios.AsNoTracking().OrderBy(u => u.Nombre).ToListAsync(cancellationToken);

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _db.Usuarios.AnyAsync(u =>
            u.Email.ToLower() == normalized &&
            (!excludeId.HasValue || u.Id != excludeId.Value), cancellationToken);
    }

    public async Task<Usuario> AddAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync(cancellationToken);
        return usuario;
    }

    public async Task UpdateAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        _db.Usuarios.Update(usuario);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (entity is null)
            return false;

        _db.Usuarios.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
