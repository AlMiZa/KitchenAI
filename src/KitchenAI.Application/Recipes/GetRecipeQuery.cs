using MediatR;

namespace KitchenAI.Application.Recipes;

/// <summary>Returns a single saved recipe belonging to the household.</summary>
public record GetRecipeQuery(Guid HouseholdId, Guid RecipeId) : IRequest<RecipeDto>;
