using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Medios;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class MedioClienteService : IMedioClienteService
{
    private readonly IMedioClienteRepository _repo;
    private readonly IMedioRepository _medios;

    public MedioClienteService(IMedioClienteRepository repo, IMedioRepository medios)
    {
        _repo = repo;
        _medios = medios;
    }

    public async Task<PagedListResult<MedioClienteDto>> ListAsync(
        string? search,
        int? clienteId,
        int? medioId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (rows, total) = await _repo.ListAsync(search, clienteId, medioId, page, pageSize, cancellationToken);
        var items = rows.Select(x => new MedioClienteDto(
            x.Row.MedioId,
            x.Row.ClienteId,
            x.NombreMedio,
            x.NombreCliente,
            x.Row.EsNacional)).ToList();

        return new PagedListResult<MedioClienteDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<MedioClienteDto?> GetByKeyAsync(int medioId, int clienteId, CancellationToken cancellationToken = default)
    {
        var (row, nm, nc) = await _repo.GetByKeyAsync(medioId, clienteId, cancellationToken);
        if (row is null || nm is null || nc is null)
            return null;
        return new MedioClienteDto(row.MedioId, row.ClienteId, nm, nc, row.EsNacional);
    }

    public async Task<IReadOnlyList<MedioClientePorClienteDto>> ListByClienteAsync(
        int clienteId,
        CancellationToken cancellationToken = default)
    {
        var rows = await _repo.ListByClienteAsync(clienteId, cancellationToken);
        return rows.Select(x => new MedioClientePorClienteDto(x.MedioId, x.NombreMedio, x.EsNacional)).ToList();
    }

    public async Task<MedioClienteDto> CreateAsync(MedioClienteCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (await _repo.ExistsAsync(request.MedioId, request.ClienteId, cancellationToken))
            throw new InvalidOperationException("Ya existe la asignación de este medio al cliente.");

        var medio = await _medios.GetByIdAsync(request.MedioId, cancellationToken);
        if (medio is null || !medio.Activo)
            throw new InvalidOperationException("Medio no encontrado o inactivo.");

        var entity = new MedioCliente
        {
            MedioId = request.MedioId,
            ClienteId = request.ClienteId,
            EsNacional = request.EsNacional
        };
        await _repo.AddAsync(entity, cancellationToken);
        return (await GetByKeyAsync(request.MedioId, request.ClienteId, cancellationToken))!;
    }

    public async Task<MedioClienteDto?> UpdateAsync(
        int medioId,
        int clienteId,
        MedioClienteUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await GetByKeyAsync(medioId, clienteId, cancellationToken) is null)
            return null;
        await _repo.UpdateAsync(new MedioCliente
        {
            MedioId = medioId,
            ClienteId = clienteId,
            EsNacional = request.EsNacional
        }, cancellationToken);
        return await GetByKeyAsync(medioId, clienteId, cancellationToken);
    }

    public Task<bool> DeleteAsync(int medioId, int clienteId, CancellationToken cancellationToken = default) =>
        _repo.DeleteAsync(medioId, clienteId, cancellationToken);

    private static int ComputeTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0) pageSize = 10;
        if (total <= 0) return 0;
        return (total + pageSize - 1) / pageSize;
    }
}
