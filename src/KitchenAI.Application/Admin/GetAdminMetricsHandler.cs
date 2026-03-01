using KitchenAI.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Admin;

/// <summary>Returns site-wide admin metrics by counting rows in the database.</summary>
public class GetAdminMetricsHandler(IAppDbContext db)
    : IRequestHandler<GetAdminMetricsQuery, AdminMetricsDto>
{
    /// <inheritdoc/>
    public async Task<AdminMetricsDto> Handle(GetAdminMetricsQuery request, CancellationToken cancellationToken)
    {
        var totalUsers = await db.Users.CountAsync(cancellationToken);
        var totalHouseholds = await db.Households.CountAsync(cancellationToken);
        var totalRecipesGenerated = await db.GeneratedRecipes.CountAsync(cancellationToken);
        var totalItemsTracked = await db.Items.CountAsync(cancellationToken);

        return new AdminMetricsDto(totalUsers, totalHouseholds, totalRecipesGenerated, totalItemsTracked);
    }
}
