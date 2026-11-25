namespace StackManager.Extensions;

/// <summary>A small store for the settings of one extension. The host keeps the values in a file.</summary>
public interface IExtensionSettings
{
    string? Get(string key);

    void Set(string key, string value);

    /// <summary>Raised after a value changed, from the settings form or from the extension.</summary>
    event Action<string>? Changed;
}

public static class ExtensionSettingsExtensions
{
    public static string Get(this IExtensionSettings settings, string key, string fallback)
    {
        return settings.Get(key) ?? fallback;
    }

    public static bool GetBool(this IExtensionSettings settings, string key, bool fallback = false)
    {
        return bool.TryParse(settings.Get(key), out var value) ? value : fallback;
    }

    public static int GetInt(this IExtensionSettings settings, string key, int fallback = 0)
    {
        return int.TryParse(settings.Get(key), out var value) ? value : fallback;
    }
}
