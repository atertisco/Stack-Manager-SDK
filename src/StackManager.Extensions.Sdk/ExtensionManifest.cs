using System.Text.Json.Serialization;

namespace StackManager.Extensions;

/// <summary>The content of extension.json.</summary>
public sealed record ExtensionManifest
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Version { get; init; } = string.Empty;

    public string ApiVersion { get; init; } = string.Empty;

    public string Publisher { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    /// <summary>The DLL that holds the class that implements <see cref="IExtension" />.</summary>
    public string Entry { get; init; } = string.Empty;

    public string[] Capabilities { get; init; } = [];

    public ExtensionLicense License { get; init; } = new();

    public ExtensionFile[] Files { get; init; } = [];
}

public sealed record ExtensionLicense
{
    /// <summary>"free" or "paid".</summary>
    public string Type { get; init; } = "free";

    public string? Product { get; init; }

    /// <summary>The public key of the developer. Only for a paid external extension.</summary>
    public string? PublicKey { get; init; }

    [JsonIgnore]
    public bool IsFree => Type == "free";
}

public sealed record ExtensionFile(string Path, string Sha256);
