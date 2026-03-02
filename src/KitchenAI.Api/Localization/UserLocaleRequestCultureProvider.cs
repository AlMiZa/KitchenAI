using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Localization;

namespace KitchenAI.Api.Localization;

/// <summary>
/// Determines request culture from the authenticated user's <c>locale</c> JWT claim.
/// Falls back to the next provider when the claim is absent or invalid.
/// </summary>
public sealed class UserLocaleRequestCultureProvider : RequestCultureProvider
{
    /// <inheritdoc/>
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var locale = httpContext.User.FindFirstValue("locale");
        if (string.IsNullOrWhiteSpace(locale) || !IsSupportedCulture(locale))
        {
            return NullProviderCultureResult;
        }

        return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(locale));
    }

    private static bool IsSupportedCulture(string locale)
    {
        try
        {
            _ = CultureInfo.GetCultureInfo(locale);
            return true;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }
}
