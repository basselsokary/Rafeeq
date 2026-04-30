using API.Controllers.Base;
using Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admins/dashboard/")]
[Authorize(Roles = UserRoles.Admin)]
public class AdminDashboardController : ApiBaseController
{
    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        var stats = new
        {
            TotalUsers = 1000,
            ActiveUsers = 800,
            TotalOrders = 500,
            Revenue = 100000
        };

        return Ok(stats);
    }
	
}
