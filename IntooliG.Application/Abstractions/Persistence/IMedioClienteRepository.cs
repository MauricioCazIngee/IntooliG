using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IMedioClienteRepository
{
    Task<(IReadOnlyList<(MedioCliente Row, string NombreMedio, string NombreCliente)> Items, int Total)> ListAsync(
        string? search,
        int? clienteId,
        int? medioId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<(MedioCliente? Row, string? NombreMedio, string? NombreCliente)> GetByKeyAsync(
        int medioId,
        int clienteId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int medioId, int clienteId, CancellationToken cancellationToken = default);

    Task<MedioCliente> AddAsync(MedioCliente entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(MedioCliente entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int medioId, int clienteId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<(int MedioId, string NombreMedio, bool EsNacional)>> ListByClienteAsync(
        int clienteId,
        CancellationToken cancellationToken = default);
}
