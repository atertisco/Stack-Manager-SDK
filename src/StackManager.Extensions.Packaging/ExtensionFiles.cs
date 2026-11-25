using System.Security.Cryptography;

namespace StackManager.Extensions.Hosting;

public static class ExtensionFiles
{
    public static string Hash(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
    }

    /// <summary>Checks each listed file and refuses a file that is not listed.</summary>
    public static void Verify(string folder, ExtensionManifest manifest)
    {
        var listed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var file in manifest.Files)
        {
            var normal = ManifestReader.Normal(file.Path);
            var path = Path.Combine(folder, normal.Replace('/', Path.DirectorySeparatorChar));
            listed.Add(normal);

            if (!File.Exists(path))
            {
                throw new ExtensionException($"The file {file.Path} is missing.");
            }

            if (!Hash(path).Equals(file.Sha256, StringComparison.OrdinalIgnoreCase))
            {
                throw new ExtensionException($"The file {file.Path} changed. Its checksum does not match.");
            }
        }

        foreach (var path in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(folder, path).Replace('\\', '/');

            if (relative != ManifestReader.FileName && !listed.Contains(relative))
            {
                throw new ExtensionException($"The file {relative} is not in the file list.");
            }
        }
    }
}
