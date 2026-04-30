using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IProductoRepository
{
    Task<IReadOnlyList<CatProducto>> ListByMarcaAsync(int marcaId, int clienteId, CancellationToken cancellationToken = default);
    Task<CatProducto?> GetByIdForClienteAsync(int productoId, int clienteId, CancellationToken cancellationToken = default);
}
