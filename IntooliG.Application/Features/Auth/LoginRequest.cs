namespace IntooliG.Application.Features.Auth;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string Token, int UsuarioId, string Email, string Nombre, string Rol);
