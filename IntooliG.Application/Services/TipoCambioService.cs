using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.TipoCambio;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class TipoCambioService : ITipoCambioService
{
    private static readonly IReadOnlyList<TipoCambioMesDto> Meses =
    [
        new(1, "Enero"),
        new(2, "Febrero"),
        new(3, "Marzo"),
        new(4, "Abril"),
        new(5, "Mayo"),
        new(6, "Junio"),
        new(7, "Julio"),
        new(8, "Agosto"),
        new(9, "Septiembre"),
        new(10, "Octubre"),
        new(11, "Noviembre"),
        new(12, "Diciembre")
    ];

    private readonly ITipoCambioRepository _repo;
    private readonly IPaisRepository _paises;

    public TipoCambioService(ITipoCambioRepository repo, IPaisRepository paises)
    {
        _repo = repo;
        _paises = paises;
    }

    public async Task<PagedListResult<TipoCambioDto>> ListAsync(
        string? search,
        int? paisId,
        int? anio,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repo.ListAsync(search, paisId, anio, page, pageSize, cancellationToken);
        return new PagedListResult<TipoCambioDto>
        {
            Items = items.Select(Map).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<TipoCambioDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _repo.GetByIdAsync(id, cancellationToken);
        return row is null ? null : Map(row);
    }

    public async Task<TipoCambioDto> CreateAsync(TipoCambioRequestDto request, CancellationToken cancellationToken = default)
    {
        await ValidateRequestAsync(request, null, cancellationToken);
        var entity = new CatTipoCambio
        {
            PaisId = request.PaisId,
            Anio = request.Anio,
            Mes = request.Mes,
            TipoCambio = request.TipoCambio
        };

        var created = await _repo.AddAsync(entity, cancellationToken);
        return (await GetByIdAsync(created.Id, cancellationToken))!;
    }

    public async Task<TipoCambioDto?> UpdateAsync(int id, TipoCambioRequestDto request, CancellationToken cancellationToken = default)
    {
        if (await _repo.GetByIdAsync(id, cancellationToken) is null)
            return null;

        await ValidateRequestAsync(request, id, cancellationToken);

        await _repo.UpdateAsync(new CatTipoCambio
        {
            Id = id,
            PaisId = request.PaisId,
            Anio = request.Anio,
            Mes = request.Mes,
            TipoCambio = request.TipoCambio
        }, cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        _repo.DeleteAsync(id, cancellationToken);

    public async Task<IReadOnlyList<TipoCambioAnioDto>> ListAniosAsync(CancellationToken cancellationToken = default)
    {
        var years = await _repo.ListAniosAsync(cancellationToken);
        return years.Select(y => new TipoCambioAnioDto(y)).ToList();
    }

    public Task<IReadOnlyList<TipoCambioMesDto>> ListMesesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Meses);

    private async Task ValidateRequestAsync(TipoCambioRequestDto request, int? excludeId, CancellationToken cancellationToken)
    {
        if (request.Mes is < 1 or > 12)
            throw new InvalidOperationException("Mes inválido. Debe estar entre 1 y 12.");

        if (request.TipoCambio <= 0)
            throw new InvalidOperationException("El tipo de cambio debe ser mayor a 0.");

        if (await _paises.GetByIdAsync(request.PaisId, cancellationToken) is null)
            throw new InvalidOperationException("País no válido.");

        if (await _repo.ExistsAsync(request.PaisId, request.Anio, request.Mes, excludeId, cancellationToken))
            throw new InvalidOperationException("Ya existe un tipo de cambio para el País + Año + Mes seleccionado.");
    }

    private static TipoCambioDto Map(CatTipoCambio x) =>
        new(
            x.Id,
            x.PaisId,
            x.Pais?.NombrePais ?? string.Empty,
            x.Anio,
            x.Mes,
            Meses.FirstOrDefault(m => m.Mes == x.Mes)?.Nombre ?? x.Mes.ToString(),
            Math.Round(x.TipoCambio, 4));

    private static int ComputeTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0) pageSize = 10;
        if (total <= 0) return 0;
        return (total + pageSize - 1) / pageSize;
    }
}
