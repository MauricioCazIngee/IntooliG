using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class MedioClienteRepository : IMedioClienteRepository
{
    private readonly AppDbContext _db;

    public MedioClienteRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<(MedioCliente Row, string NombreMedio, string NombreCliente)> Items, int Total)> ListAsync(
        string? search,
        int? clienteId,
        int? medioId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var q =
            from mc in _db.MedioClientes.AsNoTracking()
            join m in _db.CatMedios.AsNoTracking() on mc.MedioId equals m.Id
            join c in _db.CatClientes.AsNoTracking() on mc.ClienteId equals c.Id
            where (clienteId == null || mc.ClienteId == clienteId)
                  && (medioId == null || mc.MedioId == medioId)
                  && (term == null || term == string.Empty
                      || m.NombreMedio.Contains(term)
                      || c.Nombre.Contains(term))
            select new { mc, m.NombreMedio, ClienteNombre = c.Nombre };

        var total = await q.CountAsync(cancellationToken);
        var pageRows = await q
            .OrderBy(x => x.ClienteNombre)
            .ThenBy(x => x.NombreMedio)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = pageRows.Select(x => (x.mc, x.NombreMedio, x.ClienteNombre)).ToList();
        return (items, total);
    }

    public async Task<(MedioCliente? Row, string? NombreMedio, string? NombreCliente)> GetByKeyAsync(
        int medioId,
        int clienteId,
        CancellationToken cancellationToken = default)
    {
        var row = await (
            from mc in _db.MedioClientes.AsNoTracking()
            join m in _db.CatMedios.AsNoTracking() on mc.MedioId equals m.Id
            join c in _db.CatClientes.AsNoTracking() on mc.ClienteId equals c.Id
            where mc.MedioId == medioId && mc.ClienteId == clienteId
            select new { mc, m.NombreMedio, ClienteNombre = c.Nombre }).FirstOrDefaultAsync(cancellationToken);

        return row is null ? (null, null, null) : (row.mc, row.NombreMedio, row.ClienteNombre);
    }

    public Task<bool> ExistsAsync(int medioId, int clienteId, CancellationToken cancellationToken = default) =>
        _db.MedioClientes.AsNoTracking().AnyAsync(
            x => x.MedioId == medioId && x.ClienteId == clienteId,
            cancellationToken);

    public async Task<MedioCliente> AddAsync(MedioCliente entity, CancellationToken cancellationToken = default)
    {
        _db.MedioClientes.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(MedioCliente entity, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.MedioClientes.FirstOrDefaultAsync(
            x => x.MedioId == entity.MedioId && x.ClienteId == entity.ClienteId,
            cancellationToken);
        if (tracked is null)
            throw new InvalidOperationException("Asignación no encontrada.");
        tracked.EsNacional = entity.EsNacional;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int medioId, int clienteId, CancellationToken cancellationToken = default)
    {
        var tracked = await _db.MedioClientes.FirstOrDefaultAsync(
            x => x.MedioId == medioId && x.ClienteId == clienteId,
            cancellationToken);
        if (tracked is null)
            return false;
        _db.MedioClientes.Remove(tracked);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<(int MedioId, string NombreMedio, bool EsNacional)>> ListByClienteAsync(
        int clienteId,
        CancellationToken cancellationToken = default)
    {
        var rows = await (
            from mc in _db.MedioClientes.AsNoTracking()
            join m in _db.CatMedios.AsNoTracking() on mc.MedioId equals m.Id
            where mc.ClienteId == clienteId
            orderby m.NombreMedio
            select new { mc.MedioId, m.NombreMedio, mc.EsNacional }).ToListAsync(cancellationToken);

        return rows.Select(x => (x.MedioId, x.NombreMedio, x.EsNacional)).ToList();
    }
}
