using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.TipoCambio;

namespace IntooliG.Application.Services;

public interface ITipoCambioService
{
    Task<PagedListResult<TipoCambioDto>> ListAsync(
        string? search,
        int? paisId,
        int? anio,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<TipoCambioDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<TipoCambioDto> CreateAsync(TipoCambioRequestDto request, CancellationToken cancellationToken = default);

    Task<TipoCambioDto?> UpdateAsync(int id, TipoCambioRequestDto request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TipoCambioAnioDto>> ListAniosAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TipoCambioMesDto>> ListMesesAsync(CancellationToken cancellationToken = default);
}
