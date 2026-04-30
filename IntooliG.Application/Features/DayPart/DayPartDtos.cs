namespace IntooliG.Application.Features.DayPart;

public record DayPartDto(
    long Id,
    int PaisId,
    string PaisNombre,
    int MedioId,
    string MedioNombre,
    string Descripcion,
    int HoraInicio,
    int HoraFin,
    string InicioTexto,
    string FinTexto);

public record DayPartRequestDto(
    int PaisId,
    int MedioId,
    string Descripcion,
    int HoraInicio,
    int HoraFin);

public record DayPartLookupOptionDto(int Id, string Nombre);

public record DayPartLookupDto(
    IReadOnlyList<DayPartLookupOptionDto> Paises,
    IReadOnlyList<DayPartLookupOptionDto> Medios,
    IReadOnlyList<int> Horas);
