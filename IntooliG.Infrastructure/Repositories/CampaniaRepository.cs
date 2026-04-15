using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Domain.Entities;
using IntooliG.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntooliG.Infrastructure.Repositories;

public class CampaniaRepository : ICampaniaRepository
{
    private readonly AppDbContext _db;

    public CampaniaRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Campania>> ListAsync(CancellationToken cancellationToken = default) =>
        await _db.Campanias.AsNoTracking().OrderByDescending(c => c.AnioCreacion).ToListAsync(cancellationToken);

    public async Task<Campania?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _db.Campanias.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<Campania> AddAsync(Campania campania, CancellationToken cancellationToken = default)
    {
        _db.Campanias.Add(campania);
        await _db.SaveChangesAsync(cancellationToken);
        return campania;
    }

    public async Task UpdateAsync(Campania campania, CancellationToken cancellationToken = default)
    {
        _db.Campanias.Update(campania);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Campanias.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (entity is null)
            return false;

        _db.Campanias.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
