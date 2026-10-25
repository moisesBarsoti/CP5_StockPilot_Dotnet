using Microsoft.EntityFrameworkCore;
using StockPilot.Infrastructure.Data;
using StockPilot.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration
                           .GetConnectionString("OracleConnection")
                       ?? throw new InvalidOperationException(
                           "A connection string 'OracleConnection' não foi encontrada."
                       );


builder.Services.AddDbContext<StockPilotDbContext>(options => options.UseOracle(connectionString, oracleOptions =>
        oracleOptions.MigrationsHistoryTable("TB_STOCKPILOT_MIGRATIONS"))
);

builder.Services.AddStockPilotServices();

var app = builder.Build();

app.MapControllers();

app.Run();

public partial class Program
{
}