namespace IntooliG.Application.Features.TipoCambio;

public record TipoCambioDto(
    int Id,
    int PaisId,
    string PaisNombre,
    int Anio,
    int Mes,
    string MesNombre,
    decimal TipoCambio);

public record TipoCambioRequestDto(
    int PaisId,
    int Anio,
    int Mes,
    decimal TipoCambio);

public record TipoCambioAnioDto(int Anio);

public record TipoCambioMesDto(int Mes, string Nombre);
