using System.Text.Json;
using System.Text.Json.Serialization;
using API.Configurations;
using API.Middlewares;
using Infrastructure.Logging;
using Microsoft.OpenApi.Models;

namespace API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services, IHostBuilder hostBuilder, IConfiguration configuration)
    {
        // global exception handling and problem details
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddSwaggerGen();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                // By setting allowIntegerValues to false, we ensure that only valid
                // string representations of enum values are accepted, improving the robustness of our API.
                options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
            });

        services.AddEndpointsApiExplorer();

        services.AddCors();

        services.AddRateLimiter(configuration);
        hostBuilder.ConfigureSerilog(configuration);

        services.Configure<RouteOptions>(options =>
        {
            // Enforce lowercase URLs for consistency and SEO benefits.
            options.LowercaseUrls = true;
        });

        return services;
    }

    private static IServiceCollection AddSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title   = "Rafeeq API",
                Version = "v1"
            });

            // ─── Mobile: Bearer Token (standard) ──────────────────────────────
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name         = "Authorization",
                Type         = SecuritySchemeType.Http,
                Scheme       = "bearer",
                BearerFormat = "JWT",
                In           = ParameterLocation.Header,
                Description  = "Mobile clients: paste your JWT access token here."
            });

            // ─── Web: Cookie Auth ──────────────────────────────────────────────
            options.AddSecurityDefinition("CookieAuth", new OpenApiSecurityScheme
            {
                Name        = "access_token",
                Type        = SecuritySchemeType.ApiKey,
                In          = ParameterLocation.Cookie,
                Description = "Web clients: obtained automatically after calling POST /api/web/auth/login."
            });

            // ─── Apply both schemes globally ──────────────────────────────────
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                },
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "CookieAuth"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    private static IServiceCollection AddCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                // policy.AllowAnyOrigin()
                //     // .AllowCredentials()
                //     .AllowAnyHeader()
                //     .AllowAnyMethod();
                
                policy.WithOrigins(
                    "http://localhost:5173", // Frontend URL
                    "https://localhost:5173", // Frontend URL
                    "http://localhost:5143",
                    "https://rafeeq-inky.vercel.app",
                    "https://admin.rafeeq.live"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            });
        });

        return services;
    }
}
