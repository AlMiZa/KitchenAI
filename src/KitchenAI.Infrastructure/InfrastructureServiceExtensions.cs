using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KitchenAI.Infrastructure;

/// <summary>Registers infrastructure services (EF Core, repositories, etc.) with the DI container.</summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>Adds the SQLite-backed <see cref="AppDbContext"/> and applies pending migrations on startup.</summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=kitchenai.db";

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        return services;
    }
}
