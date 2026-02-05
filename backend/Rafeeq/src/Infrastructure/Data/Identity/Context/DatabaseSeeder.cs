using Domain.Enums;
using Infrastructure.User;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Identity.Context;

public class DatabaseSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DatabaseSeeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminsAsync();
    }


    private async Task SeedRolesAsync()
    {
        string[] roles = [nameof(UserRole.Admin), nameof(UserRole.Customer)];

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private async Task SeedAdminsAsync()
    {
        ApplicationUser admin = new() { Email = "a@a.a", UserName = "admin" };
        if (_userManager.Users.Any(u => u.Email == "a@a.a"))
            return;

        var result = await _userManager.CreateAsync(admin, "123456");
        if (!result.Succeeded)
            throw new ApplicationException();

        await _userManager.AddToRoleAsync(admin, nameof(UserRole.Admin));
    }
}