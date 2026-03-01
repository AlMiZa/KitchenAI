using MediatR;

namespace KitchenAI.Application.Admin;

/// <summary>Updates mutable admin configuration (LLM model selection, etc.).</summary>
public record UpdateAdminConfigCommand(bool? GeminiApiKeyPresent, string? LlmModel) : IRequest<AdminConfigDto>;
