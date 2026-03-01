using MediatR;
using Microsoft.Extensions.Configuration;

namespace KitchenAI.Application.Admin;

/// <summary>
/// Reflects an admin config update. In production, config changes require environment variables or
/// user-secrets; this handler acknowledges the request and returns the effective state.
/// </summary>
public class UpdateAdminConfigHandler(IConfiguration configuration)
    : IRequestHandler<UpdateAdminConfigCommand, AdminConfigDto>
{
    /// <inheritdoc/>
    public Task<AdminConfigDto> Handle(UpdateAdminConfigCommand request, CancellationToken cancellationToken)
    {
        // IConfiguration is read-only at runtime; actual changes require redeployment with updated env vars.
        var geminiConfigured = request.GeminiApiKeyPresent
            ?? !string.IsNullOrWhiteSpace(configuration["LlmService:GeminiApiKey"]);

        var model = request.LlmModel
            ?? configuration["LlmService:Model"]
            ?? "gemini-1.5-flash";

        var dto = new AdminConfigDto(
            GeminiApiKeyConfigured: geminiConfigured,
            LlmModel: model,
            Version: typeof(UpdateAdminConfigHandler).Assembly.GetName().Version?.ToString() ?? "1.0.0");

        return Task.FromResult(dto);
    }
}
