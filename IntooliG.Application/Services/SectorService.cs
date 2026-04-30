using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Catalogo;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class SectorService : ISectorService
{
    private readonly ISectorRepository _repo;

    public SectorService(ISectorRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedListResult<SectorDto>> ListAsync(
        int clienteId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (rows, total) = await _repo.ListAsync(clienteId, search, page, pageSize, cancellationToken);
        var items = rows.Select(r => new SectorDto(r.Sector.Id, r.Sector.NombreSector, r.ClienteNombre, r.Sector.Activo)).ToList();
        return new PagedListResult<SectorDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<SectorDto?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        var (s, nombre) = await _repo.GetByIdAsync(id, clienteId, cancellationToken);
        if (s is null || nombre is null)
            return null;
        return new SectorDto(s.Id, s.NombreSector, nombre, s.Activo);
    }

    public async Task<SectorDto> CreateAsync(int clienteId, SectorCreateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new CatSector
        {
            ClienteId = clienteId,
            NombreSector = request.NombreSector.Trim(),
            Activo = true
        };
        var created = await _repo.AddAsync(entity, cancellationToken);
        var (_, nombre) = await _repo.GetByIdAsync(created.Id, clienteId, cancellationToken);
        return new SectorDto(created.Id, created.NombreSector, nombre ?? string.Empty, created.Activo);
    }

    public async Task<SectorDto?> UpdateAsync(int id, int clienteId, SectorUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var (existing, _) = await _repo.GetByIdAsync(id, clienteId, cancellationToken);
        if (existing is null)
            return null;

        await _repo.UpdateAsync(new CatSector
        {
            Id = id,
            ClienteId = clienteId,
            NombreSector = request.NombreSector.Trim(),
            Activo = request.Activo
        }, cancellationToken);

        return await GetByIdAsync(id, clienteId, cancellationToken);
    }

    public async Task<(bool Eliminado, bool Conflicto)> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        if (await GetByIdAsync(id, clienteId, cancellationToken) is null)
            return (false, false);

        var ok = await _repo.DeleteAsync(id, clienteId, cancellationToken);
        return ok ? (true, false) : (false, true);
    }

    private static int ComputeTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0) pageSize = 10;
        if (total <= 0) return 0;
        return (total + pageSize - 1) / pageSize;
    }
}
