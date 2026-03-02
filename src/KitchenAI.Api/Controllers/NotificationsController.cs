using KitchenAI.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenAI.Api.Controllers;

/// <summary>Notification retrieval and subscription management for a household.</summary>
[Authorize]
[Route("api/households/{hid:guid}/notifications")]
public class NotificationsController : ApiControllerBase
{
    /// <summary>Returns undelivered notifications for the household.</summary>
    [HttpGet]
    public async Task<IActionResult> GetNotifications(Guid hid, CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        var result = await Mediator.Send(new GetNotificationsQuery(hid), ct);
        return Ok(result);
    }

    /// <summary>Updates the current user's notification preferences for the household.</summary>
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe(
        Guid hid,
        [FromBody] SubscribeNotificationsCommand command,
        CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        await Mediator.Send(command with { HouseholdId = hid, UserId = GetCurrentUserId() }, ct);
        return Ok();
    }

    /// <summary>Creates a test notification for the household and returns it.</summary>
    [HttpPost("test")]
    public async Task<IActionResult> Test(Guid hid, CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        var result = await Mediator.Send(new CreateTestNotificationCommand(hid), ct);
        return Ok(result);
    }
}
