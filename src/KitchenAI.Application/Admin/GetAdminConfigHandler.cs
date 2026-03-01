using MediatR;
using Microsoft.Extensions.Configuration;

namespace KitchenAI.Application.Admin;

/// <summary>Returns the admin configuration status derived from application configuration.</summary>
public class GetAdminConfigHandler(IConfiguration configuration)
    : IRequestHandler<GetAdminConfigQuery, AdminConfigDto>
{
    /// <inheritdoc/>
    public Task<AdminConfigDto> Handle(GetAdminConfigQuery request, CancellationToken cancellationToken)
    {
        var dto = new AdminConfigDto(
            GeminiApiKeyConfigured: !string.IsNullOrWhiteSpace(configuration["LlmService:GeminiApiKey"]),
            LlmModel: configuration["LlmService:Model"] ?? "gemini-1.5-flash",
            Version: typeof(GetAdminConfigHandler).Assembly.GetName().Version?.ToString() ?? "1.0.0");

        return Task.FromResult(dto);
    }
}
