namespace IntooliG.Application.Abstractions.Persistence;

public interface IConceptoRepository
{
    Task<(IReadOnlyList<ConceptoListRow> Items, int Total)> ListAsync(
        int clienteId,
        int? categoriaId,
        int? rubroGeneralId,
        bool? activo,
        bool? top,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<(ConceptoListRow? Row, bool Found)> GetDetailRowByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default);

    Task<long> CreateFullAsync(
        int rubroGeneralId,
        int categoriaId,
        string nombreConcepto,
        int posicion,
        decimal valor,
        bool activo,
        bool top,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateFullAsync(
        long id,
        int clienteId,
        int rubroGeneralId,
        int categoriaId,
        string nombreConcepto,
        int posicion,
        decimal valor,
        bool activo,
        bool top,
        CancellationToken cancellationToken = default);

    Task<(bool Eliminado, bool Conflicto)> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default);
}

/// <summary>Fila de listado de conceptos (repo → servicio).</summary>
public record ConceptoListRow(
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
