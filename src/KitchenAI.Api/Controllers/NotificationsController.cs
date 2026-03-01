using KitchenAI.Application.Notifications;
using KitchenAI.Domain.Entities;
using KitchenAI.Domain.Enums;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenAI.Api.Controllers;

/// <summary>Notification retrieval and subscription management for a household.</summary>
[Authorize]
[Route("api/households/{hid:guid}/notifications")]
public class NotificationsController(AppDbContext db) : ApiControllerBase
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

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            HouseholdId = hid,
            Type = NotificationType.RecipeSuggestion,
            Payload = """{"message":"This is a test notification."}""",
            Delivered = false,
            CreatedAt = DateTime.UtcNow
        };

        db.Notifications.Add(notification);
        await db.SaveChangesAsync(ct);

        return Ok(new NotificationDto(
            notification.Id,
            notification.Type,
            notification.Payload,
            notification.Delivered,
            notification.CreatedAt));
    }
}
