using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class CiudadService : ICiudadService
{
    private readonly ICiudadRepository _repo;

    public CiudadService(ICiudadRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedListResult<CiudadDto>> ListAsync(
        string? search,
        int? paisId,
        int? estadoId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default)
    {
        var (rows, total) = await _repo.ListAsync(search, paisId, estadoId, page, pageSize, soloActivos, cancellationToken);
        var items = rows.Select(x => new CiudadDto(
            x.Ciudad.Id,
            x.Ciudad.NombreCiudad,
            x.Ciudad.NombreCorto,
            x.Ciudad.EstadoId,
            x.NombreEstado,
            x.PaisId,
            x.NombrePais,
            x.Ciudad.CiudadPrincipal,
            x.Ciudad.Activo,
            x.Ciudad.Poblacion)).ToList();

        return new PagedListResult<CiudadDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<CiudadDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var (c, ne, np, paisId) = await _repo.GetByIdAsync(id, cancellationToken);
        if (c is null || ne is null || np is null || paisId is null)
            return null;
        return new CiudadDto(
            c.Id,
            c.NombreCiudad,
            c.NombreCorto,
            c.EstadoId,
            ne,
            paisId.Value,
            np,
            c.CiudadPrincipal,
            c.Activo,
            c.Poblacion);
    }

    public Task<IReadOnlyList<(int Id, string Nombre)>> ListForSelectorAsync(
        int? paisId,
        int? estadoId,
        CancellationToken cancellationToken = default) =>
        _repo.ListForSelectorAsync(paisId, estadoId, cancellationToken);

    public async Task<CiudadDto> CreateAsync(CiudadCreateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new CatCiudad
        {
            EstadoId = request.EstadoId,
            NombreCiudad = request.NombreCiudad.Trim(),
            NombreCorto = string.IsNullOrWhiteSpace(request.NombreCorto) ? null : request.NombreCorto.Trim(),
            CiudadPrincipal = request.CiudadPrincipal,
            Activo = request.Activo,
            Poblacion = request.Poblacion
        };
        var created = await _repo.AddAsync(entity, cancellationToken);
        return (await GetByIdAsync(created.Id, cancellationToken))!;
    }

    public async Task<CiudadDto?> UpdateAsync(int id, CiudadUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (await GetByIdAsync(id, cancellationToken) is null)
            return null;
        await _repo.UpdateAsync(new CatCiudad
        {
            Id = id,
            EstadoId = request.EstadoId,
            NombreCiudad = request.NombreCiudad.Trim(),
            NombreCorto = string.IsNullOrWhiteSpace(request.NombreCorto) ? null : request.NombreCorto.Trim(),
            CiudadPrincipal = request.CiudadPrincipal,
            Activo = request.Activo,
            Poblacion = request.Poblacion
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
