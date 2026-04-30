using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.RubrosConceptos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/clientes")]
[Authorize]
public class ClientesCatalogoController : ControllerBase
{
    private readonly IClienteCatalogRepository _repo;

    public ClientesCatalogoController(IClienteCatalogRepository repo)
    {
        _repo = repo;
    }

    /// <summary>Listado de clientes activos (selectores de configuración).</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ClienteLookupDto>>>> List(CancellationToken cancellationToken)
    {
        var items = await _repo.ListActivosAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ClienteLookupDto>>.Ok(items));
    }
}
