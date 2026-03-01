using MediatR;

namespace KitchenAI.Application.Admin;

/// <summary>Returns site-wide admin metrics.</summary>
public record GetAdminMetricsQuery : IRequest<AdminMetricsDto>;
