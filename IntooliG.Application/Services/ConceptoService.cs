using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RubrosConceptos;

namespace IntooliG.Application.Services;

public class ConceptoService : IConceptoService
{
    private readonly IConceptoRepository _repo;
    private readonly IRubroRepository _rubros;
    private readonly ICategoriaRepository _categorias;

    public ConceptoService(
        IConceptoRepository repo,
        IRubroRepository rubros,
        ICategoriaRepository categorias)
    {
        _repo = repo;
        _rubros = rubros;
        _categorias = categorias;
    }

    public async Task<PagedListResult<ConceptoListItemDto>> ListAsync(
        int clienteId,
        int? categoriaId,
        int? rubroGeneralId,
        bool? activo,
        bool? top,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repo.ListAsync(
            clienteId, categoriaId, rubroGeneralId, activo, top, search, page, pageSize, cancellationToken);
        var dtos = items.Select(x => new ConceptoListItemDto(
            x.Id,
            x.NombreConcepto,
            x.CategoriaId,
            x.CategoriaNombre,
            x.RubroGeneralId,
            x.RubroNombre,
            x.Posicion,
            x.Valor,
            x.Activo,
            x.Top)).ToList();
        return new PagedListResult<ConceptoListItemDto>
        {
            Items = dtos,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<ConceptoDetailDto?> GetByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default)
    {
        var (row, found) = await _repo.GetDetailRowByIdAsync(id, clienteId, cancellationToken);
        if (row is null)
            return null;

        return new ConceptoDetailDto(
            row.Id,
            row.NombreConcepto,
            row.RubroGeneralId,
            row.RubroNombre,
            row.CategoriaId,
            row.CategoriaNombre,
            row.Posicion,
            row.Valor ?? 0m,
            row.Activo,
            row.Top);
    }

    public async Task<ConceptoDetailDto> CreateAsync(int clienteId, ConceptoCreateRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureCategoriaForClienteAsync(request.CategoriaId, clienteId, cancellationToken);
        var rubroOk = await _rubros.ExistsCombinacionAsync(
            request.RubroGeneralId,
            request.CategoriaId,
            clienteId,
            null,
            cancellationToken);
        if (!rubroOk)
            throw new InvalidOperationException("Debe existir una combinación Rubro+Categoría para el cliente antes de crear el concepto.");

        var id = await _repo.CreateFullAsync(
            request.RubroGeneralId,
            request.CategoriaId,
            request.NombreConcepto,
            request.Posicion,
            request.Valor,
            request.Activo,
            request.Top,
            cancellationToken);

        var dto = await GetByIdAsync(id, clienteId, cancellationToken);
        return dto ?? throw new InvalidOperationException("No fue posible cargar el concepto creado.");
    }

    public async Task<ConceptoDetailDto?> UpdateAsync(long id, int clienteId, ConceptoUpdateRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureCategoriaForClienteAsync(request.CategoriaId, clienteId, cancellationToken);
        var rubroOk = await _rubros.ExistsCombinacionAsync(
            request.RubroGeneralId,
            request.CategoriaId,
            clienteId,
            null,
            cancellationToken);
        if (!rubroOk)
            throw new InvalidOperationException("Debe existir una combinación Rubro+Categoría para el cliente.");

        var ok = await _repo.UpdateFullAsync(
            id,
            clienteId,
            request.RubroGeneralId,
            request.CategoriaId,
            request.NombreConcepto,
            request.Posicion,
            request.Valor,
            request.Activo,
            request.Top,
            cancellationToken);
        return ok ? await GetByIdAsync(id, clienteId, cancellationToken) : null;
    }

    public Task<(bool Eliminado, bool Conflicto)> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default) =>
        _repo.DeleteAsync(id, clienteId, cancellationToken);

    private async Task EnsureCategoriaForClienteAsync(int categoriaId, int clienteId, CancellationToken cancellationToken)
    {
        if (await _categorias.GetByIdAsync(categoriaId, clienteId, cancellationToken) is null)
            throw new InvalidOperationException("Categoría no encontrada para el cliente.");
    }

    private static int ComputeTotalPages(int total, int pageSize) =>
        Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
}
