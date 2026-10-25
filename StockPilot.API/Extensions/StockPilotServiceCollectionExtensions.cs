using StockPilot.Application.Interfaces.Repositories;
using StockPilot.Infrastructure.Data;
using StockPilot.Infrastructure.Repositories;

namespace StockPilot.API.Extensions;

public static class StockPilotServiceCollectionExtensions
{
    public static IServiceCollection AddStockPilotServices(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}