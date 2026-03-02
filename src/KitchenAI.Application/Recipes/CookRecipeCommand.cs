using MediatR;

namespace KitchenAI.Application.Recipes;

/// <summary>Records that a household has cooked a recipe.</summary>
public record CookRecipeCommand(Guid HouseholdId, Guid RecipeId) : IRequest;
