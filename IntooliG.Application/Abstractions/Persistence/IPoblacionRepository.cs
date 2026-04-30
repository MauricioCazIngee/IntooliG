using IntooliG.Domain.Entities;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IPoblacionRepository
{
    Task<(IReadOnlyList<(CatPoblacion Poblacion, string NombreCiudad, string NombreEstado, string NombrePais, int EstadoId, int PaisId)> Items, int Total)> ListAsync(
        string? search,
        int? paisId,
        int? estadoId,
        int? anio,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<(CatPoblacion? Poblacion, string? NombreCiudad, string? NombreEstado, string? NombrePais, int? EstadoId, int? PaisId)> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsCiudadAnioAsync(int ciudadId, int anio, int? exceptId, CancellationToken cancellationToken = default);

    Task<CatPoblacion> AddAsync(CatPoblacion entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(CatPoblacion entity, CancellationToken cancellationToken = default);

    /// <summary>Eliminación física (tbPoblacion no tiene FbActivo).</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> GetAniosDisponiblesAsync(CancellationToken cancellationToken = default);
}
