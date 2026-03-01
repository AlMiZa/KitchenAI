using MediatR;

namespace KitchenAI.Application.Recipes;

/// <summary>Checks whether all ingredients for a saved recipe are available in the household's inventory.</summary>
public record CheckRecipeAvailabilityQuery(Guid HouseholdId, Guid RecipeId) : IRequest<AvailabilityResultDto>;
