namespace StackManager.Extensions.Testing;

/// <summary>
/// A context for a test. It has a fake host, a fake request queue and settings in memory.
/// Give it to <see cref="IExtension.Start" /> and look at what the extension registered.
/// </summary>
public sealed class FakeExtensionContext : IExtensionContext, IDisposable
{
    public FakeExtensionContext(string id = "test-extension")
    {
        Id = id;
        Folder = Path.Combine(Path.GetTempPath(), "smx-test-" + Guid.NewGuid().ToString("N"));
        DataFolder = Path.Combine(Folder, "data");
        Directory.CreateDirectory(DataFolder);
    }

    public string Id { get; }

    public string Folder { get; }

    public string DataFolder { get; }

    public string StackRoot { get; set; } = @"C:\stack";

    public FakeHost Host { get; } = new();

    public FakeRequestQueue Requests { get; } = new();

    public FakeSettings Settings { get; } = new();

    public FakeLog Log { get; } = new();

    public FakeRegistry Registry { get; } = new();

    IStackHost IExtensionContext.Host => Host;

    IRequestQueue IExtensionContext.Requests => Requests;

    IExtensionSettings IExtensionContext.Settings => Settings;

    IExtensionLog IExtensionContext.Log => Log;

    IExtensionRegistry IExtensionContext.Registry => Registry;

    public void Dispose()
    {
        if (Directory.Exists(Folder))
        {
            Directory.Delete(Folder, true);
        }
    }
}
