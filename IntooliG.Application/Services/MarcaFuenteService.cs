using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Fuentes;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class MarcaFuenteService : IMarcaFuenteService
{
    private readonly IMarcaFuenteRepository _repo;
    private readonly IFuenteRepository _fuentes;
    private readonly IMarcaRepository _marcas;
    private readonly IProductoRepository _productos;

    public MarcaFuenteService(
        IMarcaFuenteRepository repo,
        IFuenteRepository fuentes,
        IMarcaRepository marcas,
        IProductoRepository productos)
    {
        _repo = repo;
        _fuentes = fuentes;
        _marcas = marcas;
        _productos = productos;
    }

    public async Task<PagedListResult<MarcaFuenteDto>> ListAsync(
        int clienteId,
        string? search,
        int? fuenteId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repo.ListAsync(clienteId, search, fuenteId, page, pageSize, cancellationToken);
        var dtos = items.Select(Map).ToList();
        return new PagedListResult<MarcaFuenteDto>
        {
            Items = dtos,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<MarcaFuenteDto?> GetByIdAsync(long id, int clienteId, CancellationToken cancellationToken = default)
    {
        var row = await _repo.GetByIdAsync(id, clienteId, cancellationToken);
        return row is null ? null : Map(row);
    }

    public async Task<MarcaFuenteDto> CreateAsync(
        int clienteId,
        MarcaFuenteCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateRelacionesAsync(clienteId, request.FuenteId, request.MarcaId, request.ProductoId, cancellationToken);

        var producto = (await _productos.GetByIdForClienteAsync(request.ProductoId, clienteId, cancellationToken))!;

        var entity = new CatMarcaProductoFuente
        {
            NombreMarcaFuente = request.NombreMarcaFuente.Trim(),
            NombreProductoFuente = producto.Nombre,
            FuenteId = request.FuenteId,
            MarcaId = request.MarcaId,
            ProductoId = request.ProductoId
        };
        var created = await _repo.AddAsync(entity, cancellationToken);
        return (await GetByIdAsync(created.Id, clienteId, cancellationToken))!;
    }

    public async Task<MarcaFuenteDto?> UpdateAsync(
        long id,
        int clienteId,
        MarcaFuenteUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await _repo.GetByIdAsync(id, clienteId, cancellationToken) is null)
            return null;

        await ValidateRelacionesAsync(clienteId, request.FuenteId, request.MarcaId, request.ProductoId, cancellationToken);
        var producto = (await _productos.GetByIdForClienteAsync(request.ProductoId, clienteId, cancellationToken))!;

        await _repo.UpdateAsync(new CatMarcaProductoFuente
        {
            Id = id,
            NombreMarcaFuente = request.NombreMarcaFuente.Trim(),
            NombreProductoFuente = producto.Nombre,
            FuenteId = request.FuenteId,
            MarcaId = request.MarcaId,
            ProductoId = request.ProductoId
        }, cancellationToken);

        return await GetByIdAsync(id, clienteId, cancellationToken);
    }

    public Task<bool> DeleteAsync(long id, int clienteId, CancellationToken cancellationToken = default) =>
        _repo.DeleteAsync(id, clienteId, cancellationToken);

    public async Task<IReadOnlyList<MarcaFuenteDto>> ListByFuenteAsync(
        int fuenteId,
        int clienteId,
        CancellationToken cancellationToken = default)
    {
        var rows = await _repo.ListByFuenteAsync(fuenteId, clienteId, cancellationToken);
        return rows.Select(Map).ToList();
    }

    private async Task ValidateRelacionesAsync(
        int clienteId,
        int fuenteId,
        int marcaId,
        int productoId,
        CancellationToken cancellationToken)
    {
        var fuente = await _fuentes.GetByIdAsync(fuenteId, cancellationToken);
        if (fuente is null)
            throw new InvalidOperationException("Fuente no válida.");

        var (marca, _) = await _marcas.GetByIdWithClienteAsync(marcaId, clienteId, cancellationToken);
        if (marca is null)
            throw new InvalidOperationException("Marca no válida para su cliente.");

        var producto = await _productos.GetByIdForClienteAsync(productoId, clienteId, cancellationToken);
        if (producto is null)
            throw new InvalidOperationException("Producto no válido.");
        if (producto.MarcaId != marcaId)
            throw new InvalidOperationException("El producto no pertenece a la marca seleccionada.");
    }

    private static MarcaFuenteDto Map(CatMarcaProductoFuente x)
    {
        var f = x.Fuente;
        var m = x.Marca;
        var p = x.Producto;
        var activo = (f?.Activo ?? false) && (m?.Activo ?? false) && (p?.Activo ?? false);
        return new MarcaFuenteDto(
            x.Id,
            x.NombreMarcaFuente,
            x.FuenteId,
            f?.NombreFuente ?? string.Empty,
            x.MarcaId,
            m?.NombreMarca ?? string.Empty,
            x.ProductoId,
            p?.Nombre ?? string.Empty,
            activo);
    }

    private static int ComputeTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0) pageSize = 10;
        if (total <= 0) return 0;
        return (total + pageSize - 1) / pageSize;
    }
}
