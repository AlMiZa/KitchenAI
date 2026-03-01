namespace KitchenAI.Domain.Entities;

/// <summary>Persists a mutable admin configuration key/value pair in the database.</summary>
public class AdminSetting
{
    /// <summary>Configuration key, e.g. <c>"LlmService:Model"</c>.</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Configuration value.</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>UTC timestamp of the last update.</summary>
    public DateTime UpdatedAt { get; set; }
}
