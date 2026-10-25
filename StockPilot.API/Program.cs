using Microsoft.EntityFrameworkCore;
using StockPilot.Infrastructure.Data;
using StockPilot.API.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using StockPilot.API.Exceptions;
using StockPilot.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration
                           .GetConnectionString("OracleConnection")
                       ?? throw new InvalidOperationException(
                           "A connection string 'OracleConnection' não foi encontrada."
                       );



builder.Services.AddDbContext<StockPilotDbContext>(options => options.UseOracle(connectionString, oracleOptions =>
    {
        oracleOptions.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19);

        oracleOptions.MigrationsHistoryTable("TB_STOCKPILOT_MIGRATIONS");
    })
);


builder.Services.AddStockPilotServices();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
JwtConfiguration.Configure(
    builder.Services,
    builder.Configuration
);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(new ExceptionHandlerOptions
{
    AllowStatusCode404Response = true
});

app.MapControllers();

app.Run();

public partial class Program
{
}