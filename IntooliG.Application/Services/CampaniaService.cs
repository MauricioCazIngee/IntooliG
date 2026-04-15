using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Features.Campanias;
using IntooliG.Domain.Entities;

namespace IntooliG.Application.Services;

public class CampaniaService : ICampaniaService
{
    private readonly ICampaniaRepository _repository;

    public CampaniaService(ICampaniaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CampaniaDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.ListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<CampaniaDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public async Task<CampaniaDto> CreateAsync(CreateCampaniaRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Campania
        {
            Nombre = request.Nombre.Trim(),
            Activa = request.Activa,
            AnioCreacion = request.FechaInicio?.Year ?? DateTime.UtcNow.Year,
            ClienteId = 1,
            PaisId = 1
        };

        var created = await _repository.AddAsync(entity, cancellationToken);
        return Map(created);
    }

    public async Task<CampaniaDto?> UpdateAsync(int id, UpdateCampaniaRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return null;

        entity.Nombre = request.Nombre.Trim();
        entity.Activa = request.Activa;
        entity.AnioCreacion = request.FechaInicio?.Year ?? entity.AnioCreacion;

        await _repository.UpdateAsync(entity, cancellationToken);
        return Map(entity);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        _repository.DeleteAsync(id, cancellationToken);

    private static CampaniaDto Map(Campania c) =>
        new(
            c.Id,
            c.Codigo,
            c.Nombre,
            c.Descripcion,
            c.FechaInicio,
            c.FechaFin,
            c.Activa,
            new DateTime(Math.Max(c.AnioCreacion, 1900), 1, 1));
}
