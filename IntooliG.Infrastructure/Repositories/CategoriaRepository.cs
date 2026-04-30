using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _db;

    public CategoriaRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<CatCategoria> Items, int Total)> ListAsync(
        int clienteId,
        int? buId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        // tbCatCategoria no tiene FiBUid en esta BD; buId se ignora hasta exista la columna o tabla puente.
        var q = _db.CatCategorias.AsNoTracking()
            .Where(c => c.ClienteId == clienteId
                        && (term == null || term == string.Empty || c.NombreCategoria.Contains(term)));

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderBy(c => c.NombreCategoria)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<CatCategoria?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default) =>
        _db.CatCategorias.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.ClienteId == clienteId, cancellationToken);

    public async Task<CatCategoria> AddAsync(CatCategoria entity, CancellationToken cancellationToken = default)
    {
        _db.CatCategorias.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatCategoria entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatCategorias.FirstOrDefaultAsync(
            c => c.Id == entity.Id && c.ClienteId == entity.ClienteId,
            cancellationToken);
        if (tracked is null)
            throw new InvalidOperationException("Categoría no encontrada.");

        tracked.NombreCategoria = entity.NombreCategoria;
        tracked.NombreCorto = entity.NombreCorto;
        tracked.Activo = entity.Activo;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        var entity = await _db.CatCategorias.FirstOrDefaultAsync(c => c.Id == id && c.ClienteId == clienteId, cancellationToken);
        if (entity is null)
            return false;

        _db.CatCategorias.Remove(entity);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }
}
