using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Medios;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class MedioService : IMedioService
{
    private readonly IMedioRepository _repo;

    public MedioService(IMedioRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedListResult<MedioDto>> ListAsync(
        string? search,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repo.ListAsync(search, page, pageSize, soloActivos, cancellationToken);
        var dtos = items.Select(m => new MedioDto(m.Id, m.NombreMedio, m.NombreMedioGenerico, m.Activo)).ToList();
        return new PagedListResult<MedioDto>
        {
            Items = dtos,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<MedioDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var m = await _repo.GetByIdAsync(id, cancellationToken);
        return m is null ? null : new MedioDto(m.Id, m.NombreMedio, m.NombreMedioGenerico, m.Activo);
    }

    public async Task<MedioDto> CreateAsync(MedioCreateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new CatMedio
        {
            NombreMedio = request.NombreMedio.Trim(),
            NombreMedioGenerico = request.NombreMedioGenerico.Trim(),
            Activo = request.Activo
        };
        var created = await _repo.AddAsync(entity, cancellationToken);
        return new MedioDto(created.Id, created.NombreMedio, created.NombreMedioGenerico, created.Activo);
    }

    public async Task<MedioDto?> UpdateAsync(int id, MedioUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (await _repo.GetByIdAsync(id, cancellationToken) is null)
            return null;
        await _repo.UpdateAsync(new CatMedio
        {
            Id = id,
            NombreMedio = request.NombreMedio.Trim(),
            NombreMedioGenerico = request.NombreMedioGenerico.Trim(),
            Activo = request.Activo
        }, cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public Task<bool> DesactivarAsync(int id, CancellationToken cancellationToken = default) =>
        _repo.SoftDeleteAsync(id, cancellationToken);

    public async Task<IReadOnlyList<MedioLookupDto>> ListActivosLookupAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _repo.ListActivosLookupAsync(cancellationToken);
        return rows.Select(x => new MedioLookupDto(x.Id, x.Nombre)).ToList();
    }

    private static int ComputeTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0) pageSize = 10;
        if (total <= 0) return 0;
        return (total + pageSize - 1) / pageSize;
    }
}
