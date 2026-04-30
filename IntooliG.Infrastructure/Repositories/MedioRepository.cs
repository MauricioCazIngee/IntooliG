using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class MedioRepository : IMedioRepository
{
    private readonly AppDbContext _db;

    public MedioRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<CatMedio> Items, int Total)> ListAsync(
        string? search,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var q = _db.CatMedios.AsNoTracking().AsQueryable();
        if (soloActivos == true)
            q = q.Where(m => m.Activo);
        if (!string.IsNullOrEmpty(term))
            q = q.Where(m =>
                m.NombreMedio.Contains(term)
                || m.NombreMedioGenerico.Contains(term));

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderBy(m => m.NombreMedio)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task<CatMedio?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.CatMedios.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<CatMedio> AddAsync(CatMedio entity, CancellationToken cancellationToken = default)
    {
        _db.CatMedios.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatMedio entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatMedios.FirstOrDefaultAsync(m => m.Id == entity.Id, cancellationToken);
        if (tracked is null)
            throw new InvalidOperationException("Medio no encontrado.");
        tracked.NombreMedio = entity.NombreMedio;
        tracked.NombreMedioGenerico = entity.NombreMedioGenerico;
        tracked.Activo = entity.Activo;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatMedios.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (tracked is null)
            return false;
        tracked.Activo = false;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<(int Id, string Nombre)>> ListActivosLookupAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _db.CatMedios.AsNoTracking()
            .Where(m => m.Activo)
            .OrderBy(m => m.NombreMedio)
            .Select(m => new { m.Id, m.NombreMedio })
            .ToListAsync(cancellationToken);
        return rows.Select(x => (x.Id, x.NombreMedio)).ToList();
    }
}
