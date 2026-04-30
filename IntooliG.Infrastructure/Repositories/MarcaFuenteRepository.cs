using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class MarcaFuenteRepository : IMarcaFuenteRepository
{
    private readonly AppDbContext _db;

    public MarcaFuenteRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<CatMarcaProductoFuente> Items, int Total)> ListAsync(int clienteId, string? search, int? fuenteId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var baseQuery =
            from mf in _db.CatMarcaProductoFuentes.AsNoTracking()
            join m in _db.CatMarcas.AsNoTracking() on mf.MarcaId equals m.Id
            join f in _db.CatFuentes.AsNoTracking() on mf.FuenteId equals f.Id
            where m.ClienteId == clienteId
            select new { mf, m, f };

        if (fuenteId is int fid && fid > 0)
            baseQuery = baseQuery.Where(x => x.mf.FuenteId == fid);
        if (!string.IsNullOrWhiteSpace(term))
            baseQuery = baseQuery.Where(x => x.mf.NombreMarcaFuente.Contains(term) || x.f.NombreFuente.Contains(term) || x.m.NombreMarca.Contains(term));

        var total = await baseQuery.CountAsync(cancellationToken);
        var ids = await baseQuery.OrderBy(x => x.mf.NombreMarcaFuente)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.mf.Id)
            .ToListAsync(cancellationToken);

        if (ids.Count == 0) return (Array.Empty<CatMarcaProductoFuente>(), total);

        var items = await _db.CatMarcaProductoFuentes.AsNoTracking()
            .Include(x => x.Fuente)
            .Include(x => x.Marca)
            .Include(x => x.Producto)
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var order = ids.Select((id, idx) => (id, idx)).ToDictionary(x => x.id, x => x.idx);
        return (items.OrderBy(x => order[x.Id]).ToList(), total);
    }

    public async Task<CatMarcaProductoFuente?> GetByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default)
    {
        var row = await _db.CatMarcaProductoFuentes.AsNoTracking()
            .Include(x => x.Fuente)
            .Include(x => x.Marca)
            .Include(x => x.Producto)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (row?.Marca is null || row.Marca.ClienteId != clienteId) return null;
        return row;
    }

    public async Task<CatMarcaProductoFuente> AddAsync(CatMarcaProductoFuente entity, CancellationToken cancellationToken = default)
    {
        _db.CatMarcaProductoFuentes.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatMarcaProductoFuente entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatMarcaProductoFuentes.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
        if (tracked is null) throw new InvalidOperationException("Marca fuente no encontrada.");
        tracked.NombreMarcaFuente = entity.NombreMarcaFuente;
        tracked.NombreProductoFuente = entity.NombreProductoFuente;
        tracked.FuenteId = entity.FuenteId;
        tracked.MarcaId = entity.MarcaId;
        tracked.ProductoId = entity.ProductoId;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatMarcaProductoFuentes.Include(x => x.Marca).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (tracked?.Marca is null || tracked.Marca.ClienteId != clienteId) return false;
        _db.CatMarcaProductoFuentes.Remove(tracked);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<CatMarcaProductoFuente>> ListByFuenteAsync(int fuenteId, int clienteId, CancellationToken cancellationToken = default)
    {
        var ids = await (
            from mf in _db.CatMarcaProductoFuentes.AsNoTracking()
            join m in _db.CatMarcas.AsNoTracking() on mf.MarcaId equals m.Id
            where mf.FuenteId == fuenteId && m.ClienteId == clienteId
            select mf.Id).ToListAsync(cancellationToken);

        if (ids.Count == 0) return Array.Empty<CatMarcaProductoFuente>();

        return await _db.CatMarcaProductoFuentes.AsNoTracking()
            .Include(x => x.Fuente)
            .Include(x => x.Marca)
            .Include(x => x.Producto)
            .Where(x => ids.Contains(x.Id))
            .OrderBy(x => x.NombreMarcaFuente)
            .ToListAsync(cancellationToken);
    }
}
