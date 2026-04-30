using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class DayPartRepository : IDayPartRepository
{
    private readonly AppDbContext _db;

    public DayPartRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<CatDaypart> Items, int Total)> ListAsync(
        int clienteId,
        string? search,
        int? paisId,
        int? medioId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var q = _db.CatDayparts
            .AsNoTracking()
            .Include(x => x.Pais)
            .Include(x => x.Medio)
            .Where(x => x.ClienteId == clienteId)
            .AsQueryable();

        if (paisId is int pid && pid > 0)
            q = q.Where(x => x.PaisId == pid);
        if (medioId is int mid && mid > 0)
            q = q.Where(x => x.MedioId == mid);

        if (!string.IsNullOrWhiteSpace(term))
        {
            q = q.Where(x =>
                x.Nombre.Contains(term)
                || (x.Pais != null && x.Pais.NombrePais.Contains(term))
                || (x.Medio != null && x.Medio.NombreMedio.Contains(term)));
        }

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderBy(x => x.Pais!.NombrePais)
            .ThenBy(x => x.Medio!.NombreMedio)
            .ThenBy(x => x.HoraInicio)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<CatDaypart?> GetByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default) =>
        _db.CatDayparts
            .AsNoTracking()
            .Include(x => x.Pais)
            .Include(x => x.Medio)
            .FirstOrDefaultAsync(x => x.Id == id && x.ClienteId == clienteId, cancellationToken);

    public async Task<CatDaypart> AddAsync(CatDaypart entity, CancellationToken cancellationToken = default)
    {
        _db.CatDayparts.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatDaypart entity, int clienteId, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatDayparts.FirstOrDefaultAsync(x => x.Id == entity.Id && x.ClienteId == clienteId, cancellationToken);
        if (tracked is null)
            throw new InvalidOperationException("DayPart no encontrado.");

        tracked.PaisId = entity.PaisId;
        tracked.MedioId = entity.MedioId;
        tracked.Nombre = entity.Nombre;
        tracked.HoraInicio = entity.HoraInicio;
        tracked.HoraFin = entity.HoraFin;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<(bool Deleted, bool Conflict)> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatDayparts.FirstOrDefaultAsync(x => x.Id == id && x.ClienteId == clienteId, cancellationToken);
        if (tracked is null)
            return (false, false);

        _db.CatDayparts.Remove(tracked);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return (true, false);
        }
        catch (DbUpdateException)
        {
            return (false, true);
        }
    }

    public Task<bool> ExistsByDescripcionAsync(
        int clienteId,
        int paisId,
        int medioId,
        string descripcion,
        long? excludeId,
        CancellationToken cancellationToken = default) =>
        _db.CatDayparts.AsNoTracking()
            .AnyAsync(x =>
                x.ClienteId == clienteId
                && x.PaisId == paisId
                && x.MedioId == medioId
                && x.Nombre == descripcion
                && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    public async Task<(IReadOnlyList<(int Id, string Nombre)> Paises, IReadOnlyList<(int Id, string Nombre)> Medios)> GetLookupAsync(
        CancellationToken cancellationToken = default)
    {
        var paises = await _db.CatPaises.AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.NombrePais)
            .Select(x => new { x.Id, x.NombrePais })
            .ToListAsync(cancellationToken);

        var medios = await _db.CatMedios.AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.NombreMedio)
            .Select(x => new { x.Id, x.NombreMedio })
            .ToListAsync(cancellationToken);

        return (
            paises.Select(x => (x.Id, x.NombrePais)).ToList(),
            medios.Select(x => (x.Id, x.NombreMedio)).ToList()
        );
    }
}
