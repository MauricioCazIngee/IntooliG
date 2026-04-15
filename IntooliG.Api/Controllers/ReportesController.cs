using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Reportes;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _reportes;

    public ReportesController(IReporteService reportes)
    {
        _reportes = reportes;
    }

    [HttpGet("kpis")]
    public async Task<ActionResult<ApiResponse<ReportesKpiDto>>> Kpis(CancellationToken cancellationToken)
    {
        var data = await _reportes.GetKpisAsync(cancellationToken);
        return Ok(ApiResponse<ReportesKpiDto>.Ok(data));
    }
}
