using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IEstadoRepository
{
    Task<(IReadOnlyList<(CatEstado Estado, string NombrePais, string? NombreCodigoMapa)> Items, int Total)> ListAsync(
        string? search,
        int? paisId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default);

    Task<(CatEstado? Estado, string? NombrePais, string? NombreCodigoMapa)> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<CatEstado> AddAsync(CatEstado entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(CatEstado entity, CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<(int Id, string Nombre)>> ListCodigosMapaAsync(CancellationToken cancellationToken = default);
}
