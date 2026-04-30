using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class MarcaRepository : IMarcaRepository, IProductoRepository
{
    private readonly AppDbContext _db;

    public MarcaRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<(CatMarca Marca, string ClienteNombre)> Items, int Total)> ListAsync(
        int clienteId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var term = search?.Trim();

        var q =
            from m in _db.CatMarcas.AsNoTracking()
            join c in _db.CatClientes.AsNoTracking() on m.ClienteId equals c.Id
            where m.ClienteId == clienteId
                  && (term == null || term == string.Empty || m.NombreMarca.Contains(term))
            select new { m, c.Nombre };

        var total = await q.CountAsync(cancellationToken);
        var pageRows = await q
            .OrderBy(x => x.m.NombreMarca)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = pageRows.Select(x => (x.m, x.Nombre)).ToList();
        return (items, total);
    }

    public async Task<Dictionary<int, string>> GetProductosResumenPorMarcaAsync(
        IReadOnlyCollection<int> marcaIds,
        CancellationToken cancellationToken = default)
    {
        if (marcaIds.Count == 0)
            return new Dictionary<int, string>();

        var productos = await _db.CatProductos.AsNoTracking()
            .Where(p => marcaIds.Contains(p.MarcaId))
            .OrderBy(p => p.Nombre)
            .Select(p => new { p.MarcaId, p.Nombre })
            .ToListAsync(cancellationToken);

        return productos
            .GroupBy(x => x.MarcaId)
            .ToDictionary(g => g.Key, g => string.Join(", ", g.Select(x => x.Nombre)));
    }

    public async Task<(CatMarca? Marca, string? ClienteNombre)> GetByIdWithClienteAsync(
        int id,
        int clienteId,
        CancellationToken cancellationToken = default)
    {
        var row = await (
            from m in _db.CatMarcas.AsNoTracking()
            join c in _db.CatClientes.AsNoTracking() on m.ClienteId equals c.Id
            where m.Id == id && m.ClienteId == clienteId
            select new { m, c.Nombre }).FirstOrDefaultAsync(cancellationToken);

        return row is null ? (null, null) : (row.m, row.Nombre);
    }

    public async Task<IReadOnlyList<CatProducto>> GetProductosByMarcaAsync(
        int marcaId,
        int clienteId,
        CancellationToken cancellationToken = default)
    {
        var exists = await _db.CatMarcas.AsNoTracking()
            .AnyAsync(m => m.Id == marcaId && m.ClienteId == clienteId, cancellationToken);
        if (!exists)
            return Array.Empty<CatProducto>();

        return await _db.CatProductos.AsNoTracking()
            .Where(p => p.MarcaId == marcaId)
            .OrderBy(p => p.Nombre)
            .ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyList<CatProducto>> ListByMarcaAsync(int marcaId, int clienteId, CancellationToken cancellationToken = default) =>
        GetProductosByMarcaAsync(marcaId, clienteId, cancellationToken);

    public async Task<CatProducto?> GetByIdForClienteAsync(int productoId, int clienteId, CancellationToken cancellationToken = default)
    {
        return await (
            from p in _db.CatProductos.AsNoTracking()
            join m in _db.CatMarcas.AsNoTracking() on p.MarcaId equals m.Id
            where p.Id == productoId && m.ClienteId == clienteId
            select p).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CatMarca> AddAsync(CatMarca entity, CancellationToken cancellationToken = default)
    {
        _db.CatMarcas.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<CatMarca?> GetTrackedByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default) =>
        _db.CatMarcas.FirstOrDefaultAsync(m => m.Id == id && m.ClienteId == clienteId, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);

    public async Task<bool> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var marca = await _db.CatMarcas.FirstOrDefaultAsync(m => m.Id == id && m.ClienteId == clienteId, cancellationToken);
            if (marca is null)
                return false;

            var productos = await _db.CatProductos.Where(p => p.MarcaId == id).ToListAsync(cancellationToken);
            _db.CatProductos.RemoveRange(productos);
            _db.CatMarcas.Remove(marca);
            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            await tx.RollbackAsync(cancellationToken);
            return false;
        }
    }

    public async Task ReplaceProductosAsync(int marcaId, IReadOnlyList<string> nombresProductos, CancellationToken cancellationToken = default)
    {
        var existing = await _db.CatProductos.Where(p => p.MarcaId == marcaId).ToListAsync(cancellationToken);
        _db.CatProductos.RemoveRange(existing);

        foreach (var nombre in nombresProductos.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => n.Trim()).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            _db.CatProductos.Add(new CatProducto
            {
                MarcaId = marcaId,
                Nombre = nombre,
                Activo = true
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
