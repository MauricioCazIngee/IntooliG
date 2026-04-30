using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class FuenteRepository : IFuenteRepository
{
    private readonly AppDbContext _db;

    public FuenteRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<CatFuente> Items, int Total)> ListAsync(string? search, int? paisId, int page, int pageSize, bool? soloActivos, CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var q = _db.CatFuentes.AsNoTracking().AsQueryable();
        if (soloActivos == true) q = q.Where(x => x.Activo);
        if (paisId is int pid && pid > 0) q = q.Where(x => x.PaisId == pid);
        if (!string.IsNullOrWhiteSpace(term)) q = q.Where(x => x.NombreFuente.Contains(term));

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .Include(x => x.Pais)
            .OrderBy(x => x.NombreFuente)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<CatFuente?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.CatFuentes.AsNoTracking().Include(x => x.Pais).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CatFuente> AddAsync(CatFuente entity, CancellationToken cancellationToken = default)
    {
        _db.CatFuentes.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatFuente entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatFuentes.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
        if (tracked is null) throw new InvalidOperationException("Fuente no encontrada.");
        tracked.NombreFuente = entity.NombreFuente;
        tracked.PaisId = entity.PaisId;
        tracked.Activo = entity.Activo;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatFuentes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (tracked is null) return false;
        tracked.Activo = false;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<(int Id, string Nombre, bool Activo)>> ListActivosLookupAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _db.CatFuentes.AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.NombreFuente)
            .Select(x => new { x.Id, x.NombreFuente, x.Activo })
            .ToListAsync(cancellationToken);
        return rows.Select(x => (x.Id, x.NombreFuente, x.Activo)).ToList();
    }

    public async Task<IReadOnlyList<CatFuente>> ListByPaisAsync(int paisId, CancellationToken cancellationToken = default)
    {
        return await _db.CatFuentes.AsNoTracking()
            .Include(x => x.Pais)
            .Where(x => x.PaisId == paisId && x.Activo)
            .OrderBy(x => x.NombreFuente)
            .ToListAsync(cancellationToken);
    }
}
