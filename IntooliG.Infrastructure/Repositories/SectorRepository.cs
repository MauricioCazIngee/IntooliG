using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class SectorRepository : ISectorRepository
{
    private readonly AppDbContext _db;

    public SectorRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<(CatSector Sector, string ClienteNombre)> Items, int Total)> ListAsync(
        int clienteId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var q =
            from s in _db.CatSectors.AsNoTracking()
            join c in _db.CatClientes.AsNoTracking() on s.ClienteId equals c.Id
            where s.ClienteId == clienteId
                  && (term == null || term == string.Empty || s.NombreSector.Contains(term))
            select new { s, c.Nombre };

        var total = await q.CountAsync(cancellationToken);
        var pageRows = await q
            .OrderBy(x => x.s.NombreSector)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new { x.s, x.Nombre })
            .ToListAsync(cancellationToken);

        var rows = pageRows.Select(x => (x.s, x.Nombre)).ToList();
        return (rows, total);
    }

    public async Task<(CatSector? Sector, string? ClienteNombre)> GetByIdAsync(
        int id,
        int clienteId,
        CancellationToken cancellationToken = default)
    {
        var row = await (
            from s in _db.CatSectors.AsNoTracking()
            join c in _db.CatClientes.AsNoTracking() on s.ClienteId equals c.Id
            where s.Id == id && s.ClienteId == clienteId
            select new { s, c.Nombre }).FirstOrDefaultAsync(cancellationToken);

        return row is null ? (null, null) : (row.s, row.Nombre);
    }

    public async Task<CatSector> AddAsync(CatSector entity, CancellationToken cancellationToken = default)
    {
        _db.CatSectors.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatSector entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatSectors.FirstOrDefaultAsync(
            s => s.Id == entity.Id && s.ClienteId == entity.ClienteId,
            cancellationToken);
        if (tracked is null)
            throw new InvalidOperationException("Sector no encontrado.");

        tracked.NombreSector = entity.NombreSector;
        tracked.Activo = entity.Activo;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        var entity = await _db.CatSectors.FirstOrDefaultAsync(s => s.Id == id && s.ClienteId == clienteId, cancellationToken);
        if (entity is null)
            return false;

        _db.CatSectors.Remove(entity);
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
