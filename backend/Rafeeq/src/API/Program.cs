using Infrastructure;
using Application;
using System.Globalization;

namespace API;

public class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddPresentation()
            .AddApplication()
            .AddInfrastructure(builder.Configuration);

        var app = builder.Build();

        await app.Services.EnsureStaticDataAsync(builder.Configuration);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Rafeeq API v1");

                // ✅ This tells the browser to send cookies with every Swagger request
                options.ConfigObject.AdditionalItems["withCredentials"] = true;
            });
        }

        #region Middlewares
        
        app.UseExceptionHandler();

        // With Serilog
        // app.UseSerilogRequestLogging(opts =>
        // {
        //     opts.MessageTemplate = "{RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
        //     opts.EnrichDiagnosticContext = (ctx, httpCtx) =>
        //     {
        //         ctx.Set("UserId", httpCtx.User?.FindFirst("sub")?.Value);
        //         ctx.Set("ClientIP", httpCtx.Connection.RemoteIpAddress);
        //     };
        // });

        app.UseRequestLocalization(options =>
        {
            var supportedCultures = new[]
            {
                "ar", // Arabic
                "en", // English 
                "ru", // Russian
                "fr", // French
                "es", // Spanish
                "de", // German
                "zh", // Chinese
                "ja", // Japanese
                "it"  // Italian
            };

            options.SetDefaultCulture("en");
            options.AddSupportedCultures(supportedCultures);
            options.AddSupportedUICultures(supportedCultures);
        });

        app.UseCors();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        // Here is an endpoint to test the localization of error messages in validators
        app.Use(async (context, next) =>
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"\nTesting localization...\nCurrent culture is => {CultureInfo.CurrentCulture}\n");
                Console.ResetColor();
                await next();
            }
        );

        #endregion

        app.Run();
    }
}
