using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Fuentes;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class FuenteService : IFuenteService
{
    private readonly IFuenteRepository _repo;
    private readonly IPaisRepository _paises;

    public FuenteService(IFuenteRepository repo, IPaisRepository paises)
    {
        _repo = repo;
        _paises = paises;
    }

    public async Task<PagedListResult<FuenteDto>> ListAsync(string? search, int? paisId, int page, int pageSize, bool? soloActivos, CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repo.ListAsync(search, paisId, page, pageSize, soloActivos, cancellationToken);
        return new PagedListResult<FuenteDto>
        {
            Items = items.Select(Map).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<FuenteDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _repo.GetByIdAsync(id, cancellationToken);
        return item is null ? null : Map(item);
    }

    public async Task<FuenteDto> CreateAsync(FuenteCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (await _paises.GetByIdAsync(request.PaisId, cancellationToken) is null)
            throw new InvalidOperationException("País no válido.");

        var entity = new CatFuente
        {
            NombreFuente = request.NombreFuente.Trim(),
            PaisId = request.PaisId,
            Activo = request.Activo
        };

        var created = await _repo.AddAsync(entity, cancellationToken);
        return (await GetByIdAsync(created.Id, cancellationToken))!;
    }

    public async Task<FuenteDto?> UpdateAsync(int id, FuenteUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (await _repo.GetByIdAsync(id, cancellationToken) is null)
            return null;
        if (await _paises.GetByIdAsync(request.PaisId, cancellationToken) is null)
            throw new InvalidOperationException("País no válido.");

        await _repo.UpdateAsync(new CatFuente
        {
            Id = id,
            NombreFuente = request.NombreFuente.Trim(),
            PaisId = request.PaisId,
            Activo = request.Activo
        }, cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public Task<bool> DesactivarAsync(int id, CancellationToken cancellationToken = default) =>
        _repo.SoftDeleteAsync(id, cancellationToken);

    public async Task<IReadOnlyList<FuenteLookupDto>> ListActivosLookupAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _repo.ListActivosLookupAsync(cancellationToken);
        return rows.Select(x => new FuenteLookupDto(x.Id, x.Nombre, x.Activo)).ToList();
    }

    public async Task<IReadOnlyList<FuenteDto>> ListByPaisAsync(int paisId, CancellationToken cancellationToken = default)
    {
        var rows = await _repo.ListByPaisAsync(paisId, cancellationToken);
        return rows.Select(Map).ToList();
    }

    private static FuenteDto Map(CatFuente x) =>
        new(x.Id, x.NombreFuente, x.PaisId, x.Pais?.NombrePais ?? string.Empty, x.Activo);

    private static int ComputeTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0) pageSize = 10;
        if (total <= 0) return 0;
        return (total + pageSize - 1) / pageSize;
    }
}
