using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Features.Reportes;

namespace IntooliG.Application.Services;

public class ReporteService : IReporteService
{
    private readonly ICampaniaRepository _campanias;
    private readonly IUsuarioRepository _usuarios;

    public ReporteService(ICampaniaRepository campanias, IUsuarioRepository usuarios)
    {
        _campanias = campanias;
        _usuarios = usuarios;
    }

    public async Task<ReportesKpiDto> GetKpisAsync(CancellationToken cancellationToken = default)
    {
        var campanias = await _campanias.ListAsync(cancellationToken);
        var usuarios = await _usuarios.ListAsync(cancellationToken);

        var campaniasActivas = campanias.Count(c => c.Activa);
        var usuariosAdmin = usuarios.Count(u => u.RolId == 1);

        return new ReportesKpiDto(
            TotalCampanias: campanias.Count,
            CampaniasActivas: campaniasActivas,
            CampaniasInactivas: campanias.Count - campaniasActivas,
            TotalUsuarios: usuarios.Count,
            UsuariosAdmin: usuariosAdmin,
            UsuariosNormales: usuarios.Count - usuariosAdmin);
    }
}
