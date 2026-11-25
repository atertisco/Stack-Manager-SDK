namespace StackManager.Extensions.Testing;

/// <summary>Keeps what the extension adds to the application: the settings fields, the buttons and the status line.</summary>
public sealed class FakeRegistry : IExtensionRegistry
{
    public List<SettingField> Fields { get; } = [];

    public List<ExtensionCommand> Commands { get; } = [];

    public string StatusLine { get; private set; } = string.Empty;

    public bool StatusIsError { get; private set; }

    public void AddSettings(IReadOnlyList<SettingField> fields)
    {
        Fields.AddRange(fields);
    }

    public void AddCommand(ExtensionCommand command)
    {
        Commands.Add(command);
    }

    public void SetStatus(string text, bool isError = false)
    {
        StatusLine = text;
        StatusIsError = isError;
    }

    /// <summary>Runs the button with this id, as a click does.</summary>
    public Task<CommandResult> RunAsync(string id, CancellationToken token = default)
    {
        var command = Commands.FirstOrDefault(item => item.Id == id) ?? throw new KeyNotFoundException($"No command has the id {id}.");
        return command.Run(token);
    }
}
