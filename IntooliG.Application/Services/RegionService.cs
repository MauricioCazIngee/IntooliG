using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class RegionService : IRegionService
{
    private readonly IRegionRepository _repo;

    public RegionService(IRegionRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedListResult<RegionDto>> ListAsync(
        int clienteId,
        string? search,
        int? paisId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default)
    {
        var (rows, total) = await _repo.ListAsync(clienteId, search, paisId, page, pageSize, soloActivos, cancellationToken);
        var items = new List<RegionDto>();
        foreach (var (r, nombrePais) in rows)
        {
            var resumen = await _repo.GetCiudadesResumenAsync(r.Id, cancellationToken);
            var ids = await _repo.GetCiudadIdsByRegionAsync(r.Id, cancellationToken);
            items.Add(new RegionDto(
                r.Id,
                r.ClienteId,
                r.NombreRegion,
                r.PaisId,
                nombrePais,
                r.EsNacional,
                r.Activo,
                resumen,
                ids));
        }

        return new PagedListResult<RegionDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<RegionDto?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        var (r, nombrePais) = await _repo.GetByIdAsync(id, clienteId, cancellationToken);
        if (r is null || nombrePais is null)
            return null;
        var resumen = await _repo.GetCiudadesResumenAsync(r.Id, cancellationToken);
        var ids = await _repo.GetCiudadIdsByRegionAsync(r.Id, cancellationToken);
        return new RegionDto(
            r.Id,
            r.ClienteId,
            r.NombreRegion,
            r.PaisId,
            nombrePais,
            r.EsNacional,
            r.Activo,
            resumen,
            ids);
    }

    public async Task<RegionDto> CreateAsync(int clienteId, RegionCreateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new CatRegion
        {
            ClienteId = clienteId,
            PaisId = request.PaisId,
            NombreRegion = request.NombreRegion.Trim(),
            EsNacional = request.EsNacional,
            Activo = request.Activo
        };
        var created = await _repo.CreateWithCiudadesAsync(entity, request.CiudadIds, cancellationToken);
        return (await GetByIdAsync(created.Id, clienteId, cancellationToken))!;
    }

    public async Task<RegionDto?> UpdateAsync(int id, int clienteId, RegionUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var (existing, _) = await _repo.GetByIdAsync(id, clienteId, cancellationToken);
        if (existing is null)
            return null;

        var entity = new CatRegion
        {
            Id = id,
            ClienteId = clienteId,
            PaisId = request.PaisId,
            NombreRegion = request.NombreRegion.Trim(),
            EsNacional = request.EsNacional,
            Activo = request.Activo
        };
        await _repo.UpdateWithCiudadesAsync(entity, request.CiudadIds, cancellationToken);
        return await GetByIdAsync(id, clienteId, cancellationToken);
    }

    public Task<bool> DesactivarAsync(int id, int clienteId, CancellationToken cancellationToken = default) =>
        _repo.SoftDeleteAsync(id, clienteId, cancellationToken);

    private static int ComputeTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0) pageSize = 10;
        if (total <= 0) return 0;
        return (total + pageSize - 1) / pageSize;
    }
}
