using System.Text.Json;
using KitchenAI.Application.Persistence;
using KitchenAI.Domain.Entities;
using KitchenAI.Domain.Enums;
using MediatR;

namespace KitchenAI.Application.Recipes;

/// <summary>Persists a recipe to the household library.</summary>
public class SaveRecipeHandler(IAppDbContext db) : IRequestHandler<SaveRecipeCommand, RecipeDto>
{
    /// <inheritdoc/>
    public async Task<RecipeDto> Handle(SaveRecipeCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecipeData.Title);

        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            HouseholdId = request.HouseholdId,
            Title = request.RecipeData.Title,
            Source = RecipeSource.Generated,
            Steps = JsonSerializer.Serialize(request.RecipeData.Steps),
            Nutrition = request.RecipeData.Nutrition,
            Servings = request.RecipeData.Servings,
            PrepTime = request.RecipeData.PrepTime,
            CookTime = request.RecipeData.CookTime,
            Tags = request.RecipeData.Tags,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var ingredients = request.RecipeData.Ingredients.Select(i => new RecipeIngredient
        {
            Id = Guid.NewGuid(),
            RecipeId = recipe.Id,
            Name = i.Name,
            Quantity = i.Quantity,
            Unit = i.Unit
        }).ToList();

        db.Recipes.Add(recipe);
        db.RecipeIngredients.AddRange(ingredients);
        db.AnalyticsEvents.Add(new AnalyticsEvent
        {
            Id = Guid.NewGuid(),
            HouseholdId = request.HouseholdId,
            EventType = "recipe_saved",
            Metadata = JsonSerializer.Serialize(new { recipeTitle = recipe.Title }),
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken);
        recipe.RecipeIngredients = ingredients;
        return ToDto(recipe);
    }

    internal static RecipeDto ToDto(Recipe recipe) => new(
        recipe.Id,
        recipe.HouseholdId,
        recipe.Title,
        recipe.Source,
        recipe.RecipeIngredients.Select(i => new RecipeIngredientDto(i.Id, i.Name, i.Quantity, i.Unit)).ToList(),
        recipe.Steps,
        recipe.Nutrition,
        recipe.Servings,
        recipe.PrepTime,
        recipe.CookTime,
        recipe.Tags,
        recipe.CreatedAt);
}
