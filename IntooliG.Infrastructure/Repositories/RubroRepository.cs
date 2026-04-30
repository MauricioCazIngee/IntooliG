using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Features.RubrosConceptos;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class RubroRepository : IRubroRepository
{
    private readonly AppDbContext _db;

    public RubroRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<RubroListRow> Items, int Total)> ListAsync(
        int clienteId,
        int? categoriaId,
        bool? activo,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var filtered =
            from cr in _db.CatRubros.AsNoTracking()
            join rg in _db.CatRubroGenerales on cr.RubroGeneralId equals rg.Id
            join cat in _db.CatCategorias on cr.CategoriaId equals cat.Id
            join cli in _db.CatClientes on cr.ClienteId equals cli.Id
            where cr.ClienteId == clienteId
                  && (categoriaId == null || cr.CategoriaId == categoriaId)
                  && (activo == null || cr.Activo == activo)
                  && (term == null || term == string.Empty
                      || rg.NombreRubro.Contains(term)
                      || cat.NombreCategoria.Contains(term))
            select new { cr, rg, cat, cli };

        var total = await filtered.CountAsync(cancellationToken);
        var rows = await filtered
            .OrderBy(x => x.rg.NombreRubro)
            .ThenBy(x => x.cat.NombreCategoria)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = rows.Select(x => new RubroListRow(
            x.cr.Id,
            x.cr.RubroGeneralId,
            x.rg.NombreRubro,
            x.cr.CategoriaId,
            x.cat.NombreCategoria,
            x.cr.ValorRubro,
            x.cr.Activo,
            x.cr.ClienteId,
            x.cli.Nombre)).ToList();

        return (items, total);
    }

    public Task<CatRubro?> GetByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default) =>
        _db.CatRubros.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.ClienteId == clienteId, cancellationToken);

    public async Task<RubroListRow?> GetListRowByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default)
    {
        var row = await (
            from cr in _db.CatRubros.AsNoTracking()
            join rg in _db.CatRubroGenerales on cr.RubroGeneralId equals rg.Id
            join cat in _db.CatCategorias on cr.CategoriaId equals cat.Id
            join cli in _db.CatClientes on cr.ClienteId equals cli.Id
            where cr.Id == id && cr.ClienteId == clienteId
            select new RubroListRow(
                cr.Id,
                cr.RubroGeneralId,
                rg.NombreRubro,
                cr.CategoriaId,
                cat.NombreCategoria,
                cr.ValorRubro,
                cr.Activo,
                cr.ClienteId,
                cli.Nombre)).FirstOrDefaultAsync(cancellationToken);
        return row;
    }

    public Task<bool> ExistsCombinacionAsync(
        int rubroGeneralId,
        int categoriaId,
        int clienteId,
        long? exceptId,
        CancellationToken cancellationToken = default)
    {
        var q = _db.CatRubros.AsNoTracking()
            .Where(x => x.RubroGeneralId == rubroGeneralId
                        && x.CategoriaId == categoriaId
                        && x.ClienteId == clienteId);
        if (exceptId is not null)
            q = q.Where(x => x.Id != exceptId.Value);
        return q.AnyAsync(cancellationToken);
    }

    public async Task<CatRubro> AddAsync(CatRubro entity, CancellationToken cancellationToken = default)
    {
        _db.CatRubros.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CatRubro entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.CatRubros.FirstOrDefaultAsync(
            x => x.Id == entity.Id && x.ClienteId == entity.ClienteId,
            cancellationToken);
        if (tracked is null)
            throw new InvalidOperationException("Rubro no encontrado.");

        tracked.RubroGeneralId = entity.RubroGeneralId;
        tracked.CategoriaId = entity.CategoriaId;
        tracked.ValorRubro = entity.ValorRubro;
        tracked.Activo = entity.Activo;
        tracked.ClienteId = entity.ClienteId;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<(bool Eliminado, bool Conflicto)> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default)
    {
        var entity = await _db.CatRubros.FirstOrDefaultAsync(x => x.Id == id && x.ClienteId == clienteId, cancellationToken);
        if (entity is null)
            return (false, false);

        _db.CatRubros.Remove(entity);
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
}
