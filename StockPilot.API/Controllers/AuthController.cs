
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

    [AllowAnonymous]
    [HttpPost("login")]
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
