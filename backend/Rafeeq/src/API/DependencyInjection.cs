using API.Infrustructure;
using API.Services.Dispatchers;
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

        services.AddEndpointsApiExplorer();

        // services.AddCors();

        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
            // options.LowercaseQueryStrings = true; // Optional
        });

        services.AddServices();

        return services;
    }

    private static IServiceCollection AddSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(static option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "EventMaster API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Please enter a valid token (e.g. => 'Authorization: Bearer <YourToken>')",
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
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

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IRequestDispatcher, RequestDispatcher>();

        return services;
    }
}
