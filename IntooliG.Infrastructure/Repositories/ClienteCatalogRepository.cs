using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Features.RubrosConceptos;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class ClienteCatalogRepository : IClienteCatalogRepository
{
    private readonly AppDbContext _db;

    public ClienteCatalogRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ClienteLookupDto>> ListActivosAsync(CancellationToken cancellationToken = default)
    {
        var items = await _db.CatClientes.AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .Select(c => new ClienteLookupDto(c.Id, c.Nombre, c.Activo))
            .ToListAsync(cancellationToken);
        return items;
    }
}
