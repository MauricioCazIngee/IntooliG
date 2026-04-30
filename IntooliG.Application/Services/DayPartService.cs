using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.DayPart;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class DayPartService : IDayPartService
{
    private readonly IDayPartRepository _repo;
    private readonly IPaisRepository _paises;
    private readonly IMedioRepository _medios;

    public DayPartService(IDayPartRepository repo, IPaisRepository paises, IMedioRepository medios)
    {
        _repo = repo;
        _paises = paises;
        _medios = medios;
    }

    public async Task<PagedListResult<DayPartDto>> ListAsync(
        int clienteId,
        string? search,
        int? paisId,
        int? medioId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repo.ListAsync(clienteId, search, paisId, medioId, page, pageSize, cancellationToken);
        return new PagedListResult<DayPartDto>
        {
            Items = items.Select(Map).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<DayPartDto?> GetByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default)
    {
        var row = await _repo.GetByIdAsync(id, clienteId, cancellationToken);
        return row is null ? null : Map(row);
    }

    public async Task<DayPartDto> CreateAsync(int clienteId, DayPartRequestDto request, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(clienteId, request, null, cancellationToken);
        var entity = new CatDaypart
        {
            ClienteId = clienteId,
            PaisId = request.PaisId,
            MedioId = request.MedioId,
            Nombre = request.Descripcion.Trim(),
            HoraInicio = request.HoraInicio,
            HoraFin = request.HoraFin
        };

        var created = await _repo.AddAsync(entity, cancellationToken);
        return (await GetByIdAsync(created.Id, clienteId, cancellationToken))!;
    }

    public async Task<DayPartDto?> UpdateAsync(long id, int clienteId, DayPartRequestDto request, CancellationToken cancellationToken = default)
    {
        if (await _repo.GetByIdAsync(id, clienteId, cancellationToken) is null)
            return null;

        await ValidateAsync(clienteId, request, id, cancellationToken);

        await _repo.UpdateAsync(new CatDaypart
        {
            Id = id,
            ClienteId = clienteId,
            PaisId = request.PaisId,
            MedioId = request.MedioId,
            Nombre = request.Descripcion.Trim(),
            HoraInicio = request.HoraInicio,
            HoraFin = request.HoraFin
        }, clienteId, cancellationToken);

        return await GetByIdAsync(id, clienteId, cancellationToken);
    }

    public Task<(bool Deleted, bool Conflict)> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default) =>
        _repo.DeleteAsync(id, clienteId, cancellationToken);

    public async Task<DayPartLookupDto> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        var (paises, medios) = await _repo.GetLookupAsync(cancellationToken);
        var horas = Enumerable.Range(0, 24).ToList();
        return new DayPartLookupDto(
            paises.Select(x => new DayPartLookupOptionDto(x.Id, x.Nombre)).ToList(),
            medios.Select(x => new DayPartLookupOptionDto(x.Id, x.Nombre)).ToList(),
            horas
        );
    }

    private async Task ValidateAsync(int clienteId, DayPartRequestDto request, long? excludeId, CancellationToken cancellationToken)
    {
        if (request.HoraInicio is < 0 or > 23 || request.HoraFin is < 0 or > 23)
            throw new InvalidOperationException("Las horas deben estar entre 0 y 23.");

        if (request.HoraInicio >= request.HoraFin)
            throw new InvalidOperationException("La hora de inicio debe ser menor que la hora de fin.");

        if (string.IsNullOrWhiteSpace(request.Descripcion))
            throw new InvalidOperationException("La descripción es requerida.");

        if (await _paises.GetByIdAsync(request.PaisId, cancellationToken) is null)
            throw new InvalidOperationException("País no válido.");

        var medio = await _medios.GetByIdAsync(request.MedioId, cancellationToken);
        if (medio is null || !medio.Activo)
            throw new InvalidOperationException("Medio no válido.");

        if (await _repo.ExistsByDescripcionAsync(
                clienteId,
                request.PaisId,
                request.MedioId,
                request.Descripcion.Trim(),
                excludeId,
                cancellationToken))
        {
            throw new InvalidOperationException("Ya existe un DayPart con la misma Descripción + País + Medio.");
        }
    }

    private static DayPartDto Map(CatDaypart x) =>
        new(
            x.Id,
            x.PaisId,
            x.Pais?.NombrePais ?? string.Empty,
            x.MedioId,
            x.Medio?.NombreMedio ?? string.Empty,
            x.Nombre,
            x.HoraInicio,
            x.HoraFin,
            ToHourText(x.HoraInicio),
            ToHourText(x.HoraFin));

    private static string ToHourText(int hour) => $"{Math.Clamp(hour, 0, 23):00}:00";

    private static int ComputeTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0) pageSize = 10;
        if (total <= 0) return 0;
        return (total + pageSize - 1) / pageSize;
    }
}
