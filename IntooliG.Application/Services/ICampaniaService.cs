using IntooliG.Application.Features.Campanias;

namespace IntooliG.Application.Services;

public interface ICampaniaService
{
    Task<IReadOnlyList<CampaniaDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<CampaniaDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CampaniaDto> CreateAsync(CreateCampaniaRequest request, CancellationToken cancellationToken = default);
    Task<CampaniaDto?> UpdateAsync(int id, UpdateCampaniaRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
