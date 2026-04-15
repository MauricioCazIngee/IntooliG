namespace IntooliG.Application.Features.Radar;

public record RadarEstadoDto(
    string FcNombreCategoria,
    short Semana,
    string FcNombreEstado,
    string FcNombreCodigoMapa,
    double? FnPrecio);
