using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class CiudadRepository : ICiudadRepository
{
    private readonly AppDbContext _db;

    public CiudadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<(CatCiudad Ciudad, string NombreEstado, string NombrePais, int PaisId)> Items, int Total)> ListAsync(
        string? search,
        int? paisId,
        int? estadoId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var q =
            from c in _db.CatCiudades.AsNoTracking()
            join e in _db.CatEstados.AsNoTracking() on c.EstadoId equals e.Id
            join p in _db.CatPaises.AsNoTracking() on e.PaisId equals p.Id
            where (soloActivos != true || c.Activo)
                  && (estadoId == null || c.EstadoId == estadoId)
                  && (paisId == null || e.PaisId == paisId)
                  && (term == null || term == string.Empty
                      || c.NombreCiudad.Contains(term)
                      || e.NombreEstado.Contains(term)
                      || p.NombrePais.Contains(term))
            select new { c, e.NombreEstado, p.NombrePais, PaisId = p.Id };

        var total = await q.CountAsync(cancellationToken);
        var pageRows = await q
            .OrderBy(x => x.NombrePais)
            .ThenBy(x => x.NombreEstado)
            .ThenBy(x => x.c.NombreCiudad)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = pageRows.Select(x => (x.c, x.NombreEstado, x.NombrePais, x.PaisId)).ToList();
        return (items, total);
    }

    public async Task<(CatCiudad? Ciudad, string? NombreEstado, string? NombrePais, int? PaisId)> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var row = await (
            from c in _db.CatCiudades.AsNoTracking()
            join e in _db.CatEstados.AsNoTracking() on c.EstadoId equals e.Id
            join p in _db.CatPaises.AsNoTracking() on e.PaisId equals p.Id
            where c.Id == id
            select new { c, e.NombreEstado, p.NombrePais, PaisId = p.Id }).FirstOrDefaultAsync(cancellationToken);

        return row is null ? (null, null, null, null) : (row.c, row.NombreEstado, row.NombrePais, row.PaisId);
    }

    public async Task<CatCiudad> AddAsync(CatCiudad entity, CancellationToken cancellationToken = default)
    {
        _db.CatCiudades.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatCiudad entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatCiudades.FirstOrDefaultAsync(c => c.Id == entity.Id, cancellationToken);
        if (tracked is null)
            throw new InvalidOperationException("Ciudad no encontrada.");
        tracked.EstadoId = entity.EstadoId;
        tracked.NombreCiudad = entity.NombreCiudad;
        tracked.NombreCorto = entity.NombreCorto;
        tracked.CiudadPrincipal = entity.CiudadPrincipal;
        tracked.Activo = entity.Activo;
        tracked.Poblacion = entity.Poblacion;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatCiudades.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (tracked is null)
            return false;
        tracked.Activo = false;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<(int Id, string Nombre)>> ListForSelectorAsync(
        int? paisId,
        int? estadoId,
        CancellationToken cancellationToken = default)
    {
        var q =
            from c in _db.CatCiudades.AsNoTracking()
            join e in _db.CatEstados.AsNoTracking() on c.EstadoId equals e.Id
            where c.Activo
                  && (estadoId == null || c.EstadoId == estadoId)
                  && (paisId == null || e.PaisId == paisId)
            orderby c.NombreCiudad
            select new { c.Id, c.NombreCiudad };

        var rows = await q.ToListAsync(cancellationToken);
        return rows.Select(x => (x.Id, x.NombreCiudad)).ToList();
    }
}
