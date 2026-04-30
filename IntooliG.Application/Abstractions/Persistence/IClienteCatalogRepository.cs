using IntooliG.Application.Features.RubrosConceptos;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IClienteCatalogRepository
{
    Task<IReadOnlyList<ClienteLookupDto>> ListActivosAsync(CancellationToken cancellationToken = default);
}
