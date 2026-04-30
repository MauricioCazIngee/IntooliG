using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class ConceptoRepository : IConceptoRepository
{
    private readonly AppDbContext _db;

    public ConceptoRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<ConceptoListRow> Items, int Total)> ListAsync(
        int clienteId,
        int? categoriaId,
        int? rubroGeneralId,
        bool? activo,
        bool? top,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var filtered =
            from c in _db.CatConceptos.AsNoTracking()
            join rg in _db.CatRubroGenerales on c.RubroGeneralId equals rg.Id
            join cat in _db.CatCategorias on c.CategoriaId equals cat.Id
            join rnk in _db.CatRankings on c.Id equals rnk.ConceptoId
            where _db.CatRubros.Any(cr =>
                      cr.RubroGeneralId == c.RubroGeneralId
                      && cr.CategoriaId == c.CategoriaId
                      && cr.ClienteId == clienteId)
                  && (categoriaId == null || c.CategoriaId == categoriaId)
                  && (rubroGeneralId == null || c.RubroGeneralId == rubroGeneralId)
                  && (activo == null || c.Activo == activo)
                  && (top == null || c.Top == top)
                  && (term == null || term == string.Empty
                      || c.NombreConcepto.Contains(term)
                      || rg.NombreRubro.Contains(term)
                      || cat.NombreCategoria.Contains(term))
            join val in _db.CatValores on new { R = c.RubroGeneralId, C = c.CategoriaId, P = rnk.Posicion }
                equals new { R = val.RubroGeneralId, C = val.CategoriaId, P = val.Posicion } into valJoin
            from val in valJoin.DefaultIfEmpty()
            select new { c, rg, cat, rnk, val };

        var total = await filtered.CountAsync(cancellationToken);
        var rows = await filtered
            .OrderBy(x => x.rg.NombreRubro)
            .ThenBy(x => x.cat.NombreCategoria)
            .ThenBy(x => x.c.NombreConcepto)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = rows.Select(x => new ConceptoListRow(
            x.c.Id,
            x.c.NombreConcepto,
            x.c.CategoriaId,
            x.cat.NombreCategoria,
            x.c.RubroGeneralId,
            x.rg.NombreRubro,
            x.rnk.Posicion,
            x.val != null ? x.val.Valor : (decimal?)null,
            x.c.Activo,
            x.c.Top)).ToList();

        return (items, total);
    }

    public async Task<(ConceptoListRow? Row, bool Found)> GetDetailRowByIdAsync(
        long id,
        int clienteId,
        CancellationToken cancellationToken = default)
    {
        var row = await (
            from c in _db.CatConceptos.AsNoTracking()
            join rg in _db.CatRubroGenerales on c.RubroGeneralId equals rg.Id
            join cat in _db.CatCategorias on c.CategoriaId equals cat.Id
            join rnk in _db.CatRankings on c.Id equals rnk.ConceptoId
            where c.Id == id
                  && _db.CatRubros.Any(cr =>
                      cr.RubroGeneralId == c.RubroGeneralId
                      && cr.CategoriaId == c.CategoriaId
                      && cr.ClienteId == clienteId)
            join val in _db.CatValores on new { R = c.RubroGeneralId, C = c.CategoriaId, P = rnk.Posicion }
                equals new { R = val.RubroGeneralId, C = val.CategoriaId, P = val.Posicion } into valJoin
            from val in valJoin.DefaultIfEmpty()
            select new ConceptoListRow(
                c.Id,
                c.NombreConcepto,
                c.CategoriaId,
                cat.NombreCategoria,
                c.RubroGeneralId,
                rg.NombreRubro,
                rnk.Posicion,
                val != null ? val.Valor : (decimal?)null,
                c.Activo,
                c.Top)).FirstOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            var exists = await _db.CatConceptos.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken);
            return (null, exists);
        }

        return (row, true);
    }

    public async Task<long> CreateFullAsync(
        int rubroGeneralId,
        int categoriaId,
        string nombreConcepto,
        int posicion,
        decimal valor,
        bool activo,
        bool top,
        CancellationToken cancellationToken = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var c = new CatConcepto
            {
                RubroGeneralId = rubroGeneralId,
                CategoriaId = categoriaId,
                NombreConcepto = nombreConcepto.Trim(),
                Activo = activo,
                Top = top
            };
            _db.CatConceptos.Add(c);
            await _db.SaveChangesAsync(cancellationToken);

            var r = new CatRanking
            {
                ConceptoId = c.Id,
                RubroGeneralId = rubroGeneralId,
                CategoriaId = categoriaId,
                Posicion = posicion
            };
            _db.CatRankings.Add(r);
            await _db.SaveChangesAsync(cancellationToken);

            await UpsertValorCoreAsync(rubroGeneralId, categoriaId, posicion, valor, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return c.Id;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> UpdateFullAsync(
        long id,
        int clienteId,
        int rubroGeneralId,
        int categoriaId,
        string nombreConcepto,
        int posicion,
        decimal valor,
        bool activo,
        bool top,
        CancellationToken cancellationToken = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var c = await _db.CatConceptos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (c is null)
            {
                await tx.RollbackAsync(cancellationToken);
                return false;
            }

            var scoped = await _db.CatRubros.AnyAsync(
                cr => cr.RubroGeneralId == c.RubroGeneralId
                      && cr.CategoriaId == c.CategoriaId
                      && cr.ClienteId == clienteId,
                cancellationToken);
            if (!scoped)
            {
                await tx.RollbackAsync(cancellationToken);
                return false;
            }

            var rk = await _db.CatRankings.FirstOrDefaultAsync(x => x.ConceptoId == id, cancellationToken);
            if (rk is null)
            {
                await tx.RollbackAsync(cancellationToken);
                return false;
            }

            var oldR = rk.RubroGeneralId;
            var oldC = rk.CategoriaId;
            var oldP = rk.Posicion;

            c.NombreConcepto = nombreConcepto.Trim();
            c.RubroGeneralId = rubroGeneralId;
            c.CategoriaId = categoriaId;
            c.Activo = activo;
            c.Top = top;

            rk.RubroGeneralId = rubroGeneralId;
            rk.CategoriaId = categoriaId;
            rk.Posicion = posicion;

            await UpsertValorCoreAsync(rubroGeneralId, categoriaId, posicion, valor, cancellationToken);

            if (oldR != rubroGeneralId || oldC != categoriaId || oldP != posicion)
                await RemoveValorIfOrphanAsync(oldR, oldC, oldP, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task UpsertValorCoreAsync(int rubroGeneralId, int categoriaId, int posicion, decimal valor, CancellationToken cancellationToken)
    {
        var existing = await _db.CatValores.FirstOrDefaultAsync(
            v => v.RubroGeneralId == rubroGeneralId
                 && v.CategoriaId == categoriaId
                 && v.Posicion == posicion,
            cancellationToken);

        if (existing is null)
        {
            _db.CatValores.Add(new CatValor
            {
                RubroGeneralId = rubroGeneralId,
                CategoriaId = categoriaId,
                Posicion = posicion,
                Valor = valor
            });
        }
        else
        {
            existing.Valor = valor;
        }
    }

    private async Task RemoveValorIfOrphanAsync(int rubroGeneralId, int categoriaId, int posicion, CancellationToken cancellationToken)
    {
        var stillUsed = await _db.CatRankings.AnyAsync(
            r => r.RubroGeneralId == rubroGeneralId
                 && r.CategoriaId == categoriaId
                 && r.Posicion == posicion,
            cancellationToken);
        if (stillUsed)
            return;

        var val = await _db.CatValores.FirstOrDefaultAsync(
            v => v.RubroGeneralId == rubroGeneralId
                 && v.CategoriaId == categoriaId
                 && v.Posicion == posicion,
            cancellationToken);
        if (val is not null)
            _db.CatValores.Remove(val);
    }

    public async Task<(bool Eliminado, bool Conflicto)> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default)
    {
        var c = await _db.CatConceptos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (c is null)
            return (false, false);

        var scoped = await _db.CatRubros.AnyAsync(
            cr => cr.RubroGeneralId == c.RubroGeneralId
                  && cr.CategoriaId == c.CategoriaId
                  && cr.ClienteId == clienteId,
            cancellationToken);
        if (!scoped)
            return (false, false);

        var rnk = await _db.CatRankings.AsNoTracking().FirstOrDefaultAsync(x => x.ConceptoId == id, cancellationToken);
        var rubro = rnk?.RubroGeneralId ?? c.RubroGeneralId;
        var cat = rnk?.CategoriaId ?? c.CategoriaId;
        var pos = rnk?.Posicion ?? 0;

        try
        {
            _db.CatConceptos.Remove(c);
            await _db.SaveChangesAsync(cancellationToken);

            await RemoveValorIfOrphanAsync(rubro, cat, pos, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return (true, false);
        }
        catch (DbUpdateException)
        {
            return (false, true);
        }
    }
}
