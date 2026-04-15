using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Usuarios;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarios;

    public UsuariosController(IUsuarioService usuarios)
    {
        _usuarios = usuarios;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UsuarioDto>>>> List(CancellationToken cancellationToken)
    {
        var data = await _usuarios.ListAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<UsuarioDto>>.Ok(data));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var data = await _usuarios.GetByIdAsync(id, cancellationToken);
        if (data is null)
            return NotFound(ApiResponse<UsuarioDto>.Fail("Usuario no encontrado.", StatusCodes.Status404NotFound));

        return Ok(ApiResponse<UsuarioDto>.Ok(data));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> Create([FromBody] CreateUsuarioRequest request, CancellationToken cancellationToken)
    {
        var (usuario, error) = await _usuarios.CreateAsync(request, cancellationToken);
        if (usuario is null)
            return BadRequest(ApiResponse<UsuarioDto>.Fail(error ?? "No fue posible crear el usuario.", StatusCodes.Status400BadRequest));

        return CreatedAtAction(nameof(GetById), new { id = usuario.Id },
            ApiResponse<UsuarioDto>.Ok(usuario, "Usuario creado.", StatusCodes.Status201Created));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> Update(int id, [FromBody] UpdateUsuarioRequest request, CancellationToken cancellationToken)
    {
        var (usuario, error) = await _usuarios.UpdateAsync(id, request, cancellationToken);
        if (usuario is null)
        {
            var status = error == "Usuario no encontrado." ? StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest;
            return StatusCode(status, ApiResponse<UsuarioDto>.Fail(error ?? "No fue posible actualizar el usuario.", status));
        }

        return Ok(ApiResponse<UsuarioDto>.Ok(usuario, "Usuario actualizado."));
    }

    [HttpPatch("{id:int}/rol")]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> ChangeRol(int id, [FromBody] ChangeRolRequest request, CancellationToken cancellationToken)
    {
        var (usuario, error) = await _usuarios.ChangeRolAsync(id, request, cancellationToken);
        if (usuario is null)
        {
            var status = error == "Usuario no encontrado." ? StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest;
            return StatusCode(status, ApiResponse<UsuarioDto>.Fail(error ?? "No fue posible cambiar el rol.", status));
        }

        return Ok(ApiResponse<UsuarioDto>.Ok(usuario, "Rol actualizado."));
    }

    [HttpPost("{id:int}/reset-password")]
    public async Task<ActionResult<ApiResponse<bool>>> ResetPassword(int id, [FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var (ok, error) = await _usuarios.ResetPasswordAsync(id, request, cancellationToken);
        if (!ok)
        {
            var status = error == "Usuario no encontrado." ? StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest;
            return StatusCode(status, ApiResponse<bool>.Fail(error ?? "No fue posible restablecer la contraseña.", status));
        }

        return Ok(ApiResponse<bool>.Ok(true, "Contraseña restablecida."));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _usuarios.DeleteAsync(id, cancellationToken);
        if (!ok)
            return NotFound(ApiResponse<bool>.Fail("Usuario no encontrado.", StatusCodes.Status404NotFound));

        return Ok(ApiResponse<bool>.Ok(true, "Usuario eliminado."));
    }
}
