namespace KitchenAI.Application.Admin;

/// <summary>Current admin configuration state (never exposes secret values).</summary>
public record AdminConfigDto(
    bool GeminiApiKeyConfigured,
    string LlmModel,
    string Version);
