namespace IntooliG.Application.Features.Inversiones;

/// <summary>Parámetros resueltos (fechas, ids) listos para ejecutar stored procedures.</summary>
public record InversionesQueryContext(
    int Periodo,
    DateTime StartDate,
    DateTime FinalDate,
    int? CategoriaId,
    int TipoTarifa,
    int Vista,
    int? RegionId,
    int? CiudadId,
    int ExchangeRate,
    int? MarcaId,
    int? PaisId,
    int? SectorId,
    int? BuId);
