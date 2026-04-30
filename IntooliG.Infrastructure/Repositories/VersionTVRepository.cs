using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class VersionTVRepository : IVersionTVRepository
{
    private readonly AppDbContext _db;

    public VersionTVRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CatVersionTV>> ListActivosAsync(CancellationToken cancellationToken = default)
    {
        return await _db.CatVersionTVs.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync(cancellationToken);
    }

    public Task<CatVersionTV?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        _db.CatVersionTVs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}
