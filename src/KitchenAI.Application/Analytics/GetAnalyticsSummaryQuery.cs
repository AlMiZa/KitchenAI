using MediatR;

namespace KitchenAI.Application.Analytics;

/// <summary>Returns an analytics summary for the specified household.</summary>
public record GetAnalyticsSummaryQuery(Guid HouseholdId) : IRequest<AnalyticsSummaryDto>;
