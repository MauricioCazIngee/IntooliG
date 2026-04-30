using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RegionCiudad;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class EstadoService : IEstadoService
{
    private readonly IEstadoRepository _repo;

    public EstadoService(IEstadoRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedListResult<EstadoDto>> ListAsync(
        string? search,
        int? paisId,
        int page,
        int pageSize,
        bool? soloActivos,
        CancellationToken cancellationToken = default)
    {
        var (rows, total) = await _repo.ListAsync(search, paisId, page, pageSize, soloActivos, cancellationToken);
        var items = rows.Select(x => new EstadoDto(
            x.Estado.Id,
            x.Estado.NombreEstado,
            x.Estado.PaisId,
            x.NombrePais,
            x.Estado.CodigoMapaId,
            x.NombreCodigoMapa,
            x.Estado.Activo)).ToList();

        return new PagedListResult<EstadoDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<EstadoDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var (e, np, cm) = await _repo.GetByIdAsync(id, cancellationToken);
        if (e is null || np is null)
            return null;
        return new EstadoDto(e.Id, e.NombreEstado, e.PaisId, np, e.CodigoMapaId, cm, e.Activo);
    }

    public Task<IReadOnlyList<(int Id, string Nombre)>> GetCodigosMapaAsync(CancellationToken cancellationToken = default) =>
        _repo.ListCodigosMapaAsync(cancellationToken);

    public async Task<EstadoDto> CreateAsync(EstadoCreateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new CatEstado
        {
            NombreEstado = request.NombreEstado.Trim(),
            PaisId = request.PaisId,
            CodigoMapaId = request.CodigoMapaId,
            Activo = request.Activo
        };
        var created = await _repo.AddAsync(entity, cancellationToken);
        return (await GetByIdAsync(created.Id, cancellationToken))!;
    }

    public async Task<EstadoDto?> UpdateAsync(int id, EstadoUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (await GetByIdAsync(id, cancellationToken) is null)
            return null;
        await _repo.UpdateAsync(new CatEstado
        {
            Id = id,
            PaisId = request.PaisId,
            NombreEstado = request.NombreEstado.Trim(),
            CodigoMapaId = request.CodigoMapaId,
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
