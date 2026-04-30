using IntooliG.Api.Extensions;
using IntooliG.Application.Abstractions.Persistence;
using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Catalogo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly IProductoRepository _productos;

    public ProductosController(IProductoRepository productos)
    {
        _productos = productos;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProductoDto>>>> ListByMarca(
        [FromQuery] int marcaId,
        CancellationToken cancellationToken)
    {
        var clienteId = User.GetClienteId();
        var list = await _productos.ListByMarcaAsync(marcaId, clienteId, cancellationToken);
        var dto = list.Select(p => new ProductoDto(p.Id, p.Nombre, p.Activo)).ToList();
        return Ok(ApiResponse<IReadOnlyList<ProductoDto>>.Ok(dto));
    }
}
