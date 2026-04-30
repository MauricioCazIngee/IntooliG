using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class PoblacionService : IPoblacionService
{
    private readonly IPoblacionRepository _repo;

    public PoblacionService(IPoblacionRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedListResult<PoblacionDto>> ListAsync(
        string? search,
        int? paisId,
        int? estadoId,
        int? anio,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (rows, total) = await _repo.ListAsync(search, paisId, estadoId, anio, page, pageSize, cancellationToken);
        var items = rows.Select(x => new PoblacionDto(
            x.Poblacion.Id,
            x.Poblacion.CiudadId,
            x.NombreCiudad,
            x.EstadoId,
            x.NombreEstado,
            x.PaisId,
            x.NombrePais,
            x.Poblacion.Anio,
            x.Poblacion.Cantidad)).ToList();

        return new PagedListResult<PoblacionDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<PoblacionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var (po, nc, ne, np, eid, pid) = await _repo.GetByIdAsync(id, cancellationToken);
        if (po is null || nc is null || ne is null || np is null || eid is null || pid is null)
            return null;
        return new PoblacionDto(po.Id, po.CiudadId, nc, eid.Value, ne, pid.Value, np, po.Anio, po.Cantidad);
    }

    public Task<IReadOnlyList<int>> GetAniosDisponiblesAsync(CancellationToken cancellationToken = default) =>
        _repo.GetAniosDisponiblesAsync(cancellationToken);

    public async Task<PoblacionDto> CreateAsync(PoblacionCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (await _repo.ExistsCiudadAnioAsync(request.CiudadId, request.Anio, null, cancellationToken))
            throw new InvalidOperationException("Ya existe un registro de población para esta ciudad y año.");

        var entity = new CatPoblacion
        {
            CiudadId = request.CiudadId,
            Anio = request.Anio,
            Cantidad = request.Cantidad
        };
        var created = await _repo.AddAsync(entity, cancellationToken);
        return (await GetByIdAsync(created.Id, cancellationToken))!;
    }

    public async Task<PoblacionDto?> UpdateAsync(int id, PoblacionUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (await GetByIdAsync(id, cancellationToken) is null)
            return null;
        if (await _repo.ExistsCiudadAnioAsync(request.CiudadId, request.Anio, id, cancellationToken))
            throw new InvalidOperationException("Ya existe otro registro de población para esta ciudad y año.");

        await _repo.UpdateAsync(new CatPoblacion
        {
            Id = id,
            CiudadId = request.CiudadId,
            Anio = request.Anio,
            Cantidad = request.Cantidad
        }, cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default) =>
        _repo.DeleteAsync(id, cancellationToken);

    private static int ComputeTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0) pageSize = 10;
        if (total <= 0) return 0;
        return (total + pageSize - 1) / pageSize;
    }
}
