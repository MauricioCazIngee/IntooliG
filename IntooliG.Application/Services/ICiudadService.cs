using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;

namespace IntooliG.Application.Services;

public interface ICiudadService
{
    Task<PagedListResult<CiudadDto>> ListAsync(
        string? search,
        int? paisId,
        int? estadoId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default);

    Task<CiudadDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<(int Id, string Nombre)>> ListForSelectorAsync(
        int? paisId,
        int? estadoId,
        CancellationToken cancellationToken = default);

    Task<CiudadDto> CreateAsync(CiudadCreateRequest request, CancellationToken cancellationToken = default);

    Task<CiudadDto?> UpdateAsync(int id, CiudadUpdateRequest request, CancellationToken cancellationToken = default);

    Task<bool> DesactivarAsync(int id, CancellationToken cancellationToken = default);
}
