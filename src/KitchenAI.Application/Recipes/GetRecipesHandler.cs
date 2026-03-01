using KitchenAI.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Recipes;

/// <summary>Returns all saved recipes for a household.</summary>
public class GetRecipesHandler(IAppDbContext db) : IRequestHandler<GetRecipesQuery, IList<RecipeDto>>
{
    /// <inheritdoc/>
    public async Task<IList<RecipeDto>> Handle(GetRecipesQuery request, CancellationToken cancellationToken)
    {
        var recipes = await db.Recipes
            .Include(r => r.RecipeIngredients)
            .Where(r => r.HouseholdId == request.HouseholdId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return recipes.Select(SaveRecipeHandler.ToDto).ToList();
    }
}
