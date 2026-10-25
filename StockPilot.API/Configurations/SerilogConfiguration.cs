using Serilog;
using Serilog.Events;

namespace StockPilot.API.Configurations;

public static class SerilogConfiguration
{
    public static void Configure(IHostBuilder host)
    {
        host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .MinimumLevel.Information()
                .MinimumLevel.Override(
                    "Microsoft",
                    LogEventLevel.Warning
                )
                .MinimumLevel.Override(
                    "Microsoft.Hosting.Lifetime",
                    LogEventLevel.Information
                )
                .Enrich.FromLogContext()
                .Enrich.WithProperty(
                    "Application",
                    "StockPilot.API"
                )
                .Enrich.WithProperty(
                    "Environment",
                    context.HostingEnvironment.EnvironmentName
                )
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] " +
                    "{Message:lj} {Properties:j}" +
                    "{NewLine}{Exception}"
                );
        });
    }
}