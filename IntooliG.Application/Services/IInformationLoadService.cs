using IntooliG.Application.Features.InformationLoad;

namespace IntooliG.Application.Services;

public interface IInformationLoadService
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

    Task<CargaGlobalUploadResultDto> GuardarArchivoTemporalAsync(
        string fileName,
        Stream fileStream,
        long sizeInBytes,
        CancellationToken cancellationToken = default);

    Task<CargaGlobalProcessResultDto> EjecutarProcesoAsync(
        int clienteId,
        int usuarioId,
        CargaGlobalProcessRequestDto request,
        CancellationToken cancellationToken = default);

    Task<bool> FinalizarCatalogacionPaisAsync(
        int paisId,
        int usuarioId,
        CancellationToken cancellationToken = default);
}
