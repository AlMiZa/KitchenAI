using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace KitchenAI.Application;

/// <summary>Registers application services (MediatR handlers, etc.) with the DI container.</summary>
public static class ApplicationServiceExtensions
{
    /// <summary>Adds MediatR and all handlers defined in the Application assembly.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
