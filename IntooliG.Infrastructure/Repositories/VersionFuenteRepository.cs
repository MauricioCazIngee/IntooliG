using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class VersionFuenteRepository : IVersionFuenteRepository
{
    private readonly AppDbContext _db;

    public VersionFuenteRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<CatVersionFuente> Items, int Total)> ListAsync(int clienteId, string? search, int? fuenteId, int? categoriaId, int page, int pageSize, bool? soloActivos, CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var baseQuery =
            from vf in _db.CatVersionFuentes.AsNoTracking()
            join c in _db.CatCategorias.AsNoTracking() on vf.CategoriaId equals c.Id
            join bu in _db.CatBUs.AsNoTracking() on vf.BUId equals bu.Id
            join f in _db.CatFuentes.AsNoTracking() on vf.FuenteId equals f.Id
            where c.ClienteId == clienteId && bu.ClienteId == clienteId
            select new { vf, c, f };

        if (fuenteId is int fid && fid > 0) baseQuery = baseQuery.Where(x => x.vf.FuenteId == fid);
        if (categoriaId is int cid && cid > 0) baseQuery = baseQuery.Where(x => x.vf.CategoriaId == cid);
        if (soloActivos == true) baseQuery = baseQuery.Where(x => x.vf.Activo);
        if (!string.IsNullOrWhiteSpace(term))
            baseQuery = baseQuery.Where(x => x.vf.NombreVersionFuente.Contains(term) || x.f.NombreFuente.Contains(term) || x.c.NombreCategoria.Contains(term));

        var total = await baseQuery.CountAsync(cancellationToken);
        var ids = await baseQuery.OrderBy(x => x.vf.NombreVersionFuente)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.vf.Id)
            .ToListAsync(cancellationToken);

        if (ids.Count == 0) return (Array.Empty<CatVersionFuente>(), total);

        var items = await _db.CatVersionFuentes.AsNoTracking()
            .Include(x => x.Fuente)
            .Include(x => x.Categoria)
            .Include(x => x.BU)
            .Include(x => x.VersionTV)
            .Include(x => x.Producto!).ThenInclude(p => p.Marca)
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var order = ids.Select((id, idx) => (id, idx)).ToDictionary(x => x.id, x => x.idx);
        return (items.OrderBy(x => order[x.Id]).ToList(), total);
    }

    public async Task<CatVersionFuente?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        var row = await _db.CatVersionFuentes.AsNoTracking()
            .Include(x => x.Fuente)
            .Include(x => x.Categoria)
            .Include(x => x.BU)
            .Include(x => x.VersionTV)
            .Include(x => x.Producto!).ThenInclude(p => p.Marca)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (row?.Categoria is null || row.Categoria.ClienteId != clienteId) return null;
        if (row.BU is null || row.BU.ClienteId != clienteId) return null;
        return row;
    }

    public async Task<CatVersionFuente> AddAsync(CatVersionFuente entity, CancellationToken cancellationToken = default)
    {
        _db.CatVersionFuentes.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatVersionFuente entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatVersionFuentes.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
        if (tracked is null) throw new InvalidOperationException("Versión fuente no encontrada.");
        tracked.FuenteId = entity.FuenteId;
        tracked.NombreVersionFuente = entity.NombreVersionFuente;
        tracked.Activo = entity.Activo;
        tracked.ProductoId = entity.ProductoId;
        tracked.VersionTVId = entity.VersionTVId;
        tracked.BUId = entity.BUId;
        tracked.CategoriaId = entity.CategoriaId;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatVersionFuentes
            .Include(x => x.Categoria)
            .Include(x => x.BU)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (tracked?.Categoria is null || tracked.Categoria.ClienteId != clienteId) return false;
        if (tracked.BU is null || tracked.BU.ClienteId != clienteId) return false;
        tracked.Activo = false;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<CatVersionFuente>> ListByFuenteAsync(int fuenteId, int clienteId, CancellationToken cancellationToken = default)
    {
        var ids = await (
            from vf in _db.CatVersionFuentes.AsNoTracking()
            join c in _db.CatCategorias.AsNoTracking() on vf.CategoriaId equals c.Id
            join bu in _db.CatBUs.AsNoTracking() on vf.BUId equals bu.Id
            where vf.FuenteId == fuenteId && c.ClienteId == clienteId && bu.ClienteId == clienteId
            select vf.Id).ToListAsync(cancellationToken);

        if (ids.Count == 0) return Array.Empty<CatVersionFuente>();

        return await _db.CatVersionFuentes.AsNoTracking()
            .Include(x => x.Fuente)
            .Include(x => x.Categoria)
            .Include(x => x.BU)
            .Include(x => x.VersionTV)
            .Include(x => x.Producto!).ThenInclude(p => p.Marca)
            .Where(x => ids.Contains(x.Id))
            .OrderBy(x => x.NombreVersionFuente)
            .ToListAsync(cancellationToken);
    }
}
