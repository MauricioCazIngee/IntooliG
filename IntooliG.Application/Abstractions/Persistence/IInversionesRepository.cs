using IntooliG.Application.Features.Inversiones;

namespace IntooliG.Application.Abstractions.Persistence;

public interface IInversionesRepository
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

    Task<IReadOnlyList<InversionesResultDto>> GetDatosAsync(
        int clienteId,
        int usuarioId,
        InversionesQueryContext query,
        CancellationToken cancellationToken = default);

    Task<InversionesTablaDinamicaDto> GetDatosBarrasAsync(
        int clienteId,
        int usuarioId,
        InversionesQueryContext query,
        CancellationToken cancellationToken = default);

    Task<InversionesTablaDinamicaDto> GetDatosPieCategoriaAsync(
        int clienteId,
        int usuarioId,
        InversionesQueryContext query,
        CancellationToken cancellationToken = default);

    Task<InversionesTablaDinamicaDto> GetDatosPieMarcaAsync(
        int clienteId,
        int usuarioId,
        InversionesQueryContext query,
        CancellationToken cancellationToken = default);

    Task<InversionesTablaDinamicaDto> GetDatosPieMedioAsync(
        int clienteId,
        int usuarioId,
        InversionesQueryContext query,
        CancellationToken cancellationToken = default);
}
