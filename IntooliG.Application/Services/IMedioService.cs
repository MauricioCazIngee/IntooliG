using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Medios;

namespace IntooliG.Application.Services;

public interface IMedioService
{
    Task<PagedListResult<MedioDto>> ListAsync(
        string? search,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default);

    Task<MedioDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<MedioDto> CreateAsync(MedioCreateRequest request, CancellationToken cancellationToken = default);

    Task<MedioDto?> UpdateAsync(int id, MedioUpdateRequest request, CancellationToken cancellationToken = default);

    Task<bool> DesactivarAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MedioLookupDto>> ListActivosLookupAsync(CancellationToken cancellationToken = default);
}
