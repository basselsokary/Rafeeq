using Infrastructure;
using Application;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddPresentation()
            .AddApplication()
            .AddInfrastructure(builder.Configuration);

        var app = builder.Build();

        Task.Run(async () =>
        {
            await app.Services.EnsureStaticDataAsync(builder.Configuration);
        });

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        #region Middlewares
        // app.UseCors("AllowAllOrigins");
        app.UseExceptionHandler();
        
        app.UseCors();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();
        #endregion

        app.Run();
    }
}
