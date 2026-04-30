using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IMarcaRepository
{
    Task<(IReadOnlyList<(CatMarca Marca, string ClienteNombre)> Items, int Total)> ListAsync(
        int clienteId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Dictionary<int, string>> GetProductosResumenPorMarcaAsync(
        IReadOnlyCollection<int> marcaIds,
        CancellationToken cancellationToken = default);

    Task<(CatMarca? Marca, string? ClienteNombre)> GetByIdWithClienteAsync(int id, int clienteId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CatProducto>> GetProductosByMarcaAsync(int marcaId, int clienteId, CancellationToken cancellationToken = default);

    Task<CatMarca> AddAsync(CatMarca entity, CancellationToken cancellationToken = default);

    Task<CatMarca?> GetTrackedByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default);

    Task ReplaceProductosAsync(int marcaId, IReadOnlyList<string> nombresProductos, CancellationToken cancellationToken = default);
}
