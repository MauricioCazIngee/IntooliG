using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Catalogo;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _repo;

    public CategoriaService(ICategoriaRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedListResult<CategoriaDto>> ListAsync(
        int clienteId,
        int? buId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (rows, total) = await _repo.ListAsync(clienteId, buId, search, page, pageSize, cancellationToken);
        var items = rows
            .Select(c => new CategoriaDto(c.Id, c.NombreCategoria, c.NombreCorto, null, c.Activo))
            .ToList();
        return new PagedListResult<CategoriaDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<CategoriaDto?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        var c = await _repo.GetByIdAsync(id, clienteId, cancellationToken);
        return c is null ? null : new CategoriaDto(c.Id, c.NombreCategoria, c.NombreCorto, null, c.Activo);
    }

    public async Task<CategoriaDto> CreateAsync(int clienteId, CategoriaCreateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new CatCategoria
        {
            ClienteId = clienteId,
            NombreCategoria = request.NombreCategoria.Trim(),
            NombreCorto = string.IsNullOrWhiteSpace(request.NombreCorto) ? null : request.NombreCorto.Trim(),
            Activo = true
        };
        var created = await _repo.AddAsync(entity, cancellationToken);
        return (await GetByIdAsync(created.Id, clienteId, cancellationToken))!;
    }

    public async Task<CategoriaDto?> UpdateAsync(int id, int clienteId, CategoriaUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (await GetByIdAsync(id, clienteId, cancellationToken) is null)
            return null;

        await _repo.UpdateAsync(new CatCategoria
        {
            Id = id,
            ClienteId = clienteId,
            NombreCategoria = request.NombreCategoria.Trim(),
            NombreCorto = string.IsNullOrWhiteSpace(request.NombreCorto) ? null : request.NombreCorto.Trim(),
            Activo = request.Activo
        }, cancellationToken);

        return await GetByIdAsync(id, clienteId, cancellationToken);
    }

    public async Task<(bool Eliminado, bool Conflicto)> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        if (await GetByIdAsync(id, clienteId, cancellationToken) is null)
            return (false, false);

        var ok = await _repo.DeleteAsync(id, clienteId, cancellationToken);
        return ok ? (true, false) : (false, true);
    }

    private static int ComputeTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0) pageSize = 10;
        if (total <= 0) return 0;
        return (total + pageSize - 1) / pageSize;
    }
}
