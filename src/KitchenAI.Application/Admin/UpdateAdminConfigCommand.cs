using MediatR;

namespace KitchenAI.Application.Admin;

/// <summary>Persists mutable admin configuration to the database (e.g. LLM model selection).</summary>
public record UpdateAdminConfigCommand(bool? GeminiApiKeyPresent, string? LlmModel) : IRequest<AdminConfigDto>;
