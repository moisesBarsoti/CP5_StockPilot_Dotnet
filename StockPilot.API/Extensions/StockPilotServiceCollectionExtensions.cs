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

                client.Timeout = TimeSpan.FromSeconds(5);
            }
        );

        
        return services;
    }
}