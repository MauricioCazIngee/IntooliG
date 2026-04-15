namespace IntooliG.Application.Features.Usuarios;

public record UsuarioDto(
    int Id,
    int ClienteId,
    string Email,
    string Nombre,
    string Rol,
    DateTime FechaCreacionUtc);

public record CreateUsuarioRequest(
    string Email,
    string Nombre,
    string Rol,
    string Password,
    int ClienteId);

public record UpdateUsuarioRequest(
    string Email,
    string Nombre,
    string Rol,
    int ClienteId);

public record ChangeRolRequest(string Rol);

public record ResetPasswordRequest(string NewPassword);
