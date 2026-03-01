using KitchenAI.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Recipes;

/// <summary>Returns a single saved recipe.</summary>
public class GetRecipeHandler(IAppDbContext db) : IRequestHandler<GetRecipeQuery, RecipeDto>
{
    /// <inheritdoc/>
    public async Task<RecipeDto> Handle(GetRecipeQuery request, CancellationToken cancellationToken)
    {
        var recipe = await db.Recipes
            .Include(r => r.RecipeIngredients)
            .FirstOrDefaultAsync(r => r.Id == request.RecipeId && r.HouseholdId == request.HouseholdId, cancellationToken)
            ?? throw new KeyNotFoundException($"Recipe {request.RecipeId} not found.");

        return SaveRecipeHandler.ToDto(recipe);
    }
}
