using KitchenAI.Application.Persistence;
using KitchenAI.Application.Services;
using KitchenAI.Infrastructure.BackgroundServices;
using KitchenAI.Infrastructure.Persistence;
using KitchenAI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KitchenAI.Infrastructure;

/// <summary>Registers infrastructure services (EF Core, repositories, etc.) with the DI container.</summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>Adds the SQLite-backed <see cref="AppDbContext"/> and all infrastructure services.</summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=kitchenai.db";

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        // Register AppDbContext as IAppDbContext so Application handlers can resolve it
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ILlmService, LlmService>();
        services.AddScoped<ExpiryNotificationJob>();
        services.AddHostedService<ExpiryNotificationService>();

        return services;
    }
}

