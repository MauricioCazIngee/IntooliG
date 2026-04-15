using IntooliG.Application.Common.Models;
using IntooliG.Application.Features.Auth;
using IntooliG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntooliG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService auth, ILogger<AuthController> logger)
    {
        _auth = auth;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _auth.LoginAsync(request, cancellationToken);
        if (result is null)
        {
            _logger.LogInformation("Login rechazado para {Email}", request.Email);
            return Unauthorized(ApiResponse<LoginResponse>.Fail("Credenciales inválidas.", StatusCodes.Status401Unauthorized));
        }

        return Ok(ApiResponse<LoginResponse>.Ok(result, "Autenticación correcta."));
    }
}
