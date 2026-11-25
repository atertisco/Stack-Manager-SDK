using System.Text.Json;
using System.Text.RegularExpressions;

namespace StackManager.Extensions.Hosting;

public static partial class ManifestReader
{
    public const string FileName = "extension.json";

    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public static ExtensionManifest Parse(string json)
    {
        ExtensionManifest? manifest;

        try
        {
            manifest = JsonSerializer.Deserialize<ExtensionManifest>(json, Options);
        }
        catch (JsonException exception)
        {
            throw new ExtensionException($"The manifest is not valid JSON: {exception.Message}");
        }

        return Check(manifest ?? throw new ExtensionException("The manifest is empty."));
    }

    public static ExtensionManifest Read(string folder)
    {
        var path = Path.Combine(folder, FileName);

        if (!File.Exists(path))
        {
            throw new ExtensionException($"The file {FileName} is missing.");
        }

        return Parse(File.ReadAllText(path));
    }

    public static string Write(ExtensionManifest manifest)
    {
        return JsonSerializer.Serialize(manifest, Options);
    }

    public static bool IsInside(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || Path.IsPathRooted(path) || path.Contains(':'))
        {
            return false;
        }

        return !path.Replace('\\', '/').Split('/').Any(part => part is ".." or "." or "");
    }

    private static ExtensionManifest Check(ExtensionManifest manifest)
    {
        Require(IdPattern().IsMatch(manifest.Id), "The id must use lower-case letters, digits and hyphens.");
        Require(!string.IsNullOrWhiteSpace(manifest.Name), "The name is missing.");
        Require(Version.TryParse(manifest.Version, out var version) && version.Revision >= 0 && version.Build >= 0, "The version must have four numbers, for example 1.0.0.0.");
        Require(!string.IsNullOrWhiteSpace(manifest.ApiVersion), "The apiVersion is missing.");
        Require(!string.IsNullOrWhiteSpace(manifest.Publisher), "The publisher is missing.");
        Require(IsInside(manifest.Entry) && manifest.Entry.EndsWith(".dll", StringComparison.OrdinalIgnoreCase), "The entry must be a DLL inside the extension folder.");

        foreach (var name in manifest.Capabilities)
        {
            Require(Capabilities.Find(name) is not null, $"The capability {name} is not known.");
        }

        Require(manifest.License.Type is "free" or "paid", "The license type must be free or paid.");
        Require(manifest.License.IsFree || !string.IsNullOrWhiteSpace(manifest.License.Product), "A paid extension needs a license product.");
        Require(manifest.Files.Length > 0, "The file list is empty.");

        foreach (var file in manifest.Files)
        {
            Require(IsInside(file.Path), $"The path {file.Path} is not inside the extension folder.");
            Require(file.Path != FileName, $"The file list must not contain {FileName}.");
            Require(HashPattern().IsMatch(file.Sha256), $"The checksum of {file.Path} is not valid.");
        }

        Require(manifest.Files.Select(file => Normal(file.Path)).Distinct(StringComparer.OrdinalIgnoreCase).Count() == manifest.Files.Length, "The file list has a path twice.");
        Require(manifest.Files.Any(file => Normal(file.Path).Equals(Normal(manifest.Entry), StringComparison.OrdinalIgnoreCase)), "The entry is not in the file list.");
        return manifest;
    }

    public static string Normal(string path)
    {
        return path.Replace('\\', '/');
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new ExtensionException(message);
        }
    }

    [GeneratedRegex("^[a-z0-9]+(-[a-z0-9]+)*$")]
    private static partial Regex IdPattern();

    [GeneratedRegex("^[0-9a-fA-F]{64}$")]
    private static partial Regex HashPattern();
}
