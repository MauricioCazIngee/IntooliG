namespace IntooliG.Application.Features.RubrosConceptos;

public record RubroListRow(
    long Id,
    int RubroGeneralId,
    string RubroNombre,
    int CategoriaId,
    string CategoriaNombre,
    decimal ValorRubro,
    bool Activo,
    int ClienteId,
    string ClienteNombre);

public record RubroGeneralDto(int Id, string NombreRubro, bool Activo);
public record RubroGeneralCreateRequest(string NombreRubro);
public record RubroGeneralUpdateRequest(string NombreRubro, bool Activo);

public record RubroCombinacionDto(
    long Id,
    int RubroGeneralId,
    string RubroNombre,
    int CategoriaId,
    string CategoriaNombre,
    decimal ValorRubro,
    bool Activo,
    int ClienteId,
    string? ClienteNombre);

public record RubroCombinacionCreateRequest(
    int RubroGeneralId,
    int CategoriaId,
    decimal ValorRubro,
    bool Activo,
    int? ClienteId);

public record RubroCombinacionUpdateRequest(
    int RubroGeneralId,
    int CategoriaId,
    decimal ValorRubro,
    bool Activo,
    int? ClienteId);

public record ConceptoListItemDto(
    long Id,
    string NombreConcepto,
    int CategoriaId,
    string CategoriaNombre,
    int RubroGeneralId,
    string RubroNombre,
    int Posicion,
    decimal? Valor,
    bool Activo,
    bool Top);

public record ConceptoDetailDto(
    long Id,
    string NombreConcepto,
    int RubroGeneralId,
    string RubroNombre,
    int CategoriaId,
    string CategoriaNombre,
    int Posicion,
    decimal Valor,
    bool Activo,
    bool Top);

public record ConceptoCreateRequest(
    string NombreConcepto,
    int RubroGeneralId,
    int CategoriaId,
    int Posicion,
    decimal Valor,
    bool Activo,
    bool Top);

public record ConceptoUpdateRequest(
    string NombreConcepto,
    int RubroGeneralId,
    int CategoriaId,
    int Posicion,
    decimal Valor,
    bool Activo,
    bool Top);

public record ClienteLookupDto(int Id, string Nombre, bool Activo);
