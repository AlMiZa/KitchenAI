using MediatR;

namespace KitchenAI.Application.Recipes;

/// <summary>Generates recipe suggestions for the given household using the LLM service.</summary>
public record GenerateRecipeCommand(
    Guid HouseholdId,
    Guid RequestedByUserId,
    RecipeConstraints? Constraints = null) : IRequest<IList<GeneratedRecipeDto>>;
