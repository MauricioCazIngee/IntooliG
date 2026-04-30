using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Catalogo;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class BUService : IBUService
{
    private readonly IBURepository _repo;

    public BUService(IBURepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedListResult<BUDto>> ListAsync(
        int clienteId,
        int? sectorId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (rows, total) = await _repo.ListAsync(clienteId, sectorId, search, page, pageSize, cancellationToken);
        var items = rows.Select(r => new BUDto(r.Bu.Id, r.Bu.NombreBU, r.Bu.SectorId, r.SectorNombre, r.Bu.Activo)).ToList();
        return new PagedListResult<BUDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<BUDto?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        var (b, sec) = await _repo.GetByIdAsync(id, clienteId, cancellationToken);
        if (b is null || sec is null)
            return null;
        return new BUDto(b.Id, b.NombreBU, b.SectorId, sec, b.Activo);
    }

    public async Task<BUDto> CreateAsync(int clienteId, BUCreateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new CatBU
        {
            ClienteId = clienteId,
            SectorId = request.SectorId,
            NombreBU = request.NombreBU.Trim(),
            Activo = true
        };
        var created = await _repo.AddAsync(entity, cancellationToken);
        return (await GetByIdAsync(created.Id, clienteId, cancellationToken))!;
    }

    public async Task<BUDto?> UpdateAsync(int id, int clienteId, BUUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (await GetByIdAsync(id, clienteId, cancellationToken) is null)
            return null;

        await _repo.UpdateAsync(new CatBU
        {
            Id = id,
            ClienteId = clienteId,
            SectorId = request.SectorId,
            NombreBU = request.NombreBU.Trim(),
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
