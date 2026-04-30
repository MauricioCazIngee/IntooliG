using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Fuentes;

namespace IntooliG.Application.Services;

public interface IMarcaFuenteService
{
    Task<PagedListResult<MarcaFuenteDto>> ListAsync(
        int clienteId,
        string? search,
        int? fuenteId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<MarcaFuenteDto?> GetByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default);
    Task<MarcaFuenteDto> CreateAsync(int clienteId, MarcaFuenteCreateRequest request, CancellationToken cancellationToken = default);
    Task<MarcaFuenteDto?> UpdateAsync(long id, int clienteId, MarcaFuenteUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MarcaFuenteDto>> ListByFuenteAsync(int fuenteId, int clienteId, CancellationToken cancellationToken = default);
}
