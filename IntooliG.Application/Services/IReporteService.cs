using IntooliG.Application.Features.Reportes;

namespace IntooliG.Application.Services;

public interface IReporteService
{
    Task<ReportesKpiDto> GetKpisAsync(CancellationToken cancellationToken = default);
}
