namespace StackManager.Extensions;

/// <summary>The places where an extension adds its own parts to the application.</summary>
public interface IExtensionRegistry
{
    /// <summary>Adds the fields of the settings form of the extension.</summary>
    void AddSettings(IReadOnlyList<SettingField> fields);

    /// <summary>Adds a button on the Extensions page.</summary>
    void AddCommand(ExtensionCommand command);

    /// <summary>Sets a short line that the Extensions page shows under the name of the extension.</summary>
    void SetStatus(string text, bool isError = false);
}

public enum SettingKind
{
    Bool,
    Text,
    Number,
    Choice
}

public sealed record SettingField(string Key, string Label, SettingKind Kind, string Default, string? Help = null, IReadOnlyList<string>? Choices = null);

/// <summary>A button. The handler returns a result. If the result has text and CopyToClipboard is true, the host copies the text.</summary>
public sealed record ExtensionCommand(string Id, string Label, Func<CancellationToken, Task<CommandResult>> Run);

public sealed record CommandResult(string Message, string? Text = null, bool CopyToClipboard = false, bool IsError = false);
