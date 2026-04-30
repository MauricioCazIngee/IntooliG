using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RubrosConceptos;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class RubroService : IRubroService
{
    private readonly IRubroRepository _repo;
    private readonly IRubroGeneralRepository _rubrosGenerales;
    private readonly ICategoriaRepository _categorias;

    public RubroService(
        IRubroRepository repo,
        IRubroGeneralRepository rubrosGenerales,
        ICategoriaRepository categorias)
    {
        _repo = repo;
        _rubrosGenerales = rubrosGenerales;
        _categorias = categorias;
    }

    public async Task<PagedListResult<RubroCombinacionDto>> ListAsync(
        int clienteId,
        int? categoriaId,
        bool? activo,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repo.ListAsync(clienteId, categoriaId, activo, search, page, pageSize, cancellationToken);
        var dtos = items.Select(x => new RubroCombinacionDto(
            x.Id,
            x.RubroGeneralId,
            x.RubroNombre,
            x.CategoriaId,
            x.CategoriaNombre,
            x.ValorRubro,
            x.Activo,
            x.ClienteId,
            x.ClienteNombre)).ToList();
        return new PagedListResult<RubroCombinacionDto>
        {
            Items = dtos,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<RubroCombinacionDto?> GetByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default)
    {
        var row = await _repo.GetListRowByIdAsync(id, clienteId, cancellationToken);
        if (row is null)
            return null;

        return new RubroCombinacionDto(
            row.Id,
            row.RubroGeneralId,
            row.RubroNombre,
            row.CategoriaId,
            row.CategoriaNombre,
            row.ValorRubro,
            row.Activo,
            row.ClienteId,
            row.ClienteNombre);
    }

    public async Task<RubroCombinacionDto> CreateAsync(int clienteId, RubroCombinacionCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request.ClienteId is not null && request.ClienteId != clienteId)
            throw new InvalidOperationException("El cliente indicado no coincide con la sesión.");

        await EnsureRubroGeneralExistsAsync(request.RubroGeneralId, cancellationToken);
        await EnsureCategoriaForClienteAsync(request.CategoriaId, clienteId, cancellationToken);

        if (await _repo.ExistsCombinacionAsync(request.RubroGeneralId, request.CategoriaId, clienteId, null, cancellationToken))
            throw new InvalidOperationException("Ya existe una combinación para este rubro, categoría y cliente.");

        var entity = new CatRubro
        {
            RubroGeneralId = request.RubroGeneralId,
            CategoriaId = request.CategoriaId,
            ValorRubro = request.ValorRubro,
            Activo = request.Activo,
            ClienteId = clienteId
        };
        var created = await _repo.AddAsync(entity, cancellationToken);
        var dto = await GetByIdAsync(created.Id, clienteId, cancellationToken);
        return dto ?? throw new InvalidOperationException("No fue posible cargar el rubro creado.");
    }

    public async Task<RubroCombinacionDto?> UpdateAsync(long id, int clienteId, RubroCombinacionUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, clienteId, cancellationToken);
        if (existing is null)
            return null;

        if (request.ClienteId is not null && request.ClienteId != clienteId)
            throw new InvalidOperationException("El cliente indicado no coincide con la sesión.");

        await EnsureRubroGeneralExistsAsync(request.RubroGeneralId, cancellationToken);
        await EnsureCategoriaForClienteAsync(request.CategoriaId, clienteId, cancellationToken);

        if (await _repo.ExistsCombinacionAsync(request.RubroGeneralId, request.CategoriaId, clienteId, id, cancellationToken))
            throw new InvalidOperationException("Ya existe otra combinación para este rubro, categoría y cliente.");

        var entity = new CatRubro
        {
            Id = id,
            RubroGeneralId = request.RubroGeneralId,
            CategoriaId = request.CategoriaId,
            ValorRubro = request.ValorRubro,
            Activo = request.Activo,
            ClienteId = clienteId
        };
        await _repo.UpdateAsync(entity, cancellationToken);
        return await GetByIdAsync(id, clienteId, cancellationToken);
    }

    public Task<(bool Eliminado, bool Conflicto)> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default) =>
        _repo.DeleteAsync(id, clienteId, cancellationToken);

    private async Task EnsureRubroGeneralExistsAsync(int id, CancellationToken cancellationToken)
    {
        if (await _rubrosGenerales.GetByIdAsync(id, cancellationToken) is null)
            throw new InvalidOperationException("Rubro general no encontrado.");
    }

    private async Task EnsureCategoriaForClienteAsync(int categoriaId, int clienteId, CancellationToken cancellationToken)
    {
        if (await _categorias.GetByIdAsync(categoriaId, clienteId, cancellationToken) is null)
            throw new InvalidOperationException("Categoría no encontrada para el cliente.");
    }

    private static int ComputeTotalPages(int total, int pageSize) =>
        Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
}
