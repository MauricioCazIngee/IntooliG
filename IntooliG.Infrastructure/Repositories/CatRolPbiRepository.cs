using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public sealed class CatRolPbiRepository : ICatRolPbiRepository
{
    private readonly AppDbContext _db;

    public CatRolPbiRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<string?> GetNombreByIdAsync(int rolPbiId, CancellationToken cancellationToken = default)
    {
        var r = await _db.CatRolesPbi
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == rolPbiId, cancellationToken);
        return r?.NombreRol;
    }
}
