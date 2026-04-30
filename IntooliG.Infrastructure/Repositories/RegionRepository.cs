using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class RegionRepository : IRegionRepository
{
    private readonly AppDbContext _db;

    public RegionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<(CatRegion Region, string NombrePais)> Items, int Total)> ListAsync(
        int clienteId,
        string? search,
        int? paisId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var q =
            from r in _db.CatRegiones.AsNoTracking()
            join p in _db.CatPaises.AsNoTracking() on r.PaisId equals p.Id
            where r.ClienteId == clienteId
                  && (soloActivos != true || r.Activo)
                  && (paisId == null || r.PaisId == paisId)
                  && (term == null || term == string.Empty
                      || r.NombreRegion.Contains(term)
                      || p.NombrePais.Contains(term))
            select new { r, p.NombrePais };

        var total = await q.CountAsync(cancellationToken);
        var pageRows = await q
            .OrderBy(x => x.NombrePais)
            .ThenBy(x => x.r.NombreRegion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new { x.r, x.NombrePais })
            .ToListAsync(cancellationToken);

        var items = pageRows.Select(x => (x.r, x.NombrePais)).ToList();
        return (items, total);
    }

    public async Task<(CatRegion? Region, string? NombrePais)> GetByIdAsync(
        int id,
        int clienteId,
        CancellationToken cancellationToken = default)
    {
        var row = await (
            from r in _db.CatRegiones.AsNoTracking()
            join p in _db.CatPaises.AsNoTracking() on r.PaisId equals p.Id
            where r.Id == id && r.ClienteId == clienteId
            select new { r, p.NombrePais }).FirstOrDefaultAsync(cancellationToken);

        return row is null ? (null, null) : (row.r, row.NombrePais);
    }

    public async Task<IReadOnlyList<int>> GetCiudadIdsByRegionAsync(int regionId, CancellationToken cancellationToken = default)
    {
        return await _db.AgrupacionRegiones.AsNoTracking()
            .Where(a => a.RegionId == regionId)
            .OrderBy(a => a.CiudadId)
            .Select(a => a.CiudadId)
            .ToListAsync(cancellationToken);
    }

    public async Task<string> GetCiudadesResumenAsync(int regionId, CancellationToken cancellationToken = default)
    {
        var names = await (
            from a in _db.AgrupacionRegiones.AsNoTracking()
            join c in _db.CatCiudades.AsNoTracking() on a.CiudadId equals c.Id
            where a.RegionId == regionId
            orderby c.NombreCiudad
            select c.NombreCiudad).ToListAsync(cancellationToken);

        return string.Join(" / ", names);
    }

    public async Task<CatRegion> AddAsync(CatRegion entity, CancellationToken cancellationToken = default)
    {
        _db.CatRegiones.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<CatRegion> CreateWithCiudadesAsync(
        CatRegion entity,
        IReadOnlyList<int> ciudadIds,
        CancellationToken cancellationToken = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            _db.CatRegiones.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);
            foreach (var cid in ciudadIds.Distinct())
            {
                _db.AgrupacionRegiones.Add(new AgrupacionRegion
                {
                    RegionId = entity.Id,
                    CiudadId = cid
                });
            }

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return entity;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateAsync(CatRegion entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatRegiones.FirstOrDefaultAsync(
            r => r.Id == entity.Id && r.ClienteId == entity.ClienteId,
            cancellationToken);
        if (tracked is null)
            throw new InvalidOperationException("Región no encontrada.");
        tracked.PaisId = entity.PaisId;
        tracked.NombreRegion = entity.NombreRegion;
        tracked.EsNacional = entity.EsNacional;
        tracked.Activo = entity.Activo;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceCiudadesAsync(int regionId, IReadOnlyList<int> ciudadIds, CancellationToken cancellationToken = default)
    {
        var existing = await _db.AgrupacionRegiones
            .Where(x => x.RegionId == regionId)
            .ToListAsync(cancellationToken);
        _db.AgrupacionRegiones.RemoveRange(existing);
        foreach (var cid in ciudadIds.Distinct())
        {
            _db.AgrupacionRegiones.Add(new AgrupacionRegion
            {
                RegionId = regionId,
                CiudadId = cid
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateWithCiudadesAsync(
        CatRegion entity,
        IReadOnlyList<int> ciudadIds,
        CancellationToken cancellationToken = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await UpdateAsync(entity, cancellationToken);
            await ReplaceCiudadesAsync(entity.Id, ciudadIds, cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> SoftDeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatRegiones.FirstOrDefaultAsync(
            r => r.Id == id && r.ClienteId == clienteId,
            cancellationToken);
        if (tracked is null)
            return false;
        tracked.Activo = false;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
