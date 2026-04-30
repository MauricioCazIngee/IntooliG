using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Catalogo;

namespace IntooliG.Application.Services;

public interface IMarcaService
{
    Task<PagedListResult<MarcaDto>> ListAsync(int clienteId, string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<MarcaDetailDto?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default);
    Task<MarcaDetailDto> CreateAsync(int clienteId, MarcaCreateRequest request, CancellationToken cancellationToken = default);
    Task<MarcaDetailDto?> UpdateAsync(int id, int clienteId, MarcaUpdateRequest request, CancellationToken cancellationToken = default);
    Task<(bool Eliminado, bool Conflicto)> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default);
}
