using System.Text.Json;
using KitchenAI.Application.Persistence;
using KitchenAI.Application.Services;
using KitchenAI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Recipes;

/// <summary>Handles recipe generation via the LLM service.</summary>
public class GenerateRecipeHandler(IAppDbContext db, ILlmService llmService)
    : IRequestHandler<GenerateRecipeCommand, IList<GeneratedRecipeDto>>
{
    /// <inheritdoc/>
    public async Task<IList<GeneratedRecipeDto>> Handle(
        GenerateRecipeCommand request,
        CancellationToken cancellationToken)
    {
        var items = await db.Items
            .Where(i => i.HouseholdId == request.HouseholdId && !i.IsArchived && i.Quantity > 0)
            .ToListAsync(cancellationToken);

        var constraints = request.Constraints ?? new RecipeConstraints();
        var recipes = await llmService.GenerateRecipesAsync(items, constraints, cancellationToken);

        var generatedRecord = new GeneratedRecipe
        {
            Id = Guid.NewGuid(),
            RecipeJson = JsonSerializer.Serialize(recipes),
            Rationale = "Generated from household inventory",
            RequestedBy = request.RequestedByUserId,
            MatchedInventorySnapshot = JsonSerializer.Serialize(items.Select(i => i.Id)),
            CreatedAt = DateTime.UtcNow
        };

        db.GeneratedRecipes.Add(generatedRecord);
        db.AnalyticsEvents.Add(new AnalyticsEvent
        {
            Id = Guid.NewGuid(),
            HouseholdId = request.HouseholdId,
            EventType = "recipe_generated",
            Metadata = JsonSerializer.Serialize(new { recipeCount = recipes.Count }),
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken);
        return recipes;
    }
}
