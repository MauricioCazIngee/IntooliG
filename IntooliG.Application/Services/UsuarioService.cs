using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Features.Usuarios;
using IntooliG.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IntooliG.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UsuarioService(IUsuarioRepository repository, IPasswordHasher<Usuario> passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<IReadOnlyList<UsuarioDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var users = await _repository.ListAsync(cancellationToken);
        return users.Select(Map).ToList();
    }

    public async Task<UsuarioDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        return user is null ? null : Map(user);
    }

    public async Task<(UsuarioDto? Usuario, string? Error)> CreateAsync(CreateUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();
        var nombre = request.Nombre.Trim();
        var rol = NormalizeRol(request.Rol);

        if (string.IsNullOrWhiteSpace(email))
            return (null, "El email es requerido.");

        if (string.IsNullOrWhiteSpace(nombre))
            return (null, "El nombre es requerido.");

        if (rol == 0)
            return (null, "Rol inválido. Usa Admin o Usuario.");

        if (request.ClienteId <= 0)
            return (null, "ClienteId es requerido y debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            return (null, "La contraseña inicial debe tener al menos 6 caracteres.");

        if (await _repository.ExistsByEmailAsync(email, null, cancellationToken))
            return (null, "Ya existe un usuario con ese email.");

        var ahora = DateTime.UtcNow;
        var entity = new Usuario
        {
            Email = email,
            Nombre = nombre,
            Apellidos = string.Empty,
            ClienteId = request.ClienteId,
            RolId = rol,
            RolPbiId = null,
            Estatus = true,
            CodigoActivacion = Guid.NewGuid(),
            CorreoVerificado = true,
            Intentos = null,
            ResetPasswordCode = null,
            PrimerAcceso = false,
            ExpiracionPassword = null,
            FechaNacimiento = null,
            FechaCreacion = ahora
        };
        entity.PasswordHash = _passwordHasher.HashPassword(entity, request.Password);

        var created = await _repository.AddAsync(entity, cancellationToken);
        return (Map(created), null);
    }

    public async Task<(UsuarioDto? Usuario, string? Error)> UpdateAsync(int id, UpdateUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return (null, "Usuario no encontrado.");

        var email = request.Email.Trim();
        var nombre = request.Nombre.Trim();
        var rol = NormalizeRol(request.Rol);

        if (string.IsNullOrWhiteSpace(email))
            return (null, "El email es requerido.");

        if (string.IsNullOrWhiteSpace(nombre))
            return (null, "El nombre es requerido.");

        if (rol == 0)
            return (null, "Rol inválido. Usa Admin o Usuario.");

        if (request.ClienteId <= 0)
            return (null, "ClienteId es requerido y debe ser mayor a cero.");

        if (await _repository.ExistsByEmailAsync(email, id, cancellationToken))
            return (null, "Ya existe un usuario con ese email.");

        entity.Email = email;
        entity.Nombre = nombre;
        entity.ClienteId = request.ClienteId;
        entity.RolId = rol;
        await _repository.UpdateAsync(entity, cancellationToken);

        return (Map(entity), null);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        _repository.DeleteAsync(id, cancellationToken);

    public async Task<(UsuarioDto? Usuario, string? Error)> ChangeRolAsync(int id, ChangeRolRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return (null, "Usuario no encontrado.");

        var rol = NormalizeRol(request.Rol);
        if (rol == 0)
            return (null, "Rol inválido. Usa Admin o Usuario.");

        entity.RolId = rol;
        await _repository.UpdateAsync(entity, cancellationToken);
        return (Map(entity), null);
    }

    public async Task<(bool Ok, string? Error)> ResetPasswordAsync(int id, ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return (false, "Usuario no encontrado.");

        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
            return (false, "La contraseña debe tener al menos 6 caracteres.");

        entity.PasswordHash = _passwordHasher.HashPassword(entity, request.NewPassword);
        await _repository.UpdateAsync(entity, cancellationToken);
        return (true, null);
    }

    private static int NormalizeRol(string raw)
    {
        var value = raw.Trim();
        if (value.Equals("admin", StringComparison.OrdinalIgnoreCase))
            return 1;
        if (value.Equals("usuario", StringComparison.OrdinalIgnoreCase))
            return 2;
        return 0;
    }

    private static UsuarioDto Map(Usuario u) =>
        new(u.Id, u.ClienteId, u.Email, u.NombreCompleto, UsuarioAppRoles.FromRolId(u.RolId), u.FechaCreacion);
}
