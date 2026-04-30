namespace IntooliG.Application.Features.RegionCiudad;

public record CodigoMapaLookupDto(int Id, string Nombre);

public record CiudadSelectorItemDto(int Id, string Nombre);

public record PaisDto(int Id, string NombrePais, bool Activo);

public record PaisCreateRequest(string NombrePais, bool Activo);

public record PaisUpdateRequest(string NombrePais, bool Activo);

public record EstadoDto(
    int Id,
    string NombreEstado,
    int PaisId,
    string NombrePais,
    int? CodigoMapaId,
    string? CodigoMapaNombre,
    bool Activo);

public record EstadoCreateRequest(string NombreEstado, int PaisId, int? CodigoMapaId, bool Activo);

public record EstadoUpdateRequest(string NombreEstado, int PaisId, int? CodigoMapaId, bool Activo);

public record RegionDto(
    int Id,
    int ClienteId,
    string NombreRegion,
    int PaisId,
    string NombrePais,
    bool EsNacional,
    bool Activo,
    string CiudadesResumen,
    IReadOnlyList<int> CiudadIds);

public record RegionCreateRequest(
    int PaisId,
    string NombreRegion,
    bool EsNacional,
    bool Activo,
    IReadOnlyList<int> CiudadIds);

public record RegionUpdateRequest(
    int PaisId,
    string NombreRegion,
    bool EsNacional,
    bool Activo,
    IReadOnlyList<int> CiudadIds);

public record CiudadDto(
    int Id,
    string NombreCiudad,
    string? NombreCorto,
    int EstadoId,
    string NombreEstado,
    int PaisId,
    string NombrePais,
    bool CiudadPrincipal,
    bool Activo,
    int? Poblacion);

public record CiudadCreateRequest(
    string NombreCiudad,
    string? NombreCorto,
    int EstadoId,
    bool CiudadPrincipal,
    bool Activo,
    int? Poblacion);

public record CiudadUpdateRequest(
    string NombreCiudad,
    string? NombreCorto,
    int EstadoId,
    bool CiudadPrincipal,
    bool Activo,
    int? Poblacion);

public record PoblacionDto(
    int Id,
    int CiudadId,
    string NombreCiudad,
    int EstadoId,
    string NombreEstado,
    int PaisId,
    string NombrePais,
    int Anio,
    int Cantidad);

public record PoblacionCreateRequest(int CiudadId, int Anio, int Cantidad);

public record PoblacionUpdateRequest(int CiudadId, int Anio, int Cantidad);

/// <summary>Relación región-ciudad (referencia opcional).</summary>
public record AgrupacionRegionDto(int RegionId, int CiudadId, string? CiudadNombre);
