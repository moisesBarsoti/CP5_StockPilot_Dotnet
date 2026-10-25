using Microsoft.OpenApi.Models;

namespace StockPilot.API.Configurations;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddStockPilotSwagger(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "StockPilot API",
                Version = "v1",
                Description =
                    "API REST de gerenciamento de produtos e fornecedores " +
                    "com Oracle Database, JWT e integração ViaCEP."
            });

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description =
                        "Informe somente o token JWT gerado no login."
                }
            );

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            );
        });

        return services;
    }
}