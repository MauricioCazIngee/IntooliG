using IntooliG.Application.Features.Usuarios;

namespace IntooliG.Application.Services;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<UsuarioDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(UsuarioDto? Usuario, string? Error)> CreateAsync(CreateUsuarioRequest request, CancellationToken cancellationToken = default);
    Task<(UsuarioDto? Usuario, string? Error)> UpdateAsync(int id, UpdateUsuarioRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<(UsuarioDto? Usuario, string? Error)> ChangeRolAsync(int id, ChangeRolRequest request, CancellationToken cancellationToken = default);
    Task<(bool Ok, string? Error)> ResetPasswordAsync(int id, ResetPasswordRequest request, CancellationToken cancellationToken = default);
}
