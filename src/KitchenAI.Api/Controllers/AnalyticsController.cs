using KitchenAI.Application.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenAI.Api.Controllers;

/// <summary>Analytics endpoints for a household.</summary>
[Authorize]
[Route("api/households/{hid:guid}/analytics")]
public class AnalyticsController : ApiControllerBase
{
    /// <summary>Returns the analytics summary for the household.</summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(Guid hid, CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        var result = await Mediator.Send(new GetAnalyticsSummaryQuery(hid), ct);
        return Ok(result);
    }
}
