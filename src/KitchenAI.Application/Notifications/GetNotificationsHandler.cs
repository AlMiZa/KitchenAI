using KitchenAI.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Notifications;

/// <summary>Returns all undelivered notifications for a household.</summary>
public class GetNotificationsHandler(IAppDbContext db)
    : IRequestHandler<GetNotificationsQuery, IList<NotificationDto>>
{
    /// <inheritdoc/>
    public async Task<IList<NotificationDto>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var notifications = await db.Notifications
            .Where(n => n.HouseholdId == request.HouseholdId && !n.Delivered)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

        return notifications
            .Select(n => new NotificationDto(n.Id, n.Type, n.Payload, n.Delivered, n.CreatedAt))
            .ToList();
    }
}
