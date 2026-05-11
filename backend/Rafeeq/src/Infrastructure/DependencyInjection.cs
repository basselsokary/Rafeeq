using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Services;
using CloudinaryDotNet;
using Domain.Common.Interfaces;
using Domain.Repositories;
using Infrastructure.Authentication;
using Infrastructure.BackgroundJobs;
using Infrastructure.Caching;
using Infrastructure.Emails;
using Infrastructure.Events;
using Infrastructure.ExternalServices.CloudinaryService;
using Infrastructure.ExternalServices.GoogleService;
using Infrastructure.ExternalServices.ScannerService;
using Infrastructure.ExternalServices.TripPlannerService;
using Infrastructure.Identity;
using Infrastructure.Identity.Entities;
using Infrastructure.Localization;
using Infrastructure.Persistence;
using Infrastructure.Persistence.ApplicationContext;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.QueryServices;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Seeding;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplicationDbContext(configuration);

        // Add Unit of Work, Repositories, and Query Services
        services.AddRepositories();
        services.AddQueryServices();

        // Add Identity & Authentication
        services.AddIdentity(configuration)
            .AddJwtAuthentication(configuration);

        services.AddLocalization();

        services.AddMemoryCache();

        services.AddExternalServices(configuration);

        // Add Background Jobs
        // services.AddBackgroundJobs(configuration);

        // Add Logging
        // services.AddLogging(configuration);

        services.AddOtherServices(configuration);

        return services;
    }

    private static IServiceCollection AddOtherServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CurrentUserService>();
        services.AddScoped<IUserContext, CurrentUserService>();
        services.AddScoped<IModeratorService, ModeratorService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<ICsvFileParser, CsvParser>();
        services.AddScoped<CsvMapRegistry>();
        services.AddScoped<ICacheService, BaseCache>();
        services.AddScoped<IImageProcessingService, ImageProcessingService>();
        
        // Add email service
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddSingleton<IEmailService, EmailService>();

        services.AddSingleton<IPasswordGenerator, TemporaryPasswordGenerator>();
        services.AddScoped<IEmailGeneratorService, EmailGeneratorService>();
        services.AddScoped<IExternalIdentityService, ExternalIdentityService>();

        services.AddScoped<DomainEventsDispatcher>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAttractionRepository, AttractionRepository>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<ITouristRepository, TouristRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<ISponsorRepository, SponsorRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IContentReportRepository, ContentReportRepository>();
        services.AddScoped<IStoredFileRepository, StoredFileRepository>();

        return services;
    }

    private static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddScoped<IArtifactQueryService, ArtifactQueryService>();
        services.AddScoped<IAttractionQueryService, AttractionQueryService>();
        services.AddScoped<ISiteQueryService, SiteQueryService>();
        services.AddScoped<ITouristQueryService, TouristQueryService>();
        services.AddScoped<IReviewQueryService, ReviewQueryService>();
        services.AddScoped<ISponsorQueryService, SponsorQueryService>();
        services.AddScoped<ICityQueryService, CityQueryService>();
        services.AddScoped<IContentReportQueryService, ContentReportQueryService>();
        services.AddScoped<ITripQueryService, TripQueryService>();

        services.Decorate<IAttractionQueryService, CachedAttractionQueryService>();
        services.Decorate<ISiteQueryService, CachedSiteQueryService>();
        services.Decorate<ITouristQueryService, CachedTouristQueryService>();
        services.Decorate<IReviewQueryService, CachedReviewQueryService>();
        services.Decorate<ISponsorQueryService, CachedSponsorQueryService>();
        services.Decorate<ICityQueryService, CachedCityQueryService>();
        services.Decorate<IContentReportQueryService, CachedContentReportQueryService>();
        services.Decorate<ITripQueryService, CachedTripQueryService>();

        return services;
    }

    private static IServiceCollection AddLocalization(this IServiceCollection services)
    {
        // ResourcesPath is intentionally empty.
        // Our marker classes live in namespaces that already mirror the folder
        // structure (e.g. Infrastructure.Localization.Resources.ErrorResource
        // maps to Localization/Resources/ErrorResource.resx).
        // Setting ResourcesPath to "Resources" would double the path segment.
        services.AddLocalization(options => options.ResourcesPath = "");

        services.AddScoped<IEnumLocalizer, EnumLocalizer>();
        services.AddScoped<IErrorLocalizer, ErrorLocalizer>();

        return services;
    }

    private static IServiceCollection AddApplicationDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        bool useStaticData = configuration.GetValue<bool>("StaticData:InMemory");

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.AddInterceptors(
                serviceProvider.GetRequiredService<AuditableEntityInterceptor>(),
                serviceProvider.GetRequiredService<DomainEventDispatcherInterceptor>()
            );

            if (useStaticData)
            {
                options.UseInMemoryDatabase("RafeeqStaticData");
            }
            else
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("SqlServer"),
                    b =>
                    {
                        b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        b.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                    });
            }

            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
        });

        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<DomainEventDispatcherInterceptor>();

        return services;
    }

    private static IServiceCollection AddIdentity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

        var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()!;

        services.Configure<DataProtectionTokenProviderOptions>(options =>
            options.TokenLifespan = TimeSpan.FromHours(jwtSettings.TokenLifespanHours));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ClockSkew = TimeSpan.Zero, // Remove default 5 min clock skew

                NameClaimType = JwtRegisteredClaimNames.Sub, // Important for UserIdentifier
            };

            // ✅ Read token from cookie if no Bearer header present
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    // Try cookie first (web), then fall back to header (mobile)
                    if (context.Request.Cookies.TryGetValue("access_token", out var cookieToken))
                        context.Token = cookieToken;

                    return Task.CompletedTask;
                }
            };

            // options.MapInboundClaims = false; // Prevents automatic mapping of claims to Microsoft-specific claim types
        });

        services.AddScoped<JwtTokenGenerator>();

        return services;
    }

    private static IServiceCollection AddAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorization();

        return services;
    }

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

    private static IServiceCollection AddExternalServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCloudinary(configuration);
        services.AddScannerService(configuration);
        services.AddTripPlannerService(configuration);

        return services;
    }

    private static IServiceCollection AddTripPlannerService(this IServiceCollection services, IConfiguration configuration)
    {
        var tripPlannerSettings = configuration
           .GetSection(TripPlannerSettings.SectionName)
           .Get<TripPlannerSettings>()!;

        services.AddSingleton(tripPlannerSettings);

        // Named HttpClient — BaseUrl and timeout set here, not inside the service.
        // This keeps HttpClient lifecycle management in DI where it belongs.
        services.AddHttpClient<ITripPlannerService, HttpTripPlannerService>(client =>
        {
            client.BaseAddress = new Uri(tripPlannerSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(tripPlannerSettings.TimeoutSeconds);
        });

        return services;
    }
    
    private static IServiceCollection AddScannerService(this IServiceCollection services, IConfiguration configuration)
    {
        var scanSettings = configuration
           .GetSection(ImageScannerSettings.SectionName)
           .Get<ImageScannerSettings>()!;

        services.AddSingleton(scanSettings);

        // Named HttpClient — BaseUrl and timeout set here, not inside the service.
        // This keeps HttpClient lifecycle management in DI where it belongs.
        services.AddHttpClient<IImageScannerService, HttpImageScannerService>(client =>
        {
            client.BaseAddress = new Uri(scanSettings.BaseUrl);
            // client.Timeout = TimeSpan.FromSeconds(120);
            client.Timeout = TimeSpan.FromSeconds(scanSettings.TimeoutSeconds);
        });

        return services;
    }

    private static IServiceCollection AddCloudinary(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CloudinarySettings>(
            configuration.GetSection("Cloudinary"));

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;

            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret);

            return new Cloudinary(account);
        });

        services.AddScoped<IFileStorageService, CloudinaryStorageService>();
        // services.AddScoped<IFileStorageService, InMemoryFileStorageService>();

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RefreshTokenCleanupSettings>(
            configuration.GetSection(RefreshTokenCleanupSettings.SectionName));

        services.AddHostedService<RefreshTokenCleanupJob>();

        return services;
    }

    private static IServiceCollection AddLogging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                path: "logs/rafeeq-.txt",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddSerilog(dispose: true);
        });

        return services;
    }
}
