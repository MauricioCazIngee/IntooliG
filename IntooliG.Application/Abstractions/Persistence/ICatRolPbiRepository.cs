namespace IntooliG.Application.Abstractions.Persistence;

public interface ICatRolPbiRepository
{
    /// <summary>Resuelve el nombre del rol (ej. ADMIN) para RLS, o null si no existe el id.</summary>
    Task<string?> GetNombreByIdAsync(int rolPbiId, CancellationToken cancellationToken = default);
}
