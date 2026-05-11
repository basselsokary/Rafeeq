using Infrastructure.Persistence.Seeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class StaticDataServiceProviderExtensions
{
    public static async Task EnsureStaticDataAsync(
        this IServiceProvider services,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        bool useStaticData = configuration.GetValue<bool>("StaticData:UseStaticData");
        
        if (!useStaticData)
        {
            return;
        }

        using IServiceScope scope = services.CreateScope();
        await StaticDataSeeder.SeedAsync(scope.ServiceProvider, cancellationToken);
    }
}
