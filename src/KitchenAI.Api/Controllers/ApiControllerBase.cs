using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KitchenAI.Api.Controllers;

/// <summary>Base controller providing common helpers for all KitchenAI API controllers.</summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    private IMediator? _mediator;

    /// <summary>Lazily resolved <see cref="IMediator"/> instance.</summary>
    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    /// <summary>Returns the authenticated user's ID from the JWT <c>sub</c> / NameIdentifier claim.</summary>
    protected Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User identity claim not found."));

    /// <summary>Returns the primary household ID embedded in the JWT token.</summary>
    protected Guid GetCurrentHouseholdId() =>
        Guid.Parse(User.FindFirstValue("householdId")
            ?? throw new UnauthorizedAccessException("Household identity claim not found."));

    /// <summary>Throws <see cref="UnauthorizedAccessException"/> when the route household does not match the JWT claim.</summary>
    protected void ValidateHouseholdAccess(Guid householdId)
    {
        if (GetCurrentHouseholdId() != householdId)
            throw new UnauthorizedAccessException("Access to this household is not permitted.");
    }
}
