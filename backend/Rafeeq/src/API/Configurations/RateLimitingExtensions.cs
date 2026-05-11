using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Configurations;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimiter(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration
            .GetSection(RateLimiterSettings.SectionName)
            .Get<RateLimiterSettings>() ?? new RateLimiterSettings();

        services.AddRateLimiter(limiter =>
        {
            // Auth endpoints: tight fixed window
            limiter.AddFixedWindowLimiter(RateLimiterPolicies.Auth, o =>
            {
                o.PermitLimit        = options.Auth.PermitLimit;        // e.g. 5
                o.Window             = options.Auth.Window;             // e.g. 1 minute
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                o.QueueLimit         = 0; // reject immediately, no queuing
            });

            // Global limiter
            limiter.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(ip, _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit        = options.Global.PermitLimit,
                        Window             = options.Global.Window,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit         = 0
                    });
            });

            // Partition by IP so one client doesn't block another
            limiter.AddPolicy(RateLimiterPolicies.AuthPerIp, context =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(ip, _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit        = options.Auth.PermitLimit,
                        Window             = options.Auth.Window,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit         = 0
                    });
            });

            // Uniform 429 response
            limiter.OnRejected = async (ctx, ct) =>
            {
                ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    ctx.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString();
                }

                ctx.HttpContext.Response.ContentType = "application/problem+json";

                await ctx.HttpContext.Response.WriteAsync("""
                    {
                        "type":   "https://tools.ietf.org/html/rfc6585#section-4",
                        "title":  "Too Many Requests",
                        "status": 429,
                        "detail": "You have exceeded the allowed request rate. Please slow down."
                    }
                    """, ct);
            };
        });

        return services;
    }
}

public sealed class RateLimiterSettings
{
    public const string SectionName = "RateLimiter";

    public WindowPolicy Auth   { get; init; } = new()
    {
        PermitLimit = 5,
        Window = TimeSpan.FromMinutes(1)
    };
    public WindowPolicy Global { get; init; } = new()
    {
        PermitLimit = 60,
        Window = TimeSpan.FromMinutes(1)
    };

    public sealed class WindowPolicy
    {
        public int      PermitLimit { get; init; }
        public TimeSpan Window      { get; init; }
    }
}

public static class RateLimiterPolicies
{
    public const string Auth      = "fixed-auth";
    public const string AuthPerIp = "fixed-auth-per-ip";
    public const string Global    = "fixed-global";
}
