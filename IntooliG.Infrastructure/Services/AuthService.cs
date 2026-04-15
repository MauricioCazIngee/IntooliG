using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Abstractions.Security;
using IntooliG.Application.Features.Auth;
using IntooliG.Application.Services;
using IntooliG.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IntooliG.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly IJwtTokenGenerator _jwt;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository usuarios,
        IPasswordHasher<Usuario> passwordHasher,
        IJwtTokenGenerator jwt,
        ILogger<AuthService> logger)
    {
        _usuarios = usuarios;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarios.GetByEmailAsync(request.Email.Trim(), cancellationToken);
        if (usuario is null)
        {
            _logger.LogWarning("Intento de login con email inexistente: {Email}", request.Email);
            return null;
        }

        var passwordOk = false;
        if (LegacyPasswordHasher.IsLegacyStoredHash(usuario.PasswordHash))
        {
            passwordOk = LegacyPasswordHasher.VerifyPassword(request.Password, usuario.PasswordHash);
        }
        else
        {
            var verify = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, request.Password);
            passwordOk = verify != PasswordVerificationResult.Failed;
        }

        if (!passwordOk)
        {
            _logger.LogWarning("Contraseña inválida para usuario {Email}", request.Email);
            return null;
        }

        var token = _jwt.CreateToken(usuario);
        return new LoginResponse(token, usuario.Id, usuario.Email, usuario.Nombre, UsuarioAppRoles.FromRolId(usuario.RolId));
    }
}
