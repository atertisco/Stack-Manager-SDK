using System.IO.Compression;

namespace StackManager.Extensions.Hosting;

/// <summary>Makes and unpacks .smx files. An .smx file is a zip file with extension.json in its root.</summary>
public static class ExtensionPackage
{
    public const string FileType = ".smx";

    /// <summary>The ending of the folder that holds a staged extension, before it is installed.</summary>
    public const string StagedSuffix = ".new";

    public static string StagedFolder(string root, ExtensionManifest manifest)
    {
        return Path.Combine(root, manifest.Id, manifest.Version + StagedSuffix);
    }

    /// <summary>
    /// Reads the manifest from the source folder, adds the file list with the checksums,
    /// and writes the package. The manifest in the source folder needs no file list.
    /// The .pdb files are left out, unless includeSymbols is true. A package with symbols lets a debugger stop in the extension.
    /// </summary>
    public static ExtensionManifest Pack(string sourceFolder, string outputFile, bool includeSymbols = false)
    {
        var template = ManifestTemplate(sourceFolder);

        var files = Directory.EnumerateFiles(sourceFolder, "*", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(sourceFolder, path).Replace('\\', '/'))
            .Where(path => path != ManifestReader.FileName && (includeSymbols || !path.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase)))
            .Order(StringComparer.OrdinalIgnoreCase)
            .Select(path => new ExtensionFile(path, ExtensionFiles.Hash(Path.Combine(sourceFolder, path))))
            .ToArray();

        var manifest = template with { Files = files };
        ManifestReader.Parse(ManifestReader.Write(manifest));

        if (File.Exists(outputFile))
        {
            File.Delete(outputFile);
        }

        using var archive = ZipFile.Open(outputFile, ZipArchiveMode.Create);
        var entry = archive.CreateEntry(ManifestReader.FileName, CompressionLevel.Optimal);

        using (var writer = new StreamWriter(entry.Open()))
        {
            writer.Write(ManifestReader.Write(manifest));
        }

        foreach (var file in files)
        {
            archive.CreateEntryFromFile(Path.Combine(sourceFolder, file.Path), file.Path, CompressionLevel.Optimal);
        }

        return manifest;
    }

    /// <summary>Unpacks the zip file into a .new folder and checks it. Nothing is active yet.</summary>
    public static StagedExtension Stage(string zipFile, string extensionsRoot)
    {
        var scratch = Path.Combine(extensionsRoot, ".staging", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(scratch);

        try
        {
            Unpack(zipFile, scratch);
            var manifest = ManifestReader.Read(scratch);
            ExtensionFiles.Verify(scratch, manifest);

            if (!ApiVersion.IsCompatible(manifest.ApiVersion))
            {
                throw new ExtensionException($"The extension was built for API {manifest.ApiVersion}. This application uses API {ApiVersion.Current}.");
            }

            var target = StagedFolder(extensionsRoot, manifest);

            if (Directory.Exists(target))
            {
                Directory.Delete(target, true);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            Directory.Move(scratch, target);
            return new StagedExtension(manifest, target);
        }
        catch
        {
            if (Directory.Exists(scratch))
            {
                Directory.Delete(scratch, true);
            }

            throw;
        }
    }

    private static ExtensionManifest ManifestTemplate(string folder)
    {
        var path = Path.Combine(folder, ManifestReader.FileName);

        if (!File.Exists(path))
        {
            throw new ExtensionException($"The file {ManifestReader.FileName} is missing in the source folder.");
        }

        var template = System.Text.Json.JsonSerializer.Deserialize<ExtensionManifest>(File.ReadAllText(path), ManifestReader.Options);
        return template ?? throw new ExtensionException("The manifest is empty.");
    }

    private static void Unpack(string zipFile, string folder)
    {
        using var archive = ZipFile.OpenRead(zipFile);
        var full = Path.GetFullPath(folder) + Path.DirectorySeparatorChar;

        foreach (var entry in archive.Entries)
        {
            if (entry.FullName.EndsWith('/'))
            {
                continue;
            }

            var target = Path.GetFullPath(Path.Combine(folder, entry.FullName));

            if (!target.StartsWith(full, StringComparison.OrdinalIgnoreCase))
            {
                throw new ExtensionException($"The entry {entry.FullName} is outside the extension folder.");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            entry.ExtractToFile(target, true);
        }
    }
}

public sealed record StagedExtension(ExtensionManifest Manifest, string Folder)
{
    public string Signature => "Unsigned";
}
