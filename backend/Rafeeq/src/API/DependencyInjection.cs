using System.Text.Json;
using System.Text.Json.Serialization;
using API.Infrustructure;
using Microsoft.OpenApi.Models;

namespace API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddSwaggerGen();

        services.AddControllers();
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
            });

        services.AddEndpointsApiExplorer();

        // services.AddCors();

        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
            // options.LowercaseQueryStrings = true; // Optional
        });

        return services;
    }

    private static IServiceCollection AddSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(static option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Rafeeq API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Please enter a valid token (e.g. => 'Authorization: Bearer <YourToken>')",
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
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
            // options.AddPolicy("AllowAllOrigins", policy =>
            // {
            //     policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            // });

            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://127.0.0.1:5500") // Replace with your frontend origin
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); // Required for SignalR with authentication or cookies
            });
        });

        return services;
    }
}
