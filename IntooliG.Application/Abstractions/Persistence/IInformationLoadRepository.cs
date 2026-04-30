using IntooliG.Application.Features.InformationLoad;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IInformationLoadRepository
{
    Task<CargaGlobalFiltrosDto> GetCargaGlobalFiltrosAsync(
        int clienteId,
        int? paisId,
        CancellationToken cancellationToken = default);

    Task<CargaGlobalLogDto?> GetUltimoLogProcesoAsync(
        int clienteId,
        int paisId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CargaGlobalEstadoCatalogacionDto>> GetEstadoCatalogacionPaisAsync(
        int paisId,
        CancellationToken cancellationToken = default);

    Task<CargaGlobalProcessResultDto> ExecuteProcessAsync(
        int clienteId,
        int usuarioId,
        int paisId,
        string proceso,
        CancellationToken cancellationToken = default);

    Task<bool> FinalizarCatalogacionPaisAsync(
        int paisId,
        int usuarioId,
        CancellationToken cancellationToken = default);
}
