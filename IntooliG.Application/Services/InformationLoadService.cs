using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Features.InformationLoad;

namespace IntooliG.Application.Services;

public class InformationLoadService : IInformationLoadService
{
    private readonly IInformationLoadRepository _repo;

    public InformationLoadService(IInformationLoadRepository repo)
    {
        _repo = repo;
    }

    public Task<CargaGlobalFiltrosDto> GetCargaGlobalFiltrosAsync(
        int clienteId,
        int? paisId,
        CancellationToken cancellationToken = default) =>
        _repo.GetCargaGlobalFiltrosAsync(clienteId, paisId, cancellationToken);

    public Task<CargaGlobalLogDto?> GetUltimoLogProcesoAsync(
        int clienteId,
        int paisId,
        CancellationToken cancellationToken = default) =>
        _repo.GetUltimoLogProcesoAsync(clienteId, paisId, cancellationToken);

    public Task<IReadOnlyList<CargaGlobalEstadoCatalogacionDto>> GetEstadoCatalogacionPaisAsync(
        int paisId,
        CancellationToken cancellationToken = default) =>
        _repo.GetEstadoCatalogacionPaisAsync(paisId, cancellationToken);

    public async Task<CargaGlobalUploadResultDto> GuardarArchivoTemporalAsync(
        string fileName,
        Stream fileStream,
        long sizeInBytes,
        CancellationToken cancellationToken = default)
    {
        var safeName = Path.GetFileName(fileName);
        if (string.IsNullOrWhiteSpace(safeName))
            throw new InvalidOperationException("Nombre de archivo no válido.");

        var ext = Path.GetExtension(safeName).ToLowerInvariant();
        if (ext is not ".xlsx" and not ".xls" and not ".csv")
            throw new InvalidOperationException("Solo se permiten archivos Excel o CSV.");

        var folder = Path.Combine(Path.GetTempPath(), "IntooliG", "information-load");
        Directory.CreateDirectory(folder);

        var stamped = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}_{safeName}";
        var fullPath = Path.Combine(folder, stamped);

        await using var fs = File.Create(fullPath);
        await fileStream.CopyToAsync(fs, cancellationToken);

        return new CargaGlobalUploadResultDto
        {
            NombreArchivo = safeName,
            RutaTemporal = fullPath,
            TamanoBytes = sizeInBytes,
            FechaCargaUtc = DateTime.UtcNow
        };
    }

    public Task<CargaGlobalProcessResultDto> EjecutarProcesoAsync(
        int clienteId,
        int usuarioId,
        CargaGlobalProcessRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (request.PaisId <= 0)
            throw new InvalidOperationException("Debe seleccionar un país válido.");
        if (string.IsNullOrWhiteSpace(request.Proceso))
            throw new InvalidOperationException("El nombre del proceso es requerido.");

        return _repo.ExecuteProcessAsync(clienteId, usuarioId, request.PaisId, request.Proceso, cancellationToken);
    }

    public Task<bool> FinalizarCatalogacionPaisAsync(
        int paisId,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (paisId <= 0)
            throw new InvalidOperationException("Debe seleccionar un país válido.");
        return _repo.FinalizarCatalogacionPaisAsync(paisId, usuarioId, cancellationToken);
    }
}
