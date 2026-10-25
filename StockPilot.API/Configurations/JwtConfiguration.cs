
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StockPilot.API.Services;

namespace StockPilot.API.Configurations;

public static class JwtConfiguration
{
    public const string Issuer = "stockpilot-api";
    public const string Audience = "stockpilot-client";

    public static void Configure(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var secret = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "A chave JWT não foi configurada."
            );

        byte[] keyBytes;

        try
        {
            keyBytes = Convert.FromBase64String(secret);
        }
        catch (FormatException)
        {
            throw new InvalidOperationException(
                "A chave JWT deve estar codificada em Base64."
            );
        }

        if (keyBytes.Length < 32)
            throw new InvalidOperationException(
                "A chave JWT deve possuir pelo menos 256 bits."
            );

        var signingKey = new SymmetricSecurityKey(keyBytes);

        services.AddSingleton(
            new JwtTokenService(signingKey)
        );

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Issuer,

                        ValidateAudience = true,
                        ValidAudience = Audience,

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,

                        RoleClaimType = "role",
                        NameClaimType = JwtRegisteredClaimNames.Sub
                    };
            });

        services.AddAuthorization();
    }
}
