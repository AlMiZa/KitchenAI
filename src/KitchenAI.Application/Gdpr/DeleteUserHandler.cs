using KitchenAI.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KitchenAI.Application.Gdpr;

/// <summary>Permanently deletes the user and cascades the deletion to all owned household data.</summary>
public class DeleteUserHandler(IAppDbContext db, ILogger<DeleteUserHandler> logger) : IRequestHandler<DeleteUserCommand>
{
    /// <inheritdoc/>
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FindAsync([request.UserId], cancellationToken)
            ?? throw new KeyNotFoundException($"User {request.UserId} not found.");

        logger.LogWarning("Account deletion initiated for user {UserId}.", request.UserId);

        // Remove restricted FK referencing the user before deleting the user
        await db.GeneratedRecipes
            .Where(g => g.RequestedBy == request.UserId)
            .ExecuteDeleteAsync(cancellationToken);

        // Deleting owned households cascades to items, recipes, members, notifications, analytics
        var ownedHouseholds = await db.Households
            .Where(h => h.OwnerUserId == request.UserId)
            .ToListAsync(cancellationToken);

        db.Households.RemoveRange(ownedHouseholds);
        db.Users.Remove(user);

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Account deletion completed for user {UserId}.", request.UserId);
    }
}
