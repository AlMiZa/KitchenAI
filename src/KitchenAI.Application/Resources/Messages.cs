using System.Globalization;
using System.Resources;

namespace KitchenAI.Application.Resources;

/// <summary>
/// Provides access to localized application messages using the current UI culture.
/// Falls back to Polish (default) when a key is not present for the current culture.
/// </summary>
internal static class Messages
{
    private static readonly ResourceManager ResourceManager =
        new("KitchenAI.Application.Resources.Messages", typeof(Messages).Assembly);

    /// <summary>Returns the localized message for <paramref name="key"/> in the current UI culture.</summary>
    internal static string Get(string key) =>
        ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;

    /// <summary>Returns the localized and formatted message for <paramref name="key"/> in the current UI culture.</summary>
    internal static string Get(string key, params object[] args) =>
        string.Format(Get(key), args);
}
