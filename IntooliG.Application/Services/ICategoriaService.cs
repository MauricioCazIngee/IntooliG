using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Catalogo;

namespace IntooliG.Application.Services;

public interface ICategoriaService
{
    Task<PagedListResult<CategoriaDto>> ListAsync(int clienteId, int? buId, string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<CategoriaDto?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default);
    Task<CategoriaDto> CreateAsync(int clienteId, CategoriaCreateRequest request, CancellationToken cancellationToken = default);
    Task<CategoriaDto?> UpdateAsync(int id, int clienteId, CategoriaUpdateRequest request, CancellationToken cancellationToken = default);
    Task<(bool Eliminado, bool Conflicto)> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default);
}
