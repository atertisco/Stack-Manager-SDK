namespace StackManager.Extensions;

/// <summary>What the host gives to an extension when it starts.</summary>
public interface IExtensionContext
{
    string Id { get; }

    /// <summary>The folder with the files of the extension.</summary>
    string Folder { get; }

    /// <summary>A folder that stays when the extension is updated. The extension can write here.</summary>
    string DataFolder { get; }

    string StackRoot { get; }

    IStackHost Host { get; }

    IRequestQueue Requests { get; }

    IExtensionSettings Settings { get; }

    IExtensionLog Log { get; }

    IExtensionRegistry Registry { get; }
}

public interface IExtensionLog
{
    void Info(string text);

    void Warning(string text);

    void Error(string text);
}
