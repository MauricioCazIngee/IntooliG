using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class PaisRepository : IPaisRepository
{
    private readonly AppDbContext _db;

    public PaisRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<CatPais> Items, int Total)> ListAsync(
        string? search,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var q = _db.CatPaises.AsNoTracking().AsQueryable();
        if (soloActivos == true)
            q = q.Where(p => p.Activo);
        if (!string.IsNullOrEmpty(term))
            q = q.Where(p => p.NombrePais.Contains(term));

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderBy(p => p.NombrePais)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task<CatPais?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.CatPaises.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<CatPais> AddAsync(CatPais entity, CancellationToken cancellationToken = default)
    {
        _db.CatPaises.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatPais entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatPaises.FirstOrDefaultAsync(p => p.Id == entity.Id, cancellationToken);
        if (tracked is null)
            throw new InvalidOperationException("País no encontrado.");
        tracked.NombrePais = entity.NombrePais;
        tracked.Activo = entity.Activo;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatPaises.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (tracked is null)
            return false;
        tracked.Activo = false;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
