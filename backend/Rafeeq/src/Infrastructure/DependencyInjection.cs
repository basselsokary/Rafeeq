using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Services;
using CloudinaryDotNet;
using Domain.Common.Constants;
using Domain.Common.Interfaces;
using Domain.Repositories;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Authorization.Handlers;
using Infrastructure.Authorization.Requirements;
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
using Infrastructure.Persistence.Seeding.Seeders;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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

        // Add Identity & Authentication & Authorization
        services.AddIdentity(configuration)
            .AddJwtAuthentication(configuration)
            .AddAuthorization();

        services.AddLocalization();

        services.AddMemoryCache();

        services.AddExternalServices(configuration);

        // Add Background Jobs
        services.AddBackgroundJobs(configuration);

        services.AddOtherServices(configuration);
        services.AddSeeding();

        return services;
    }

    private static IServiceCollection AddOtherServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CurrentUserService>();
        services.AddScoped<IUserContext, CurrentUserService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.Decorate<IUserManagementService, CachedUserManagementService>();
        services.AddScoped<ICacheService, BaseCache>();
        services.AddScoped<IImageProcessingService, ImageProcessingService>();
        
        // Add email service
        services.Configure<EmailOptions>(configuration.GetSection("EmailOptions"));
        services.AddSingleton<IEmailService, EmailService>();

        services.AddScoped<IUserCredentialService, UserCredentialService>();
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
        services.AddScoped<ICityQueryService, CityQueryService>();
        services.AddScoped<IArtifactQueryService, ArtifactQueryService>();
        services.AddScoped<IAttractionQueryService, AttractionQueryService>();
        services.AddScoped<ISiteQueryService, SiteQueryService>();
        services.AddScoped<ISponsorQueryService, SponsorQueryService>();
        services.AddScoped<ITouristQueryService, TouristQueryService>();
        services.AddScoped<IReviewQueryService, ReviewQueryService>();
        services.AddScoped<IContentReportQueryService, ContentReportQueryService>();
        services.AddScoped<ITripQueryService, TripQueryService>();
        services.AddScoped<IDashboardQueryService, DashboardQueryService>();
        services.AddScoped<IMapQueryService, MapQueryService>();

        services.Decorate<ICityQueryService, CachedCityQueryService>();
        services.Decorate<IArtifactQueryService, CachedArtifactQueryService>();
        services.Decorate<IAttractionQueryService, CachedAttractionQueryService>();
        services.Decorate<ISiteQueryService, CachedSiteQueryService>();
        services.Decorate<ISponsorQueryService, CachedSponsorQueryService>();
        services.Decorate<ITouristQueryService, CachedTouristQueryService>();
        services.Decorate<IReviewQueryService, CachedReviewQueryService>();
        services.Decorate<IContentReportQueryService, CachedContentReportQueryService>();
        services.Decorate<ITripQueryService, CachedTripQueryService>();
        services.Decorate<IDashboardQueryService, CachedDashboardQueryService>();
        services.Decorate<IMapQueryService, CachedMapQueryService>();

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

    private static IServiceCollection AddSeeding(this IServiceCollection services)
    {
        // ── Infrastructure ────────────────────────────────────────────────
        services.AddSingleton<CsvMapRegistry>();
        services.AddScoped<ICsvFileParser, CsvParser>();
        services.AddScoped<DatabaseSeeder>();

        // ── Seeders (registered in any order — DatabaseSeeder sorts by Order) ──
        services.AddScoped<IDataSeeder, CitySeeder>();
        services.AddScoped<IDataSeeder, SiteSeeder>();
        services.AddScoped<IDataSeeder, OpeningHourSeeder>();
        services.AddScoped<IDataSeeder, NearestTransportationSeeder>();
        services.AddScoped<IDataSeeder, AttractionSeeder>();
        services.AddScoped<IDataSeeder, ArtifactSeeder>();
        services.AddScoped<IDataSeeder, SponsorSeeder>();
        services.AddScoped<IDataSeeder, OfferSeeder>();

        return services;
    }

    private static IServiceCollection AddApplicationDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        bool useStaticData = configuration.GetValue<bool>("StaticData:InMemory");

        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<DomainEventDispatcherInterceptor>();

        services.AddScoped(sp =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.ConfigureDbContext(configuration, useStaticData);
            optionsBuilder.AddInterceptors(
                sp.GetRequiredService<AuditableEntityInterceptor>(),
                sp.GetRequiredService<DomainEventDispatcherInterceptor>()
            );
            return new ApplicationDbContext(optionsBuilder.Options);
        });

        // services.AddDbContext<ApplicationDbContext>((sp, options) =>
        // {
        //     options.ConfigureDbContext(configuration, useStaticData);
        //     options.AddInterceptors(
        //         sp.GetRequiredService<AuditableEntityInterceptor>(),
        //         sp.GetRequiredService<DomainEventDispatcherInterceptor>()
        //     );
        // });

        services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            options.ConfigureDbContext(configuration, useStaticData);
        });

        return services;
    }

    private static void ConfigureDbContext(
        this DbContextOptionsBuilder options,
        IConfiguration configuration,
        bool useStaticData)
    {
        if (useStaticData)
        {
            options.UseInMemoryDatabase("RafeeqStaticData");
        }
        else
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultSqlServer"),
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
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

        var jwtSettings = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!;

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

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, RolesHandler>();
        services.AddScoped<IAuthorizationHandler, EmailVerifiedHandler>();
        services.AddScoped<IAuthorizationHandler, AccountActiveHandler>();
        services.AddScoped<IAuthorizationHandler, ResourceOwnerHandler>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.SuperAdminOnly, policy =>
                policy.AddRequirements(new RolesRequirement(UserRoles.SuperAdmin)));
            
            options.AddPolicy(Policies.AdminOnly, policy =>
                policy.AddRequirements(new RolesRequirement(UserRoles.SuperAdmin, UserRoles.Admin)));

            options.AddPolicy(Policies.ModeratorOrAdmin, policy =>
                policy.AddRequirements(new RolesRequirement(UserRoles.SuperAdmin, UserRoles.Moderator, UserRoles.Admin)));

            options.AddPolicy(Policies.TouristOnly, policy =>
                policy.AddRequirements(new RolesRequirement(UserRoles.Tourist)));

            options.AddPolicy(Policies.CanManageUsers, policy =>
                policy.AddRequirements(new RolesRequirement(UserRoles.SuperAdmin, UserRoles.Admin)));

            options.AddPolicy(Policies.CanManageCities, policy =>
                policy.AddRequirements(new RolesRequirement(UserRoles.SuperAdmin, UserRoles.Admin, UserRoles.Moderator)));
            
            options.AddPolicy(Policies.CanManageSites, policy =>
                policy.AddRequirements(new RolesRequirement(UserRoles.SuperAdmin, UserRoles.Admin, UserRoles.Moderator)));
            
            options.AddPolicy(Policies.CanManageArtifacts, policy =>
                policy.AddRequirements(new RolesRequirement(UserRoles.SuperAdmin, UserRoles.Admin, UserRoles.Moderator)));
            
            options.AddPolicy(Policies.CanManageAttractions, policy =>
                policy.AddRequirements(new RolesRequirement(UserRoles.SuperAdmin, UserRoles.Admin, UserRoles.Moderator)));

            options.AddPolicy(Policies.CanManageSponsors, policy =>
                policy.AddRequirements(new RolesRequirement(UserRoles.SuperAdmin, UserRoles.Admin)));

            options.AddPolicy(Policies.CanModerateContent, policy =>
                policy.AddRequirements(new RolesRequirement(UserRoles.SuperAdmin, UserRoles.Admin, UserRoles.Moderator)));

            options.AddPolicy(Policies.CanViewAnalytics, policy =>
                policy.AddRequirements(new RolesRequirement(UserRoles.SuperAdmin, UserRoles.Admin, UserRoles.Moderator)));
            
            options.AddPolicy(Policies.CanImportData, policy =>
                policy.AddRequirements(new RolesRequirement(UserRoles.SuperAdmin)));

            // Tourists can manage their own trips
            // (resource ownership is checked separately via IAuthorizationService)
            options.AddPolicy(Policies.CanManageOwnTrips, policy =>
                policy
                    .AddRequirements(new RolesRequirement(UserRoles.Tourist))
                    .AddRequirements(new AccountActiveRequirement()));

            options.AddPolicy(Policies.CanWriteReviews, policy =>
                policy
                    .AddRequirements(new RolesRequirement(UserRoles.Tourist))
                    .AddRequirements(new EmailVerifiedRequirement())
                    .AddRequirements(new AccountActiveRequirement()));

            options.AddPolicy(Policies.CanReportContent, policy =>
                policy
                    .RequireAuthenticatedUser()
                    .AddRequirements(new AccountActiveRequirement()));

            // Resource-owner policy (used with IAuthorizationService directly, not via [Authorize])
            options.AddPolicy(Policies.ResourceOwner, policy =>
                policy.AddRequirements(new ResourceOwnerRequirement(allowAdminOverride: true)));

            // Any endpoint with [Authorize] but no policy requires authentication
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Endpoints with [AllowAnonymous] are not affected
            options.FallbackPolicy = null;
        });

        return services;
    }

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
           .GetSection(TripPlannerOptions.SectionName)
           .Get<TripPlannerOptions>()!;

        services.AddSingleton(tripPlannerSettings);

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
           .GetSection(ImageScannerOptions.SectionName)
           .Get<ImageScannerOptions>()!;

        services.AddSingleton(scanSettings);

        services.AddHttpClient<IImageScannerService, HttpImageScannerService>(client =>
        {
            client.BaseAddress = new Uri(scanSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(scanSettings.TimeoutSeconds);
        });

        return services;
    }

    private static IServiceCollection AddCloudinary(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CloudinaryOptions>(
            configuration.GetSection("Cloudinary"));

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<CloudinaryOptions>>().Value;

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
}
