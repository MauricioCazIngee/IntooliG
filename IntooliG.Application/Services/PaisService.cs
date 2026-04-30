using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class PaisService : IPaisService
{
    private readonly IPaisRepository _repo;

    public PaisService(IPaisRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedListResult<PaisDto>> ListAsync(
        string? search,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repo.ListAsync(search, page, pageSize, soloActivos, cancellationToken);
        var dtos = items.Select(p => new PaisDto(p.Id, p.NombrePais, p.Activo)).ToList();
        return new PagedListResult<PaisDto>
        {
            Items = dtos,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<PaisDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var p = await _repo.GetByIdAsync(id, cancellationToken);
        return p is null ? null : new PaisDto(p.Id, p.NombrePais, p.Activo);
    }

    public async Task<PaisDto> CreateAsync(PaisCreateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new CatPais
        {
            NombrePais = request.NombrePais.Trim(),
            Activo = request.Activo
        };
        var created = await _repo.AddAsync(entity, cancellationToken);
        return new PaisDto(created.Id, created.NombrePais, created.Activo);
    }

    public async Task<PaisDto?> UpdateAsync(int id, PaisUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return null;
        await _repo.UpdateAsync(new CatPais
        {
            Id = id,
            NombrePais = request.NombrePais.Trim(),
            Activo = request.Activo
        }, cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public Task<bool> DesactivarAsync(int id, CancellationToken cancellationToken = default) =>
        _repo.SoftDeleteAsync(id, cancellationToken);

    private static int ComputeTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0) pageSize = 10;
        if (total <= 0) return 0;
        return (total + pageSize - 1) / pageSize;
    }
}
