namespace IntooliG.Application.Features.Fuentes;

public record FuenteDto(int Id, string NombreFuente, int PaisId, string NombrePais, bool Activo);

public record FuenteCreateRequest(string NombreFuente, int PaisId, bool Activo);

public record FuenteUpdateRequest(string NombreFuente, int PaisId, bool Activo);

public record FuenteLookupDto(int Id, string Nombre, bool Activo);

public record MarcaFuenteDto(
    long Id,
    string NombreMarcaFuente,
    int FuenteId,
    string NombreFuente,
    int MarcaId,
    string NombreMarca,
    int ProductoId,
    string NombreProducto,
    bool Activo);

public record MarcaFuenteCreateRequest(
    string NombreMarcaFuente,
    int FuenteId,
    int MarcaId,
    int ProductoId);

public record MarcaFuenteUpdateRequest(
    string NombreMarcaFuente,
    int FuenteId,
    int MarcaId,
    int ProductoId);

public record VersionFuenteDto(
    int Id,
    string NombreVersionFuente,
    int FuenteId,
    string NombreFuente,
    int CategoriaId,
    string NombreCategoria,
    int MarcaId,
    string NombreMarca,
    bool Activo,
    int ProductoId,
    string NombreProducto,
    long VersionTVId,
    string NombreVersionTV,
    int BUId,
    string NombreBU);

public record VersionFuenteCreateRequest(
    string NombreVersionFuente,
    int FuenteId,
    int CategoriaId,
    int BUId,
    int ProductoId,
    long VersionTVId,
    bool Activo);

public record VersionFuenteUpdateRequest(
    string NombreVersionFuente,
    int FuenteId,
    int CategoriaId,
    int BUId,
    int ProductoId,
    long VersionTVId,
    bool Activo);

public record VersionTVLookupDto(long Id, string Nombre);
