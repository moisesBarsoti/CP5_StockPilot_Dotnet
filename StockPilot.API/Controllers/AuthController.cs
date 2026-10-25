
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockPilot.API.DTOs.Auth;
using StockPilot.API.Services;

namespace StockPilot.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public AuthController(
        JwtTokenService jwtTokenService,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
        _environment = environment;
    }

    /// <summary>
    /// Autentica um usuário de demonstração.
    /// </summary>
    /// <remarks>
    /// Valida as credenciais dos usuários ADMIN ou VIEWER e,
    /// quando válidas, gera um token JWT para acesso à API.
    ///
    /// O login demonstrativo está disponível em Development
    /// ou quando habilitado explicitamente por configuração.
    /// </remarks>
    /// <param name="request">
    /// Credenciais do usuário utilizadas na autenticação.
    /// </param>
    /// <response code="200">
    /// Autenticação realizada e token JWT gerado.
    /// </response>
    /// <response code="400">
    /// Dados da requisição inválidos.
    /// </response>
    /// <response code="401">
    /// Usuário ou senha inválidos.
    /// </response>
    /// <response code="404">
    /// Login demonstrativo desabilitado no ambiente.
    /// </response>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!_environment.IsDevelopment() &&
            !_configuration.GetValue<bool>("DemoAuth:Enabled"))
        {
            return NotFound();
        }

        var username = request.Username.Trim().ToLowerInvariant();

        var role = username switch
        {
            "admin" => "ADMIN",
            "viewer" => "VIEWER",
            _ => null
        };

        var expectedPassword = role switch
        {
            "ADMIN" => _configuration["DemoAuth:AdminPassword"],
            "VIEWER" => _configuration["DemoAuth:ViewerPassword"],
            _ => null
        };

        if (role is null ||
            string.IsNullOrEmpty(expectedPassword) ||
            !PasswordsMatch(request.Password, expectedPassword))
        {
            return Unauthorized(new
            {
                code = "INVALID_CREDENTIALS",
                message = "Usuário ou senha inválidos."
            });
        }

        var (token, expiresAt) = _jwtTokenService
            .GenerateToken(username, role);

        return Ok(new
        {
            accessToken = token,
            tokenType = "Bearer",
            expiresAtUtc = expiresAt,
            role
        });
    }

    private static bool PasswordsMatch(
        string provided,
        string expected)
    {
        var providedHash = SHA256.HashData(
            Encoding.UTF8.GetBytes(provided)
        );

        var expectedHash = SHA256.HashData(
            Encoding.UTF8.GetBytes(expected)
        );

        return CryptographicOperations.FixedTimeEquals(
            providedHash,
            expectedHash
        );
    }
}
