using KitchenAI.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Recipes;

/// <summary>Checks whether all ingredients for a recipe are available in the household inventory.</summary>
public class CheckRecipeAvailabilityHandler(IAppDbContext db)
    : IRequestHandler<CheckRecipeAvailabilityQuery, AvailabilityResultDto>
{
    /// <inheritdoc/>
    public async Task<AvailabilityResultDto> Handle(
        CheckRecipeAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        var recipe = await db.Recipes
            .Include(r => r.RecipeIngredients)
            .FirstOrDefaultAsync(r => r.Id == request.RecipeId && r.HouseholdId == request.HouseholdId, cancellationToken)
            ?? throw new KeyNotFoundException($"Recipe {request.RecipeId} not found.");

        var inventoryItems = await db.Items
            .Where(i => i.HouseholdId == request.HouseholdId && !i.IsArchived)
            .ToListAsync(cancellationToken);

        var availabilityItems = new List<AvailabilityItemDto>();
        var allAvailable = true;

        foreach (var ingredient in recipe.RecipeIngredients)
        {
            var available = inventoryItems
                .Where(i => i.Name.Equals(ingredient.Name, StringComparison.OrdinalIgnoreCase))
                .Sum(i => i.Quantity);

            var deficit = Math.Max(0, ingredient.Quantity - available);
            if (deficit > 0) allAvailable = false;

            availabilityItems.Add(new AvailabilityItemDto(
                ingredient.Name,
                ingredient.Quantity,
                available,
                deficit));
        }

        var status = allAvailable ? "ready" : "missing";
        return new AvailabilityResultDto(status, availabilityItems);
    }
}
