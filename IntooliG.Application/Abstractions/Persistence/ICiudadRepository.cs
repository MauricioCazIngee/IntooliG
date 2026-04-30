using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface ICiudadRepository
{
    Task<(IReadOnlyList<(CatCiudad Ciudad, string NombreEstado, string NombrePais, int PaisId)> Items, int Total)> ListAsync(
        string? search,
        int? paisId,
        int? estadoId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default);

    Task<(CatCiudad? Ciudad, string? NombreEstado, string? NombrePais, int? PaisId)> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<CatCiudad> AddAsync(CatCiudad entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(CatCiudad entity, CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Ciudades activas para selector (región).</summary>
    Task<IReadOnlyList<(int Id, string Nombre)>> ListForSelectorAsync(
        int? paisId,
        int? estadoId,
        CancellationToken cancellationToken = default);
}
