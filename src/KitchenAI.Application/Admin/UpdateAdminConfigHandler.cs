using KitchenAI.Application.Persistence;
using KitchenAI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace KitchenAI.Application.Admin;

/// <summary>
/// Persists mutable admin configuration changes to the database.
/// <para>
/// <c>LlmModel</c> is upserted into <c>AdminSettings</c> under key <c>"LlmService:Model"</c>.
/// <c>GeminiApiKeyPresent</c> reflects a deployment-time secret stored in environment variables /
/// user-secrets; it cannot be changed via this endpoint and is therefore ignored.
/// </para>
/// </summary>
public class UpdateAdminConfigHandler(IAppDbContext db, IConfiguration configuration)
    : IRequestHandler<UpdateAdminConfigCommand, AdminConfigDto>
{
    private const string LlmModelKey = "LlmService:Model";
    private const string DefaultModel = "gemini-1.5-flash";

    /// <inheritdoc/>
    public async Task<AdminConfigDto> Handle(UpdateAdminConfigCommand request, CancellationToken cancellationToken)
    {
        if (request.LlmModel is not null)
        {
            var setting = await db.AdminSettings
                .FirstOrDefaultAsync(s => s.Key == LlmModelKey, cancellationToken);

            if (setting is null)
            {
                setting = new AdminSetting { Key = LlmModelKey };
                db.AdminSettings.Add(setting);
            }

            setting.Value = request.LlmModel;
            setting.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);
        }

        var effectiveModel = await db.AdminSettings
            .Where(s => s.Key == LlmModelKey)
            .Select(s => s.Value)
            .FirstOrDefaultAsync(cancellationToken)
            ?? configuration[LlmModelKey]
            ?? DefaultModel;

        return new AdminConfigDto(
            GeminiApiKeyConfigured: !string.IsNullOrWhiteSpace(configuration["LlmService:GeminiApiKey"]),
            LlmModel: effectiveModel,
            Version: typeof(UpdateAdminConfigHandler).Assembly.GetName().Version?.ToString() ?? "1.0.0");
    }
}
