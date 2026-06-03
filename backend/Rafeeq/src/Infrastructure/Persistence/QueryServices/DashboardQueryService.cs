using Application.Common.Interfaces.QueryServices;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;
using Application.DTOs.Admins;

namespace Infrastructure.Persistence.QueryServices;

internal sealed class DashboardQueryService(
    ApplicationDbContext context) : IDashboardQueryService
{
    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var stats = await context.Database.SqlQueryRaw<DashboardStatsDto>(@"
            SELECT
                (SELECT COUNT(*) FROM Rafeeq.Users) AS TotalUsers,
                (SELECT COUNT(*) FROM Rafeeq.Sites) AS TotalSites,
                (SELECT COUNT(*) FROM Rafeeq.Sponsors) AS TotalSponsors,
                (SELECT COUNT(*) FROM Rafeeq.Cities) AS TotalCities
            ").FirstOrDefaultAsync();
        
        return stats ?? new(0, 0, 0, 0);
    }

}
