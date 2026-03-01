using KitchenAI.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace KitchenAI.Application.Admin;

/// <summary>
/// Returns the current admin configuration, reading <c>LlmModel</c> from the database first
/// and falling back to <c>IConfiguration</c> then the default <c>"gemini-1.5-flash"</c>.
/// </summary>
public class GetAdminConfigHandler(IAppDbContext db, IConfiguration configuration)
    : IRequestHandler<GetAdminConfigQuery, AdminConfigDto>
{
    private const string LlmModelKey = "LlmService:Model";
    private const string DefaultModel = "gemini-1.5-flash";

    /// <inheritdoc/>
    public async Task<AdminConfigDto> Handle(GetAdminConfigQuery request, CancellationToken cancellationToken)
    {
        var model = await db.AdminSettings
            .Where(s => s.Key == LlmModelKey)
            .Select(s => s.Value)
            .FirstOrDefaultAsync(cancellationToken)
            ?? configuration[LlmModelKey]
            ?? DefaultModel;

        return new AdminConfigDto(
            GeminiApiKeyConfigured: !string.IsNullOrWhiteSpace(configuration["LlmService:GeminiApiKey"]),
            LlmModel: model,
            Version: typeof(GetAdminConfigHandler).Assembly.GetName().Version?.ToString() ?? "1.0.0");
    }
}
