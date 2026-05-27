using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundJobs;

internal sealed class RefreshTokenCleanupJob(
    IServiceScopeFactory scopeFactory,
    ILogger<RefreshTokenCleanupJob> logger,
    IOptions<RefreshTokenCleanupSettings> options) : BackgroundService
{
    private readonly RefreshTokenCleanupSettings _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("RefreshTokenCleanupJob started. Interval: {Interval}.",
            _options.Interval);

        // Delay the first run slightly so the app finishes startup
        await Task.Delay(TimeSpan.FromHours(2), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanupAsync(stoppingToken);
            await Task.Delay(_options.Interval, stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken ct)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var utcNow = DateTime.UtcNow;

            int deleted = await db.RefreshTokens
                .Where(rt => rt.ExpiresAt < utcNow || rt.IsRevoked)
                .ExecuteDeleteAsync(ct); // Bulk delete — no round-trip load

            if (deleted > 0)
                logger.LogInformation(
                    "RefreshTokenCleanupJob: deleted {Count} inactive token(s) at {UtcNow}.",
                    deleted, utcNow);
        }
        catch (OperationCanceledException)
        {
            // App is shutting down — swallow gracefully
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "RefreshTokenCleanupJob: unexpected error during cleanup.");
            // Don't rethrow — the while loop keeps the job alive
        }
    }
}

public sealed class RefreshTokenCleanupSettings
{
    public const string SectionName = "RefreshTokenCleanup";

    public TimeSpan Interval { get; init; } = TimeSpan.FromHours(6); // sensible default
}