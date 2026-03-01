using MediatR;

namespace KitchenAI.Application.Recipes;

/// <summary>Returns all saved recipes for the specified household.</summary>
public record GetRecipesQuery(Guid HouseholdId) : IRequest<IList<RecipeDto>>;
