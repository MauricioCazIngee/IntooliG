namespace IntooliG.Application.Features.Campanias;

public record CampaniaDto(
    int Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    bool Activa,
    DateTime FechaCreacionUtc);

public record CreateCampaniaRequest(
    string Codigo,
    string Nombre,
    string? Descripcion,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    bool Activa);

public record UpdateCampaniaRequest(
    string Codigo,
    string Nombre,
    string? Descripcion,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    bool Activa);
