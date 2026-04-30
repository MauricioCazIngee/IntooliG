using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class TipoCambioRepository : ITipoCambioRepository
{
    private readonly AppDbContext _db;

    public TipoCambioRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<CatTipoCambio> Items, int Total)> ListAsync(
        string? search,
        int? paisId,
        int? anio,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var q = _db.CatTiposCambio
            .AsNoTracking()
            .Include(x => x.Pais)
            .AsQueryable();

        if (paisId is int pid && pid > 0)
            q = q.Where(x => x.PaisId == pid);

        if (anio is int year && year > 0)
            q = q.Where(x => x.Anio == year);

        if (!string.IsNullOrWhiteSpace(term))
        {
            if (int.TryParse(term, out var value))
            {
                q = q.Where(x =>
                    x.Pais!.NombrePais.Contains(term)
                    || x.Anio == value
                    || x.Mes == value);
            }
            else
            {
                q = q.Where(x => x.Pais!.NombrePais.Contains(term));
            }
        }

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.Anio)
            .ThenByDescending(x => x.Mes)
            .ThenBy(x => x.Pais!.NombrePais)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<CatTipoCambio?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.CatTiposCambio
            .AsNoTracking()
            .Include(x => x.Pais)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<CatTipoCambio> AddAsync(CatTipoCambio entity, CancellationToken cancellationToken = default)
    {
        _db.CatTiposCambio.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatTipoCambio entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatTiposCambio.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
        if (tracked is null)
            throw new InvalidOperationException("Tipo de cambio no encontrado.");

        tracked.PaisId = entity.PaisId;
        tracked.Anio = entity.Anio;
        tracked.Mes = entity.Mes;
        tracked.TipoCambio = entity.TipoCambio;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatTiposCambio.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (tracked is null)
            return false;
        _db.CatTiposCambio.Remove(tracked);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> ExistsAsync(int paisId, int anio, int mes, int? excludeId, CancellationToken cancellationToken = default) =>
        _db.CatTiposCambio.AsNoTracking()
            .AnyAsync(x => x.PaisId == paisId
                && x.Anio == anio
                && x.Mes == mes
                && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    public async Task<IReadOnlyList<int>> ListAniosAsync(CancellationToken cancellationToken = default)
    {
        return await _db.CatTiposCambio.AsNoTracking()
            .Select(x => x.Anio)
            .Distinct()
            .OrderByDescending(x => x)
            .ToListAsync(cancellationToken);
    }
}
