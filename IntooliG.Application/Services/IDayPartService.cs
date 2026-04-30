using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.DayPart;

namespace IntooliG.Application.Services;

public interface IDayPartService
{
    Task<PagedListResult<DayPartDto>> ListAsync(
        int clienteId,
        string? search,
        int? paisId,
        int? medioId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<DayPartDto?> GetByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default);

    Task<DayPartDto> CreateAsync(int clienteId, DayPartRequestDto request, CancellationToken cancellationToken = default);

    Task<DayPartDto?> UpdateAsync(long id, int clienteId, DayPartRequestDto request, CancellationToken cancellationToken = default);

    Task<(bool Deleted, bool Conflict)> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default);

    Task<DayPartLookupDto> GetLookupAsync(CancellationToken cancellationToken = default);
}
