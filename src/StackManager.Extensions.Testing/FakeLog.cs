namespace StackManager.Extensions.Testing;

/// <summary>Keeps the log lines, so that a test can look at them.</summary>
public sealed class FakeLog : IExtensionLog
{
    public List<string> Lines { get; } = [];

    public void Info(string text)
    {
        Lines.Add("info: " + text);
    }

    public void Warning(string text)
    {
        Lines.Add("warning: " + text);
    }

    public void Error(string text)
    {
        Lines.Add("error: " + text);
    }
}
