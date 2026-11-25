using StackManager.Extensions.Hosting;

if (args.Length < 2 || args[0] is not ("pack" or "validate"))
{
    Console.Error.WriteLine("Use: smx pack <source folder> <output.smx> [--symbols]");
    Console.Error.WriteLine("     smx validate <file.smx>");
    return 2;
}

try
{
    if (args[0] == "pack")
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("The output file is missing.");
            return 2;
        }

        var manifest = ExtensionPackage.Pack(args[1], args[2], args.Contains("--symbols"));
        Console.WriteLine($"{manifest.Id} {manifest.Version}: {manifest.Files.Length} files, {args[2]}");
        return 0;
    }

    var scratch = Path.Combine(Path.GetTempPath(), "smx-" + Guid.NewGuid().ToString("N"));

    try
    {
        var staged = ExtensionPackage.Stage(args[1], scratch);
        Console.WriteLine($"Valid: {staged.Manifest.Id} {staged.Manifest.Version}, API {staged.Manifest.ApiVersion}, {staged.Manifest.Files.Length} files.");
        return 0;
    }
    finally
    {
        if (Directory.Exists(scratch))
        {
            Directory.Delete(scratch, true);
        }
    }
}
catch (ExtensionException exception)
{
    Console.Error.WriteLine(exception.Message);
    return 1;
}
