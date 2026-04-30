using IntooliG.Application.Features.Inversiones;

namespace IntooliG.Application.Services;

public interface IInversionesService
{
    Task<InversionesFiltrosDto> GetFiltrosAsync(
        int clienteId,
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CiudadFiltroDto>> GetCiudadesPorRegionAsync(
        int clienteId,
        int usuarioId,
        int regionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InversionesResultDto>> ConsultarAsync(
        int clienteId,
        int usuarioId,
        InversionesRequestDto request,
        CancellationToken cancellationToken = default);

    Task<InversionesTablaDinamicaDto> GetBarrasAsync(
        int clienteId,
        int usuarioId,
        InversionesRequestDto request,
        CancellationToken cancellationToken = default);

    Task<InversionesTablaDinamicaDto> GetPieCategoriaAsync(
        int clienteId,
        int usuarioId,
        InversionesRequestDto request,
        CancellationToken cancellationToken = default);

    Task<InversionesTablaDinamicaDto> GetPieMarcaAsync(
        int clienteId,
        int usuarioId,
        InversionesRequestDto request,
        CancellationToken cancellationToken = default);

    Task<InversionesTablaDinamicaDto> GetPieMedioAsync(
        int clienteId,
        int usuarioId,
        InversionesRequestDto request,
        CancellationToken cancellationToken = default);
}
