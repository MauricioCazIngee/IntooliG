namespace IntooliG.Application.Features.Radar.Dtos;

public record RadarInversionMarcaMediosDto(
    string FcNombreCategoria,
    short Anio,
    short Semana,
    string FcNombreMedio,
    double? FnPrecio);
