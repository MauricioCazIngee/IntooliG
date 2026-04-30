using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IntooliG.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetClienteId(this ClaimsPrincipal user)
    {
        var v = user.FindFirst("cliente_id")?.Value;
        if (string.IsNullOrEmpty(v) || !int.TryParse(v, out var id))
            throw new InvalidOperationException("El token no contiene cliente_id.");
        return id;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        var v = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(v) || !int.TryParse(v, out var id))
            throw new InvalidOperationException("El token no contiene el id de usuario (sub).");
        return id;
    }
}