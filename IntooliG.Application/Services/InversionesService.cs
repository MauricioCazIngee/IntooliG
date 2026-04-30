using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Features.Inversiones;

namespace IntooliG.Application.Services;

public class InversionesService : IInversionesService
{
    private readonly IInversionesRepository _repo;

    public InversionesService(IInversionesRepository repo)
    {
        _repo = repo;
    }

    public Task<InversionesFiltrosDto> GetFiltrosAsync(
        int clienteId,
        int usuarioId,
        CancellationToken cancellationToken = default) =>
        _repo.GetFiltrosAsync(clienteId, usuarioId, cancellationToken);

    public Task<IReadOnlyList<CiudadFiltroDto>> GetCiudadesPorRegionAsync(
        int clienteId,
        int usuarioId,
        int regionId,
        CancellationToken cancellationToken = default) =>
        _repo.GetCiudadesPorRegionAsync(clienteId, usuarioId, regionId, cancellationToken);

    public Task<IReadOnlyList<InversionesResultDto>> ConsultarAsync(
        int clienteId,
        int usuarioId,
        InversionesRequestDto request,
        CancellationToken cancellationToken = default) =>
        _repo.GetDatosAsync(clienteId, usuarioId, ToContext(request), cancellationToken);

    public Task<InversionesTablaDinamicaDto> GetBarrasAsync(
        int clienteId,
        int usuarioId,
        InversionesRequestDto request,
        CancellationToken cancellationToken = default) =>
        _repo.GetDatosBarrasAsync(clienteId, usuarioId, ToContext(request), cancellationToken);

    public Task<InversionesTablaDinamicaDto> GetPieCategoriaAsync(
        int clienteId,
        int usuarioId,
        InversionesRequestDto request,
        CancellationToken cancellationToken = default) =>
        _repo.GetDatosPieCategoriaAsync(clienteId, usuarioId, ToContext(request), cancellationToken);

    public Task<InversionesTablaDinamicaDto> GetPieMarcaAsync(
        int clienteId,
        int usuarioId,
        InversionesRequestDto request,
        CancellationToken cancellationToken = default) =>
        _repo.GetDatosPieMarcaAsync(clienteId, usuarioId, ToContext(request), cancellationToken);

    public Task<InversionesTablaDinamicaDto> GetPieMedioAsync(
        int clienteId,
        int usuarioId,
        InversionesRequestDto request,
        CancellationToken cancellationToken = default) =>
        _repo.GetDatosPieMedioAsync(clienteId, usuarioId, ToContext(request), cancellationToken);

    private static InversionesQueryContext ToContext(InversionesRequestDto r)
    {
        var (start, end) = InversionesFechasHelper.CalcularRango(r.Periodo, r.FechaFinal);
        return new InversionesQueryContext(
            r.Periodo,
            start,
            end,
            r.CategoriaId is > 0 ? r.CategoriaId : null,
            r.TipoTarifa,
            r.Vista,
            r.RegionId is > 0 ? r.RegionId : null,
            r.CiudadId is > 0 ? r.CiudadId : null,
            r.ExchangeRate,
            r.MarcaId is > 0 ? r.MarcaId : null,
            r.PaisId is > 0 ? r.PaisId : null,
            r.SectorId is > 0 ? r.SectorId : null,
            r.BuId is > 0 ? r.BuId : null);
    }
}
