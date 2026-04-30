using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Fuentes;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class VersionFuenteService : IVersionFuenteService
{
    private readonly IVersionFuenteRepository _repo;
    private readonly IFuenteRepository _fuentes;
    private readonly ICategoriaRepository _categorias;
    private readonly IBURepository _bus;
    private readonly IProductoRepository _productos;
    private readonly IVersionTVRepository _versionTvs;

    public VersionFuenteService(
        IVersionFuenteRepository repo,
        IFuenteRepository fuentes,
        ICategoriaRepository categorias,
        IBURepository bus,
        IProductoRepository productos,
        IVersionTVRepository versionTvs)
    {
        _repo = repo;
        _fuentes = fuentes;
        _categorias = categorias;
        _bus = bus;
        _productos = productos;
        _versionTvs = versionTvs;
    }

    public async Task<PagedListResult<VersionFuenteDto>> ListAsync(int clienteId, string? search, int? fuenteId, int? categoriaId, int page, int pageSize, bool? soloActivos, CancellationToken cancellationToken = default)
    {
        var (rows, total) = await _repo.ListAsync(clienteId, search, fuenteId, categoriaId, page, pageSize, soloActivos, cancellationToken);
        return new PagedListResult<VersionFuenteDto>
        {
            Items = rows.Select(Map).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<VersionFuenteDto?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        var row = await _repo.GetByIdAsync(id, clienteId, cancellationToken);
        return row is null ? null : Map(row);
    }

    public async Task<VersionFuenteDto> CreateAsync(int clienteId, VersionFuenteCreateRequest request, CancellationToken cancellationToken = default)
    {
        await ValidarAsync(clienteId, request.FuenteId, request.CategoriaId, request.BUId, request.ProductoId, request.VersionTVId, cancellationToken);
        var created = await _repo.AddAsync(new CatVersionFuente
        {
            FuenteId = request.FuenteId,
            NombreVersionFuente = request.NombreVersionFuente.Trim(),
            Activo = request.Activo,
            ProductoId = request.ProductoId,
            VersionTVId = request.VersionTVId,
            BUId = request.BUId,
            CategoriaId = request.CategoriaId
        }, cancellationToken);
        return (await GetByIdAsync(created.Id, clienteId, cancellationToken))!;
    }

    public async Task<VersionFuenteDto?> UpdateAsync(int id, int clienteId, VersionFuenteUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (await _repo.GetByIdAsync(id, clienteId, cancellationToken) is null)
            return null;
        await ValidarAsync(clienteId, request.FuenteId, request.CategoriaId, request.BUId, request.ProductoId, request.VersionTVId, cancellationToken);
        await _repo.UpdateAsync(new CatVersionFuente
        {
            Id = id,
            FuenteId = request.FuenteId,
            NombreVersionFuente = request.NombreVersionFuente.Trim(),
            Activo = request.Activo,
            ProductoId = request.ProductoId,
            VersionTVId = request.VersionTVId,
            BUId = request.BUId,
            CategoriaId = request.CategoriaId
        }, cancellationToken);
        return await GetByIdAsync(id, clienteId, cancellationToken);
    }

    public Task<bool> DesactivarAsync(int id, int clienteId, CancellationToken cancellationToken = default) =>
        _repo.SoftDeleteAsync(id, clienteId, cancellationToken);

    public async Task<IReadOnlyList<VersionFuenteDto>> ListByFuenteAsync(int fuenteId, int clienteId, CancellationToken cancellationToken = default)
    {
        var rows = await _repo.ListByFuenteAsync(fuenteId, clienteId, cancellationToken);
        return rows.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<VersionTVLookupDto>> ListVersionTVLookupAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _versionTvs.ListActivosAsync(cancellationToken);
        return rows.Select(x => new VersionTVLookupDto(x.Id, x.Nombre)).ToList();
    }

    private async Task ValidarAsync(int clienteId, int fuenteId, int categoriaId, int buId, int productoId, long versionTvId, CancellationToken ct)
    {
        if (await _fuentes.GetByIdAsync(fuenteId, ct) is null)
            throw new InvalidOperationException("Fuente no válida.");
        if (await _categorias.GetByIdAsync(categoriaId, clienteId, ct) is null)
            throw new InvalidOperationException("Categoría no válida.");
        if ((await _bus.GetByIdAsync(buId, clienteId, ct)).Bu is null)
            throw new InvalidOperationException("BU no válida.");
        if (await _productos.GetByIdForClienteAsync(productoId, clienteId, ct) is null)
            throw new InvalidOperationException("Producto no válido.");
        if (await _versionTvs.GetByIdAsync(versionTvId, ct) is null)
            throw new InvalidOperationException("Versión TV no válida.");
    }

    private static VersionFuenteDto Map(CatVersionFuente x) =>
        new(
            x.Id,
            x.NombreVersionFuente,
            x.FuenteId,
            x.Fuente?.NombreFuente ?? string.Empty,
            x.CategoriaId,
            x.Categoria?.NombreCategoria ?? string.Empty,
            x.Producto?.MarcaId ?? 0,
            x.Producto?.Marca?.NombreMarca ?? string.Empty,
            x.Activo,
            x.ProductoId,
            x.Producto?.Nombre ?? string.Empty,
            x.VersionTVId,
            x.VersionTV?.Nombre ?? string.Empty,
            x.BUId,
            x.BU?.NombreBU ?? string.Empty);

    private static int ComputeTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0) pageSize = 10;
        if (total <= 0) return 0;
        return (total + pageSize - 1) / pageSize;
    }
}
