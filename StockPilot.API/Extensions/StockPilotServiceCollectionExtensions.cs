using StockPilot.Application.Interfaces.Repositories;
using StockPilot.Infrastructure.Data;
using StockPilot.Infrastructure.Repositories;
using StockPilot.Application.Interfaces.External;
using StockPilot.Infrastructure.External;
using StockPilot.Application.Interfaces.Services;
using StockPilot.Application.Services;


namespace StockPilot.API.Extensions;

public static class StockPilotServiceCollectionExtensions
{
    public static IServiceCollection AddStockPilotServices(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IProductService, ProductService>();
        
        
        services.AddHttpClient<IZipCodeService, ViaCepService>(
                client =>
                {
                    client.BaseAddress = new Uri("https://viacep.com.br/ws/");
                }
            )
            .AddStandardResilienceHandler(options =>
            {
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(8);

                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);

                options.Retry.MaxRetryAttempts = 2;

                options.Retry.Delay = TimeSpan.FromMilliseconds(250);

                options.CircuitBreaker.MinimumThroughput = 10;
            });

        
        return services;
    }
}