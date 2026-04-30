using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IRegionRepository
{
    Task<(IReadOnlyList<(CatRegion Region, string NombrePais)> Items, int Total)> ListAsync(
        int clienteId,
        string? search,
        int? paisId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default);

    Task<(CatRegion? Region, string? NombrePais)> GetByIdAsync(
        int id,
        int clienteId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> GetCiudadIdsByRegionAsync(int regionId, CancellationToken cancellationToken = default);

    Task<string> GetCiudadesResumenAsync(int regionId, CancellationToken cancellationToken = default);

    Task<CatRegion> AddAsync(CatRegion entity, CancellationToken cancellationToken = default);

    /// <summary>Alta atómica de región + agrupación de ciudades.</summary>
    Task<CatRegion> CreateWithCiudadesAsync(CatRegion entity, IReadOnlyList<int> ciudadIds, CancellationToken cancellationToken = default);

    Task UpdateAsync(CatRegion entity, CancellationToken cancellationToken = default);

    Task ReplaceCiudadesAsync(int regionId, IReadOnlyList<int> ciudadIds, CancellationToken cancellationToken = default);

    /// <summary>Actualización atómica de región + ciudades asignadas.</summary>
    Task UpdateWithCiudadesAsync(CatRegion entity, IReadOnlyList<int> ciudadIds, CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default);
}
