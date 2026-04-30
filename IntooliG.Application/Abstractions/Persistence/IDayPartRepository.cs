using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IDayPartRepository
{
    Task<(IReadOnlyList<CatDaypart> Items, int Total)> ListAsync(
        int clienteId,
        string? search,
        int? paisId,
        int? medioId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<CatDaypart?> GetByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default);

    Task<CatDaypart> AddAsync(CatDaypart entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(CatDaypart entity, int clienteId, CancellationToken cancellationToken = default);

    Task<(bool Deleted, bool Conflict)> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByDescripcionAsync(
        int clienteId,
        int paisId,
        int medioId,
        string descripcion,
        long? excludeId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<(int Id, string Nombre)> Paises, IReadOnlyList<(int Id, string Nombre)> Medios)> GetLookupAsync(
        CancellationToken cancellationToken = default);
}
