using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;

namespace IntooliG.Application.Services;

public interface IEstadoService
{
    Task<PagedListResult<EstadoDto>> ListAsync(
        string? search,
        int? paisId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default);

    Task<EstadoDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<(int Id, string Nombre)>> GetCodigosMapaAsync(CancellationToken cancellationToken = default);

    Task<EstadoDto> CreateAsync(EstadoCreateRequest request, CancellationToken cancellationToken = default);

    Task<EstadoDto?> UpdateAsync(int id, EstadoUpdateRequest request, CancellationToken cancellationToken = default);

    Task<bool> DesactivarAsync(int id, CancellationToken cancellationToken = default);
}
