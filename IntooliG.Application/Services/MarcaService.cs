using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Catalogo;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class MarcaService : IMarcaService
{
    private readonly IMarcaRepository _repo;

    public MarcaService(IMarcaRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedListResult<MarcaDto>> ListAsync(
        int clienteId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (rows, total) = await _repo.ListAsync(clienteId, search, page, pageSize, cancellationToken);
        var ids = rows.Select(r => r.Marca.Id).ToList();
        var resumen = await _repo.GetProductosResumenPorMarcaAsync(ids, cancellationToken);

        var items = rows.Select(r => new MarcaDto(
            r.Marca.Id,
            r.Marca.NombreMarca,
            r.Marca.Activo,
            r.ClienteNombre,
            r.Marca.Logo is { Length: > 0 },
            resumen.GetValueOrDefault(r.Marca.Id, string.Empty))).ToList();

        return new PagedListResult<MarcaDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = ComputeTotalPages(total, pageSize)
        };
    }

    public async Task<MarcaDetailDto?> GetByIdAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        var (m, clienteNombre) = await _repo.GetByIdWithClienteAsync(id, clienteId, cancellationToken);
        if (m is null || clienteNombre is null)
            return null;

        var productos = await _repo.GetProductosByMarcaAsync(id, clienteId, cancellationToken);
        var logoB64 = m.Logo is { Length: > 0 } ? Convert.ToBase64String(m.Logo) : null;

        return new MarcaDetailDto(
            m.Id,
            m.NombreMarca,
            m.Activo,
            m.ClienteId,
            clienteNombre,
            m.Logo is { Length: > 0 },
            logoB64,
            productos.Select(p => new ProductoDto(p.Id, p.Nombre, p.Activo)).ToList());
    }

    public async Task<MarcaDetailDto> CreateAsync(int clienteId, MarcaCreateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new CatMarca
        {
            ClienteId = clienteId,
            NombreMarca = request.NombreMarca.Trim(),
            Activo = request.Activo,
            Logo = DecodeLogo(request.LogoBase64)
        };

        var created = await _repo.AddAsync(entity, cancellationToken);
        await _repo.ReplaceProductosAsync(
            created.Id,
            request.ProductosNombres?.ToList() ?? new List<string>(),
            cancellationToken);

        return (await GetByIdAsync(created.Id, clienteId, cancellationToken))!;
    }

    public async Task<MarcaDetailDto?> UpdateAsync(int id, int clienteId, MarcaUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var tracked = await _repo.GetTrackedByIdAsync(id, clienteId, cancellationToken);
        if (tracked is null)
            return null;

        tracked.NombreMarca = request.NombreMarca.Trim();
        tracked.Activo = request.Activo;

        if (request.LogoBase64 is not null)
            tracked.Logo = string.IsNullOrWhiteSpace(request.LogoBase64) ? null : DecodeLogo(request.LogoBase64);

        await _repo.SaveChangesAsync(cancellationToken);

        await _repo.ReplaceProductosAsync(id, request.ProductosNombres?.ToList() ?? new List<string>(), cancellationToken);

        return await GetByIdAsync(id, clienteId, cancellationToken);
    }

    public async Task<(bool Eliminado, bool Conflicto)> DeleteAsync(int id, int clienteId, CancellationToken cancellationToken = default)
    {
        if (await GetByIdAsync(id, clienteId, cancellationToken) is null)
            return (false, false);

        var ok = await _repo.DeleteAsync(id, clienteId, cancellationToken);
        return ok ? (true, false) : (false, true);
    }

    private static byte[]? DecodeLogo(string? logoBase64)
    {
        if (string.IsNullOrWhiteSpace(logoBase64))
            return null;
        var s = logoBase64.Trim();
        var comma = s.IndexOf(',');
        if (s.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && comma > 0)
            s = s[(comma + 1)..];
        try
        {
            return Convert.FromBase64String(s);
        }
        catch
        {
            return null;
        }
    }

    private static int ComputeTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0) pageSize = 10;
        if (total <= 0) return 0;
        return (total + pageSize - 1) / pageSize;
    }
}
