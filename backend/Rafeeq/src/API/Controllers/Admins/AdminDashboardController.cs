using API.Controllers.Base;
using Application.Common.Interfaces.Messaging;
using Application.DTOs;
using Application.Queries.Dashboard;
using Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admins/dashboard/")]
[Authorize(Policy = Policies.CanViewAnalytics)]
public class AdminDashboardController : ApiBaseController
{
    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetStats(
        [FromServices] IQueryHandler<GetDashboardStatsQuery, DashboardStatsDto> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await queryHandler.HandleAsync(new GetDashboardStatsQuery(), cancellationToken);

        return HandleResult(result);
    }
	
}
