using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class BURepository : IBURepository
{
    private readonly AppDbContext _db;

    public BURepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<(CatBU Bu, string SectorNombre)> Items, int Total)> ListAsync(
        int clienteId,
        int? sectorId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var q =
            from b in _db.CatBUs.AsNoTracking()
            join sec in _db.CatSectors.AsNoTracking() on b.SectorId equals sec.Id
            where b.ClienteId == clienteId
                  && (sectorId == null || b.SectorId == sectorId)
                  && (term == null || term == string.Empty || b.NombreBU.Contains(term))
            select new { b, sec.NombreSector };

        var total = await q.CountAsync(cancellationToken);
        var pageRows = await q
            .OrderBy(x => x.b.NombreBU)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var rows = pageRows.Select(x => (x.b, x.NombreSector)).ToList();
        return (rows, total);
    }

    public async Task<(CatBU? Bu, string? SectorNombre)> GetByIdAsync(
        int id,
        int clienteId,
        CancellationToken cancellationToken = default)
    {
        var row = await (
            from b in _db.CatBUs.AsNoTracking()
            join sec in _db.CatSectors.AsNoTracking() on b.SectorId equals sec.Id
            where b.Id == id && b.ClienteId == clienteId
            select new { b, sec.NombreSector }).FirstOrDefaultAsync(cancellationToken);

        return row is null ? (null, null) : (row.b, row.NombreSector);
    }

    public async Task<CatBU> AddAsync(CatBU entity, CancellationToken cancellationToken = default)
    {
        _db.CatBUs.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatBU entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatBUs.FirstOrDefaultAsync(
            b => b.Id == entity.Id && b.ClienteId == entity.ClienteId,
            cancellationToken);
        if (tracked is null)
            throw new InvalidOperationException("BU no encontrado.");

        tracked.NombreBU = entity.NombreBU;
        tracked.SectorId = entity.SectorId;
        tracked.Activo = entity.Activo;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        var entity = await _db.CatBUs.FirstOrDefaultAsync(b => b.Id == id && b.ClienteId == clienteId, cancellationToken);
        if (entity is null)
            return false;

        _db.CatBUs.Remove(entity);
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
