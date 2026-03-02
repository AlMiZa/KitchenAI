using System.Text.Json;
using KitchenAI.Application.Persistence;
using KitchenAI.Application.Resources;
using KitchenAI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Recipes;

/// <summary>Records a <c>recipe_cooked</c> analytics event for the given recipe.</summary>
public class CookRecipeHandler(IAppDbContext db) : IRequestHandler<CookRecipeCommand>
{
    /// <inheritdoc/>
    public async Task Handle(CookRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await db.Recipes
            .FirstOrDefaultAsync(
                r => r.Id == request.RecipeId && r.HouseholdId == request.HouseholdId,
                cancellationToken)
            ?? throw new KeyNotFoundException(Messages.Get("Recipe_NotFound", request.RecipeId));

        db.AnalyticsEvents.Add(new AnalyticsEvent
        {
            Id = Guid.NewGuid(),
            HouseholdId = request.HouseholdId,
            EventType = "recipe_cooked",
            Metadata = JsonSerializer.Serialize(new { recipeTitle = recipe.Title }),
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}
