using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Medios;

namespace IntooliG.Application.Services;

public interface IMedioClienteService
{
    Task<PagedListResult<MedioClienteDto>> ListAsync(
        string? search,
        int? clienteId,
        int? medioId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<MedioClienteDto?> GetByKeyAsync(int medioId, int clienteId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MedioClientePorClienteDto>> ListByClienteAsync(int clienteId, CancellationToken cancellationToken = default);

    Task<MedioClienteDto> CreateAsync(MedioClienteCreateRequest request, CancellationToken cancellationToken = default);

    Task<MedioClienteDto?> UpdateAsync(int medioId, int clienteId, MedioClienteUpdateRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int medioId, int clienteId, CancellationToken cancellationToken = default);
}
