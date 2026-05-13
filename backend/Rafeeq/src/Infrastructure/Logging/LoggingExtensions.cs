using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Infrastructure.Logging;

public static class LoggingExtensions
{
    public static void ConfigureSerilog(
        this IHostBuilder hostBuilder,
        IConfiguration configuration)
    {
        hostBuilder.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(configuration)
                // .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        });
    }
}