namespace StackManager.Extensions.Testing;

/// <summary>Settings in memory. A value that the test sets before Start is what the extension reads.</summary>
public sealed class FakeSettings : IExtensionSettings
{
    private readonly Dictionary<string, string> values = new(StringComparer.Ordinal);

    public event Action<string>? Changed;

    public string? Get(string key)
    {
        return values.GetValueOrDefault(key);
    }

    /// <summary>Sets a value and raises <see cref="Changed" />, as the settings form of the application does.</summary>
    public void Set(string key, string value)
    {
        values[key] = value;
        Changed?.Invoke(key);
    }
}
