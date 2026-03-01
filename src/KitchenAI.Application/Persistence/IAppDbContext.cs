using KitchenAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Persistence;

/// <summary>Abstraction over the EF Core database context used by application handlers.</summary>
public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Household> Households { get; }
    DbSet<HouseholdMember> HouseholdMembers { get; }
    DbSet<Item> Items { get; }
    DbSet<Recipe> Recipes { get; }
    DbSet<RecipeIngredient> RecipeIngredients { get; }
    DbSet<GeneratedRecipe> GeneratedRecipes { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<AnalyticsEvent> AnalyticsEvents { get; }
    DbSet<AdminSetting> AdminSettings { get; }

    /// <summary>Saves all pending changes to the database.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
