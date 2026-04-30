using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class EstadoRepository : IEstadoRepository
{
    private readonly AppDbContext _db;

    public EstadoRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<(CatEstado Estado, string NombrePais, string? NombreCodigoMapa)> Items, int Total)> ListAsync(
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
            from e in _db.CatEstados.AsNoTracking()
            join p in _db.CatPaises.AsNoTracking() on e.PaisId equals p.Id
            join cm in _db.CatCodigoMapas.AsNoTracking() on e.CodigoMapaId equals cm.Id into cmJoin
            from cm in cmJoin.DefaultIfEmpty()
            where (soloActivos != true || e.Activo)
                  && (paisId == null || e.PaisId == paisId)
                  && (term == null || term == string.Empty
                      || e.NombreEstado.Contains(term)
                      || p.NombrePais.Contains(term))
            select new { e, p.NombrePais, Codigo = cm != null ? cm.NombreCodigoMapa : null };

        var total = await q.CountAsync(cancellationToken);
        var pageRows = await q
            .OrderBy(x => x.NombrePais)
            .ThenBy(x => x.e.NombreEstado)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = pageRows
            .Select(x => (x.e, x.NombrePais, (string?)x.Codigo))
            .ToList();
        return (items, total);
    }

    public async Task<(CatEstado? Estado, string? NombrePais, string? NombreCodigoMapa)> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var row = await (
            from e in _db.CatEstados.AsNoTracking()
            join p in _db.CatPaises.AsNoTracking() on e.PaisId equals p.Id
            join cm in _db.CatCodigoMapas.AsNoTracking() on e.CodigoMapaId equals cm.Id into cmJoin
            from cm in cmJoin.DefaultIfEmpty()
            where e.Id == id
            select new { e, p.NombrePais, Codigo = cm != null ? cm.NombreCodigoMapa : null }).FirstOrDefaultAsync(cancellationToken);

        return row is null ? (null, null, null) : (row.e, row.NombrePais, row.Codigo);
    }

    public async Task<CatEstado> AddAsync(CatEstado entity, CancellationToken cancellationToken = default)
    {
        _db.CatEstados.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatEstado entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatEstados.FirstOrDefaultAsync(e => e.Id == entity.Id, cancellationToken);
        if (tracked is null)
            throw new InvalidOperationException("Estado no encontrado.");
        tracked.PaisId = entity.PaisId;
        tracked.NombreEstado = entity.NombreEstado;
        tracked.CodigoMapaId = entity.CodigoMapaId;
        tracked.Activo = entity.Activo;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatEstados.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (tracked is null)
            return false;
        tracked.Activo = false;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<(int Id, string Nombre)>> ListCodigosMapaAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _db.CatCodigoMapas.AsNoTracking()
            .OrderBy(x => x.NombreCodigoMapa)
            .Select(x => new { x.Id, x.NombreCodigoMapa })
            .ToListAsync(cancellationToken);
        return rows.Select(x => (x.Id, x.NombreCodigoMapa)).ToList();
    }
}
