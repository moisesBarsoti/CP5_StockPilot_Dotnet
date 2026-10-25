using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using StockPilot.API.Configurations;

namespace StockPilot.API.Services;

public class JwtTokenService
{
    private readonly SymmetricSecurityKey _signingKey;

    public JwtTokenService(SymmetricSecurityKey signingKey)
    {
        _signingKey = signingKey;
    }

    public (string Token, DateTime ExpiresAtUtc) GenerateToken(
        string username,
        string role)
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.AddHours(1);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, username),
            new("role", role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(
            _signingKey,
            SecurityAlgorithms.HmacSha256
        );

        var jwt = new JwtSecurityToken(
            issuer: JwtConfiguration.Issuer,
            audience: JwtConfiguration.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return (
            new JwtSecurityTokenHandler().WriteToken(jwt),
            expiresAt
        );
    }
}