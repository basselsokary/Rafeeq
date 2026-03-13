using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Application.Common.Interfaces.QueryServices;
using Domain.Common.Interfaces;
using Domain.Repositories;
using Infrastructure.Identity;
using Infrastructure.Persistence.QueryServices;
using Infrastructure.Persistence.Repositories;
using System.Text;
using Hangfire;
using Hangfire.SqlServer;
using Serilog;
using Infrastructure.Persistence.IdentityContext;
using Infrastructure.Persistence.ApplicationContext;
using RafeeqApp.Infrastructure.Persistence;
using Infrastructure.Persistence;
using Application.Common.Interfaces.Authentication;

namespace RafeeqApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add DbContexts
        // services.AddApplicationDbContext(configuration);
        // services.AddIdentityDbContext(configuration);

        // Add Interceptors
        // services.AddScoped<DomainEventDispatcherInterceptor>();
        // services.AddScoped<AuditableEntityInterceptor>();

        // Add Repositories
        services.AddRepositories();

        // Add Query Services
        services.AddQueryServices();

        // Add Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add Identity & Authentication
        // services.AddIdentityServices(configuration);

        // // Add Caching
        // services.AddCachingServices(configuration);

        // // Add External Services
        // services.AddExternalServices(configuration);

        // // Add Background Jobs
        // services.AddBackgroundJobs(configuration);

        // // Add Logging
        // services.AddLogging(configuration);

        return services;
    }

    private static IServiceCollection AddApplicationDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b =>
                {
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    b.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });

            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
        });

        return services;
    }

    private static IServiceCollection AddIdentityDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("IdentityConnection"),
                b =>
                {
                    b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName);
                    b.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAttractionRepository, AttractionRepository>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<ISponsorRepository, SponsorRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IContentReportRepository, ContentReportRepository>();

        return services;
    }

    private static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddScoped<IAttractionQueryService, AttractionQueryService>();
        services.AddScoped<ISiteQueryService, SiteQueryService>();
        services.AddScoped<IUserQueryService, UserQueryService>();
        services.AddScoped<IReviewQueryService, ReviewQueryService>();
        services.AddScoped<ISponsorQueryService, SponsorQueryService>();
        services.AddScoped<ICityQueryService, CityQueryService>();
        services.AddScoped<IContentReportQueryService, ContentReportQueryService>();

        return services;
    }

    // private static IServiceCollection AddIdentityServices(
    //     this IServiceCollection services,
    //     IConfiguration configuration)
    // {
    //     // Add Identity
    //     services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    //     {
    //         // Password settings
    //         options.Password.RequireDigit = true;
    //         options.Password.RequireLowercase = true;
    //         options.Password.RequireUppercase = true;
    //         options.Password.RequireNonAlphanumeric = true;
    //         options.Password.RequiredLength = 8;

    //         // Lockout settings
    //         options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    //         options.Lockout.MaxFailedAccessAttempts = 5;
    //         options.Lockout.AllowedForNewUsers = true;

    //         // User settings
    //         options.User.RequireUniqueEmail = true;
    //         options.SignIn.RequireConfirmedEmail = false;
    //     })
    //     .AddEntityFrameworkStores<IdentityDbContext>()
    //     .AddDefaultTokenProviders();

    //     // Add JWT Authentication
    //     var jwtSecret = configuration["Jwt:Secret"]
    //         ?? throw new InvalidOperationException("JWT Secret not configured");

    //     services.AddAuthentication(options =>
    //     {
    //         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //         options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    //     })
    //     .AddJwtBearer(options =>
    //     {
    //         options.SaveToken = true;
    //         options.RequireHttpsMetadata = true;
    //         options.TokenValidationParameters = new TokenValidationParameters
    //         {
    //             ValidateIssuer = true,
    //             ValidateAudience = true,
    //             ValidateLifetime = true,
    //             ValidateIssuerSigningKey = true,
    //             ValidIssuer = configuration["Jwt:Issuer"],
    //             ValidAudience = configuration["Jwt:Audience"],
    //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
    //             ClockSkew = TimeSpan.Zero
    //         };
    //     });

    //     services.AddAuthorization();

    //     // Add Identity Services
    //     services.AddScoped<IIdentityService, IdentityService>();
    //     services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

    //     return services;
    // }

    // private static IServiceCollection AddCachingServices(
    //     this IServiceCollection services,
    //     IConfiguration configuration)
    // {
    //     var redisConnection = configuration.GetConnectionString("Redis");

    //     if (!string.IsNullOrEmpty(redisConnection))
    //     {
    //         services.AddStackExchangeRedisCache(options =>
    //         {
    //             options.Configuration = redisConnection;
    //             options.InstanceName = "RafeeqApp_";
    //         });
    //     }
    //     else
    //     {
    //         // Fallback to in-memory cache for development
    //         services.AddDistributedMemoryCache();
    //     }

    //     services.AddScoped<ICacheService, CacheService>();

    //     return services;
    // }

    // private static IServiceCollection AddExternalServices(
    //     this IServiceCollection services,
    //     IConfiguration configuration)
    // {
    //     // Google Maps Service
    //     services.AddHttpClient<IGoogleMapsService, GoogleMapsService>(client =>
    //     {
    //         client.Timeout = TimeSpan.FromSeconds(30);
    //     });

    //     // Add other external services here as needed
    //     // services.AddHttpClient<IOpenAIService, OpenAIService>();

    //     return services;
    // }

    // private static IServiceCollection AddBackgroundJobs(
    //     this IServiceCollection services,
    //     IConfiguration configuration)
    // {
    //     var hangfireConnection = configuration.GetConnectionString("Hangfire")
    //         ?? configuration.GetConnectionString("DefaultConnection");

    //     if (!string.IsNullOrEmpty(hangfireConnection))
    //     {
    //         services.AddHangfire(config =>
    //         {
    //             config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    //                 .UseSimpleAssemblyNameTypeSerializer()
    //                 .UseRecommendedSerializerSettings()
    //                 .UseSqlServerStorage(hangfireConnection, new SqlServerStorageOptions
    //                 {
    //                     CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
    //                     SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
    //                     QueuePollInterval = TimeSpan.Zero,
    //                     UseRecommendedIsolationLevel = true,
    //                     DisableGlobalLocks = true
    //                 });
    //         });

    //         services.AddHangfireServer(options =>
    //         {
    //             options.WorkerCount = 2;
    //         });
    //     }

    //     return services;
    // }

    // private static IServiceCollection AddLogging(
    //     this IServiceCollection services,
    //     IConfiguration configuration)
    // {
    //     Log.Logger = new LoggerConfiguration()
    //         .ReadFrom.Configuration(configuration)
    //         .Enrich.FromLogContext()
    //         .WriteTo.Console()
    //         .WriteTo.File(
    //             path: "logs/rafeeq-.txt",
    //             rollingInterval: RollingInterval.Day,
    //             retainedFileCountLimit: 30)
    //         .CreateLogger();

    //     services.AddLogging(loggingBuilder =>
    //     {
    //         loggingBuilder.AddSerilog(dispose: true);
    //     });

    //     return services;
    // }
}
