namespace IntooliG.Application.Features.Catalogo;

public record SectorDto(int Id, string NombreSector, string ClienteNombre, bool Activo);

public record SectorCreateRequest(string NombreSector);
public record SectorUpdateRequest(string NombreSector, bool Activo);

public record BUDto(int Id, string NombreBU, int SectorId, string SectorNombre, bool Activo);

public record BUCreateRequest(string NombreBU, int SectorId);
public record BUUpdateRequest(string NombreBU, int SectorId, bool Activo);

/// <summary>
/// En la BD actual, tbCatCategoria no tiene FiBUid; BuNombre queda vacío hasta migración.
/// </summary>
public record CategoriaDto(int Id, string NombreCategoria, string? NombreCorto, string? BuNombre, bool Activo);

public record CategoriaCreateRequest(string NombreCategoria, string? NombreCorto);
public record CategoriaUpdateRequest(string NombreCategoria, string? NombreCorto, bool Activo);

public record ProductoDto(int Id, string Nombre, bool Activo);

public record MarcaDto(
    int Id,
    string NombreMarca,
    bool Activo,
    string ClienteNombre,
    bool TieneLogo,
    string ProductosResumen);

public record MarcaDetailDto(
    int Id,
    string NombreMarca,
    bool Activo,
    int ClienteId,
    string ClienteNombre,
    bool TieneLogo,
    string? LogoBase64,
    IReadOnlyList<ProductoDto> Productos);

public record MarcaCreateRequest(
    string NombreMarca,
    bool Activo,
    string? LogoBase64,
    IReadOnlyList<string> ProductosNombres);

public record MarcaUpdateRequest(
    string NombreMarca,
    bool Activo,
    string? LogoBase64,
    IReadOnlyList<string> ProductosNombres);
