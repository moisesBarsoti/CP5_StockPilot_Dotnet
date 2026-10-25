using Microsoft.EntityFrameworkCore;
using StockPilot.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration
                           .GetConnectionString("OracleConnection")
                       ?? throw new InvalidOperationException(
                           "A connection string 'OracleConnection' não foi encontrada."
                       );

builder.Services.AddDbContext<StockPilotDbContext>(options => options.UseOracle(connectionString));

var app = builder.Build();

app.MapControllers();

app.Run();

public partial class Program
{
}