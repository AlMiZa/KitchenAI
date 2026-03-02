using KitchenAI.Application.Persistence;
using KitchenAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Infrastructure.Persistence;

/// <summary>Primary EF Core database context for KitchenAI.</summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Household> Households => Set<Household>();
    public DbSet<HouseholdMember> HouseholdMembers => Set<HouseholdMember>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<GeneratedRecipe> GeneratedRecipes => Set<GeneratedRecipe>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AnalyticsEvent> AnalyticsEvents => Set<AnalyticsEvent>();
    public DbSet<AdminSetting> AdminSettings => Set<AdminSetting>();
    public DbSet<MagicLinkToken> MagicLinkTokens => Set<MagicLinkToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.DisplayName).IsRequired().HasMaxLength(128);
            entity.Property(u => u.Locale).IsRequired().HasMaxLength(10).HasDefaultValue("pl-PL");
        });

        // Household
        modelBuilder.Entity<Household>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Name).IsRequired().HasMaxLength(128);
            entity.HasOne(h => h.Owner)
                  .WithMany()
                  .HasForeignKey(h => h.OwnerUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // HouseholdMember
        modelBuilder.Entity<HouseholdMember>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Role).IsRequired().HasMaxLength(32);
            entity.HasOne(m => m.Household)
                  .WithMany(h => h.Members)
                  .HasForeignKey(m => m.HouseholdId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(m => m.User)
                  .WithMany(u => u.HouseholdMemberships)
                  .HasForeignKey(m => m.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Item
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Name).IsRequired().HasMaxLength(256);
            entity.Property(i => i.Unit).IsRequired().HasMaxLength(16);
            entity.Property(i => i.Quantity).HasColumnType("decimal(18,4)");
            entity.Property(i => i.Price).HasColumnType("decimal(18,2)");
            entity.HasIndex(i => new { i.HouseholdId, i.ExpiryDate });
            entity.HasOne(i => i.Household)
                  .WithMany(h => h.Items)
                  .HasForeignKey(i => i.HouseholdId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Recipe
        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Title).IsRequired().HasMaxLength(256);
            entity.HasIndex(r => r.Title);
            entity.HasIndex(r => r.Tags);
            entity.HasOne(r => r.Household)
                  .WithMany(h => h.Recipes)
                  .HasForeignKey(r => r.HouseholdId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // RecipeIngredient
        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(ri => ri.Id);
            entity.Property(ri => ri.Name).IsRequired().HasMaxLength(256);
            entity.Property(ri => ri.Unit).IsRequired().HasMaxLength(16);
            entity.Property(ri => ri.Quantity).HasColumnType("decimal(18,4)");
            entity.HasOne(ri => ri.Recipe)
                  .WithMany(r => r.RecipeIngredients)
                  .HasForeignKey(ri => ri.RecipeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // GeneratedRecipe
        modelBuilder.Entity<GeneratedRecipe>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.HasOne(g => g.RequestedByUser)
                  .WithMany()
                  .HasForeignKey(g => g.RequestedBy)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(g => g.Household)
                  .WithMany()
                  .HasForeignKey(g => g.HouseholdId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.HasOne(n => n.Household)
                  .WithMany(h => h.Notifications)
                  .HasForeignKey(n => n.HouseholdId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // AnalyticsEvent
        modelBuilder.Entity<AnalyticsEvent>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.EventType).IsRequired().HasMaxLength(64);
            entity.HasOne(a => a.Household)
                  .WithMany(h => h.AnalyticsEvents)
                  .HasForeignKey(a => a.HouseholdId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // AdminSetting
        modelBuilder.Entity<AdminSetting>(entity =>
        {
            entity.HasKey(s => s.Key);
            entity.Property(s => s.Key).IsRequired().HasMaxLength(128);
            entity.Property(s => s.Value).IsRequired().HasMaxLength(512);
        });

        // MagicLinkToken
        modelBuilder.Entity<MagicLinkToken>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Email).IsRequired().HasMaxLength(256);
            entity.Property(t => t.Token).IsRequired().HasMaxLength(128);
            entity.HasIndex(t => t.Token).IsUnique();
        });
    }
}
