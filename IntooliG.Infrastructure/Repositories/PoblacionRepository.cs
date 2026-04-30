using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class PoblacionRepository : IPoblacionRepository
{
    private readonly AppDbContext _db;

    public PoblacionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<(CatPoblacion Poblacion, string NombreCiudad, string NombreEstado, string NombrePais, int EstadoId, int PaisId)> Items, int Total)> ListAsync(
        string? search,
        int? paisId,
        int? estadoId,
        int? anio,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var q =
            from po in _db.CatPoblaciones.AsNoTracking()
            join c in _db.CatCiudades.AsNoTracking() on po.CiudadId equals c.Id
            join e in _db.CatEstados.AsNoTracking() on c.EstadoId equals e.Id
            join p in _db.CatPaises.AsNoTracking() on e.PaisId equals p.Id
            where (anio == null || po.Anio == anio)
                  && (estadoId == null || c.EstadoId == estadoId)
                  && (paisId == null || e.PaisId == paisId)
                  && (term == null || term == string.Empty
                      || c.NombreCiudad.Contains(term)
                      || e.NombreEstado.Contains(term)
                      || p.NombrePais.Contains(term))
            select new { po, c.NombreCiudad, e.NombreEstado, p.NombrePais, EstadoId = e.Id, PaisId = p.Id };

        var total = await q.CountAsync(cancellationToken);
        var pageRows = await q
            .OrderByDescending(x => x.po.Anio)
            .ThenBy(x => x.NombrePais)
            .ThenBy(x => x.NombreCiudad)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = pageRows
            .Select(x => (x.po, x.NombreCiudad, x.NombreEstado, x.NombrePais, x.EstadoId, x.PaisId))
            .ToList();
        return (items, total);
    }

    public async Task<(CatPoblacion? Poblacion, string? NombreCiudad, string? NombreEstado, string? NombrePais, int? EstadoId, int? PaisId)> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var row = await (
            from po in _db.CatPoblaciones.AsNoTracking()
            join c in _db.CatCiudades.AsNoTracking() on po.CiudadId equals c.Id
            join e in _db.CatEstados.AsNoTracking() on c.EstadoId equals e.Id
            join p in _db.CatPaises.AsNoTracking() on e.PaisId equals p.Id
            where po.Id == id
            select new { po, c.NombreCiudad, e.NombreEstado, p.NombrePais, EstadoId = e.Id, PaisId = p.Id }).FirstOrDefaultAsync(cancellationToken);

        return row is null
            ? (null, null, null, null, null, null)
            : (row.po, row.NombreCiudad, row.NombreEstado, row.NombrePais, row.EstadoId, row.PaisId);
    }

    public Task<bool> ExistsCiudadAnioAsync(int ciudadId, int anio, int? exceptId, CancellationToken cancellationToken = default)
    {
        var q = _db.CatPoblaciones.AsNoTracking().Where(x => x.CiudadId == ciudadId && x.Anio == anio);
        if (exceptId != null)
            q = q.Where(x => x.Id != exceptId);
        return q.AnyAsync(cancellationToken);
    }

    public async Task<CatPoblacion> AddAsync(CatPoblacion entity, CancellationToken cancellationToken = default)
    {
        _db.CatPoblaciones.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatPoblacion entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatPoblaciones.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
        if (tracked is null)
            throw new InvalidOperationException("Registro de población no encontrado.");
        tracked.CiudadId = entity.CiudadId;
        tracked.Anio = entity.Anio;
        tracked.Cantidad = entity.Cantidad;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatPoblaciones.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (tracked is null)
            return false;
        _db.CatPoblaciones.Remove(tracked);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<int>> GetAniosDisponiblesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.CatPoblaciones.AsNoTracking()
            .Select(x => x.Anio)
            .Distinct()
            .OrderByDescending(a => a)
            .ToListAsync(cancellationToken);
    }
}
