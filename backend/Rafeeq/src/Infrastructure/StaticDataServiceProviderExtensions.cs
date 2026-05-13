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
        
        await using var scope = services.CreateAsyncScope();
        if (!useStaticData)
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            await seeder.SeedAsync(cancellationToken);
            return;
        }

        await StaticDataSeeder.SeedAsync(scope.ServiceProvider, cancellationToken);

    }

    public static async Task SeedAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync(cancellationToken);
    }
}
