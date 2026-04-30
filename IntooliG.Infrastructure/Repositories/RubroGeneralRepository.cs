using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class RubroGeneralRepository : IRubroGeneralRepository
{
    private readonly AppDbContext _db;

    public RubroGeneralRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<CatRubroGeneral> Items, int Total)> ListAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();
        var q = _db.CatRubroGenerales.AsNoTracking()
            .Where(r => term == null || term == string.Empty || r.NombreRubro.Contains(term));
        var total = await q.CountAsync(cancellationToken);
        var items = await q.OrderBy(r => r.NombreRubro)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task<CatRubroGeneral?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.CatRubroGenerales.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<CatRubroGeneral> AddAsync(CatRubroGeneral entity, CancellationToken cancellationToken = default)
    {
        _db.CatRubroGenerales.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatRubroGeneral entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatRubroGenerales.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
        if (tracked is null)
            throw new InvalidOperationException("Rubro general no encontrado.");
        tracked.NombreRubro = entity.NombreRubro;
        tracked.Activo = entity.Activo;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<(bool Eliminado, bool Conflicto)> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.CatRubroGenerales.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return (false, false);

        _db.CatRubroGenerales.Remove(entity);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return (true, false);
        }
        catch (DbUpdateException)
        {
            return (false, true);
        }
    }
}
