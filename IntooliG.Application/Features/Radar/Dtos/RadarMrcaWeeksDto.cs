namespace IntooliG.Application.Features.Radar.Dtos;

public record RadarMrcaWeeksDto(
    string FechaOrden,
    int Anio,
    int Semana,
    string FcNombreMarca,
    double? Valor);
