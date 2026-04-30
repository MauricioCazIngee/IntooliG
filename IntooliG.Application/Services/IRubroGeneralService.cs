using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RubrosConceptos;

namespace IntooliG.Application.Services;

public interface IRubroGeneralService
{
    Task<PagedListResult<RubroGeneralDto>> ListAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<RubroGeneralDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RubroGeneralDto> CreateAsync(RubroGeneralCreateRequest request, CancellationToken cancellationToken = default);
    Task<RubroGeneralDto?> UpdateAsync(int id, RubroGeneralUpdateRequest request, CancellationToken cancellationToken = default);
    Task<(bool Eliminado, bool Conflicto)> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
