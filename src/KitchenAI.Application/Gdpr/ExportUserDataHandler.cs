using System.Text.Json;
using KitchenAI.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Gdpr;

/// <summary>Exports all user and household data for GDPR data portability.</summary>
public class ExportUserDataHandler(IAppDbContext db) : IRequestHandler<ExportUserDataQuery, string>
{
    /// <inheritdoc/>
    public async Task<string> Handle(ExportUserDataQuery request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FindAsync([request.UserId], cancellationToken)
            ?? throw new KeyNotFoundException($"User {request.UserId} not found.");

        var households = await db.Households
            .Where(h => h.OwnerUserId == user.Id)
            .ToListAsync(cancellationToken);

        var householdIds = households.Select(h => h.Id).ToList();

        var items = await db.Items
            .Where(i => householdIds.Contains(i.HouseholdId))
            .ToListAsync(cancellationToken);

        var recipes = await db.Recipes
            .Where(r => householdIds.Contains(r.HouseholdId))
            .ToListAsync(cancellationToken);

        var export = new
        {
            user = new
            {
                user.Id,
                user.Email,
                user.DisplayName,
                user.Locale,
                user.CreatedAt
            },
            households = households.Select(h => new { h.Id, h.Name, h.CreatedAt }),
            items = items.Select(i => new
            {
                i.Id, i.HouseholdId, i.Name, i.Quantity, i.Unit,
                i.ExpiryDate, i.Price, i.IsArchived
            }),
            recipes = recipes.Select(r => new { r.Id, r.HouseholdId, r.Title, r.CreatedAt })
        };

        return JsonSerializer.Serialize(export, new JsonSerializerOptions { WriteIndented = true });
    }
}
