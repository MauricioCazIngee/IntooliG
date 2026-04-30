using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RubrosConceptos;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class RubroGeneralService : IRubroGeneralService
{
    private readonly IRubroGeneralRepository _repo;

    public RubroGeneralService(IRubroGeneralRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedListResult<RubroGeneralDto>> ListAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repo.ListAsync(search, page, pageSize, cancellationToken);
        var dtos = items.Select(x => new RubroGeneralDto(x.Id, x.NombreRubro, x.Activo)).ToList();
        return new PagedListResult<RubroGeneralDto>
        {
            Items = dtos,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<RubroGeneralDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var x = await _repo.GetByIdAsync(id, cancellationToken);
        return x is null ? null : new RubroGeneralDto(x.Id, x.NombreRubro, x.Activo);
    }

    public async Task<RubroGeneralDto> CreateAsync(RubroGeneralCreateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new CatRubroGeneral
        {
            NombreRubro = request.NombreRubro.Trim(),
            Activo = true
        };
        var created = await _repo.AddAsync(entity, cancellationToken);
        return new RubroGeneralDto(created.Id, created.NombreRubro, created.Activo);
    }

    public async Task<RubroGeneralDto?> UpdateAsync(int id, RubroGeneralUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var x = await _repo.GetByIdAsync(id, cancellationToken);
        if (x is null)
            return null;
        x.NombreRubro = request.NombreRubro.Trim();
        x.Activo = request.Activo;
        await _repo.UpdateAsync(x, cancellationToken);
        return new RubroGeneralDto(x.Id, x.NombreRubro, x.Activo);
    }

    public Task<(bool Eliminado, bool Conflicto)> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        _repo.DeleteAsync(id, cancellationToken);

    private static int ComputeTotalPages(int total, int pageSize) =>
        Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
}
