namespace StackManager.Extensions.Testing;

/// <summary>
/// A host with data that the test sets. Every property is a plain value that the test can change.
/// <see cref="ApplyChangesAsync" /> and <see cref="ControlServiceAsync" /> do not touch the machine. They record the call.
/// </summary>
public sealed class FakeHost : IStackHost
{
    public StackStatus Status { get; set; } = new(
    [
        new ServiceStatus(ServiceTarget.Apache, "Apache", "Running", [80, 443]),
        new ServiceStatus(ServiceTarget.MariaDb, "MariaDB", "Running", [3306])
    ],
    "2.4.71",
    "8.4.0",
    "11.8.0",
    @"C:\stack");

    public List<CheckMessage> Checks { get; } = [];

    public List<ConfigFileInfo> ConfigFiles { get; } = [];

    /// <summary>The text of a configuration file, by name.</summary>
    public Dictionary<string, string> ConfigText { get; } = [];

    public List<ModuleInfo> PhpExtensions { get; } = [];

    public List<ModuleInfo> ApacheModules { get; } = [];

    public List<LogFileInfo> Logs { get; } = [];

    /// <summary>The lines of a log, by name.</summary>
    public Dictionary<string, List<string>> LogLines { get; } = [];

    public List<UpdateInfo> Updates { get; } = [];

    /// <summary>The plans that <see cref="PlanChanges" /> made.</summary>
    public List<IReadOnlyList<TypedChange>> PlannedChanges { get; } = [];

    /// <summary>The plans that <see cref="ApplyChangesAsync" /> received.</summary>
    public List<ChangePlan> AppliedPlans { get; } = [];

    /// <summary>The calls of <see cref="ControlServiceAsync" />.</summary>
    public List<(ServiceTarget Target, ServiceAction Action)> ServiceCalls { get; } = [];

    /// <summary>The result of <see cref="ApplyChangesAsync" />. A test sets it to simulate a failed check.</summary>
    public ChangeResult ApplyResult { get; set; } = new(true, "The changes are applied.", []);

    public ServiceResult ServiceResult { get; set; } = new(true, "Done.");

    public Task<StackStatus> GetStatusAsync(CancellationToken token = default)
    {
        return Task.FromResult(Status);
    }

    public Task<IReadOnlyList<CheckMessage>> RunChecksAsync(CancellationToken token = default)
    {
        return Task.FromResult<IReadOnlyList<CheckMessage>>(Checks.ToList());
    }

    public IReadOnlyList<ConfigFileInfo> ListConfigFiles()
    {
        return ConfigFiles.ToList();
    }

    public string ReadConfig(string name)
    {
        return ConfigText.TryGetValue(name, out var text) ? text : throw new KeyNotFoundException($"No configuration file has the name {name}.");
    }

    public Task<IReadOnlyList<ModuleInfo>> ListPhpExtensionsAsync(CancellationToken token = default)
    {
        return Task.FromResult<IReadOnlyList<ModuleInfo>>(PhpExtensions.ToList());
    }

    public Task<IReadOnlyList<ModuleInfo>> ListApacheModulesAsync(CancellationToken token = default)
    {
        return Task.FromResult<IReadOnlyList<ModuleInfo>>(ApacheModules.ToList());
    }

    public IReadOnlyList<LogFileInfo> ListLogs()
    {
        return Logs.ToList();
    }

    public IReadOnlyList<string> TailLog(string name, int lines)
    {
        return LogLines.TryGetValue(name, out var all) ? all.TakeLast(lines).ToList() : throw new KeyNotFoundException($"No log has the name {name}.");
    }

    public ChangePlan PlanChanges(IReadOnlyList<TypedChange> changes)
    {
        PlannedChanges.Add(changes);
        var diff = string.Join(Environment.NewLine, changes.Select(change => "+ " + change));
        return new ChangePlan([new PlannedChange("fake", "fake", string.Empty)], diff, []);
    }

    public Task<ChangeResult> ApplyChangesAsync(ChangePlan plan, CancellationToken token = default)
    {
        AppliedPlans.Add(plan);
        return Task.FromResult(ApplyResult);
    }

    public Task<ServiceResult> ControlServiceAsync(ServiceTarget target, ServiceAction action, CancellationToken token = default)
    {
        ServiceCalls.Add((target, action));
        return Task.FromResult(ServiceResult);
    }

    public Task<IReadOnlyList<UpdateInfo>> CheckUpdatesAsync(CancellationToken token = default)
    {
        return Task.FromResult<IReadOnlyList<UpdateInfo>>(Updates.ToList());
    }
}
