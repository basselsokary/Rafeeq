using Infrastructure;
using Application;
using Serilog;
using API.Configurations;

namespace API;

public class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddPresentation(builder.Host, builder.Configuration)
            .AddApplication()
            .AddInfrastructure(builder.Configuration);

        var app = builder.Build();

        // await app.Services.EnsureStaticDataAsync(builder.Configuration);
        await app.Services.SeedAsync();

        // if (app.Environment.IsDevelopment())
        // {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Rafeeq API v1");

                // ✅ This tells the browser to send cookies with every Swagger request
                options.ConfigObject.AdditionalItems["withCredentials"] = true;
            });
        // }

        #region Middlewares
        
        app.UseExceptionHandler();

        app.UseSerilogRequestLogging(opts =>
        {
            opts.MessageTemplate = "{RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
            opts.EnrichDiagnosticContext = (ctx, httpCtx) =>
            {
                ctx.Set("UserId", httpCtx.User?.FindFirst("sub")?.Value);
                ctx.Set("ClientIP", httpCtx.Connection.RemoteIpAddress);
            };
        });

        app.UseLocalization();

        app.UseRouting();
        
        app.UseCors();

        app.UseAuthentication();

        app.UseAuthorization();
        
        app.UseRateLimiter();

        app.MapControllers();
        #endregion

        app.Run();
    }
}
