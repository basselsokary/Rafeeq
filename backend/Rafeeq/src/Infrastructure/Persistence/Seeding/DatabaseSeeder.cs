using Domain.Common.Constants;
using Infrastructure.Identity.Entities;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeding;

/// <summary>
/// Orchestrates all <see cref="IDataSeeder"/> implementations in the correct order.
/// Registered as a scoped service and invoked once during application startup.
/// </summary>
internal sealed class DatabaseSeeder(
    ApplicationDbContext context,
    RoleManager<IdentityRole<Guid>> roleManager,
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
    IEnumerable<IDataSeeder> seeders,
    ILogger<DatabaseSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        context.Database.Migrate();
        
        // if (await context.Cities.AnyAsync(cancellationToken))
        // {
        //     logger.LogInformation("Database already contains data; skipping seeding.");
        //     return;
        // }

        if (!roleManager.Roles.Any())
        {
            await EnsureRolesAsync(roleManager);
            await SeedInitialAdminAsync(userManager, configuration);
        }

        var ordered = seeders
            .OrderBy(s => s.Order)
            .ToList();

        logger.LogInformation("Starting database seeding — {Count} seeder(s) registered.", ordered.Count);

        foreach (var seeder in ordered)
        {
            var name = seeder.GetType().Name;
            try
            {
                logger.LogInformation("  → Running {Seeder} (order {Order})…", name, seeder.Order);
                await seeder.SeedAsync(cancellationToken);
                logger.LogInformation("  ✓ {Seeder} completed.", name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "  ✗ {Seeder} failed.", name);
                throw; // Bubble up — seeding failure should prevent startup in production.
            }
        }

        logger.LogInformation("Database seeding finished successfully.");
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        var existingRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
        
        foreach (string roleName in UserRoles.AllRoles)
        {
            bool exists = existingRoles.Contains(roleName);
            if (!exists)
            {
                IdentityResult createRoleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                if (!createRoleResult.Succeeded)
                {
                    string errors = string.Join(", ", createRoleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create role '{roleName}': {errors}");
                }
            }
        }
    }

    private static async Task SeedInitialAdminAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        // Check if admin already exists
        var adminEmail = configuration["AdminSeed:Email"] 
            ?? throw new InvalidOperationException("Admin seed email not configured");
        
        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin != null)
        {
            // Admin already exists, skip seeding
            return;
        }

        // 3. Create the initial admin user
        var adminUserResult = AdminUser.Create(
            Guid.NewGuid(),
            "admin",
            adminEmail,
            "System",
            "Administrator",
            "System Administrator");
        
        if (adminUserResult.Failed)
            throw new InvalidOperationException("Failed to create admin user instance");

        // Get password from configuration
        var adminPassword = configuration["AdminSeed:Password"] 
            ?? throw new InvalidOperationException("Admin seed password not configured");

        var result = await userManager.CreateAsync(adminUserResult.Value, adminPassword);

        if (result.Succeeded)
        {
            // 4. Assign Admin role
            await userManager.AddToRoleAsync(adminUserResult.Value, UserRoles.SuperAdmin);
            
            Console.WriteLine($"✅ Initial admin account created: {adminEmail}");
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create admin: {errors}");
        }
    }
}
