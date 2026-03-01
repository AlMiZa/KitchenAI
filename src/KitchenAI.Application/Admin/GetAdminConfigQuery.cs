using MediatR;

namespace KitchenAI.Application.Admin;

/// <summary>Returns the current admin configuration status.</summary>
public record GetAdminConfigQuery : IRequest<AdminConfigDto>;
