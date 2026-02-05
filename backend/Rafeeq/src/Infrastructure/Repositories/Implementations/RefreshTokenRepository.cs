using Application.Common.Interfaces.Repositories;
using Infrastructure.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Infrastructure.Data.Application.Context;

namespace Infrastructure.Repositories.Implementations;

internal class RefreshTokenRepository(
    AppDbContext context,
    IConfiguration configuration,
    UserManager<ApplicationUser> userManager) : IRefreshTokenRepository
{
    // public async Task AddRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default)
    //     => await context.RefreshTokens.AddAsync(
    //         RefreshToken.Create(
    //             refreshToken,
    //             userId,
    //             DateTime.UtcNow.AddDays(
    //                 int.Parse(configuration[$"{nameof(JwtSettings)}:{nameof(JwtSettings.RefreshTokenExpirationInDays)}"]
    //                     ?? throw new InvalidCastException("Failed to fetch configuration data."))
    //             )
    //         ),
    //         cancellationToken
    //     );

    // public async Task<UserDto?> GetUserDtoByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    // {
    //     var user = await context.Users
    //         .AsSplitQuery()
    //         .Include(u => u.RefreshTokens)
    //         .Where(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken))
    //         .FirstOrDefaultAsync(cancellationToken);
        
    //     if (user == null)
    //         return null;

    //     var roles = await userManager.GetRolesAsync(user);

    //     return new UserDto
    //     {
    //         Id = user.Id,
    //         UserName = user.UserName!,
    //         Email = user.Email!,
    //         Roles = roles
    //     };
    // }

    // public async Task<bool> IsRefreshTokenActiveAsync(string refreshToken, CancellationToken cancellationToken = default)
    // {
    //     return await context.RefreshTokens
    //         .AnyAsync(rt => rt.Token == refreshToken && rt.IsActive, cancellationToken);
    // }

    // public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    // {
    //     var refreshTokenEntity = await context.RefreshTokens
    //         .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);
        
    //     if (refreshTokenEntity == null)
    //         return false;

    //     refreshTokenEntity.Revoke();

    //     return true;
    // }
}
