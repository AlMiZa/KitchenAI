using KitchenAI.Application.Households;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenAI.Api.Controllers;

/// <summary>Household management endpoints.</summary>
[Authorize]
[Route("api/households")]
public class HouseholdsController : ApiControllerBase
{
    /// <summary>Returns all households the current user belongs to.</summary>
    [HttpGet]
    public async Task<IActionResult> GetHouseholds(CancellationToken ct)
    {
        var result = await Mediator.Send(new GetHouseholdsQuery(GetCurrentUserId()), ct);
        return Ok(result);
    }

    /// <summary>Creates a new household owned by the current user.</summary>
    [HttpPost]
    public async Task<IActionResult> CreateHousehold([FromBody] CreateHouseholdCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command with { UserId = GetCurrentUserId() }, ct);
        return CreatedAtAction(nameof(GetHouseholds), result);
    }

    /// <summary>Joins the current user to an existing household.</summary>
    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> JoinHousehold(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new JoinHouseholdCommand(GetCurrentUserId(), id), ct);
        return Ok(result);
    }
}
