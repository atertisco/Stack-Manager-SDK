namespace StackManager.Extensions;

/// <summary>
/// The services of the application. An extension uses them instead of its own code,
/// so that it gets the backup, check and restore steps of the application.
/// </summary>
public interface IStackHost
{
    Task<StackStatus> GetStatusAsync(CancellationToken token = default);

    Task<IReadOnlyList<CheckMessage>> RunChecksAsync(CancellationToken token = default);

    IReadOnlyList<ConfigFileInfo> ListConfigFiles();

    /// <summary>Reads one file by the name from <see cref="ListConfigFiles" />. Password values are replaced by ***.</summary>
    string ReadConfig(string name);

    Task<IReadOnlyList<ModuleInfo>> ListPhpExtensionsAsync(CancellationToken token = default);

    Task<IReadOnlyList<ModuleInfo>> ListApacheModulesAsync(CancellationToken token = default);

    IReadOnlyList<LogFileInfo> ListLogs();

    /// <summary>Reads the last lines of a log by the name from <see cref="ListLogs" />.</summary>
    IReadOnlyList<string> TailLog(string name, int lines);

    /// <summary>Makes a plan for the changes. It changes no file. The plan has the diff.</summary>
    ChangePlan PlanChanges(IReadOnlyList<TypedChange> changes);

    /// <summary>Makes a backup, writes the files, runs the checks and restores the backup if a check fails.</summary>
    Task<ChangeResult> ApplyChangesAsync(ChangePlan plan, CancellationToken token = default);

    Task<ServiceResult> ControlServiceAsync(ServiceTarget target, ServiceAction action, CancellationToken token = default);

    /// <summary>Checks for new versions of Apache, PHP and MariaDB. It downloads nothing.</summary>
    Task<IReadOnlyList<UpdateInfo>> CheckUpdatesAsync(CancellationToken token = default);
}

public enum ServiceTarget
{
    Apache,
    MariaDb
}

public enum ServiceAction
{
    Start,
    Stop,
    Restart,
    Reload
}

public sealed record StackStatus(IReadOnlyList<ServiceStatus> Services, string? ApacheVersion, string? PhpVersion, string? MariaDbVersion, string StackRoot);

public sealed record ServiceStatus(ServiceTarget Target, string Name, string State, IReadOnlyList<int> Ports);

public sealed record CheckMessage(string Severity, string Text);

public sealed record ConfigFileInfo(string Name, string Kind, string Path);

public sealed record ModuleInfo(string Name, string State);

public sealed record LogFileInfo(string Name, string Path, long Length);

public sealed record UpdateInfo(string Product, string Installed, string Available, string? Notice);

public sealed record ServiceResult(bool Succeeded, string Message);

public sealed record ChangeResult(bool Succeeded, string Message, IReadOnlyList<string> Files);

/// <summary>A prepared change. The host fills it. An extension does not build it.</summary>
public sealed record ChangePlan(IReadOnlyList<PlannedChange> Files, string Diff, IReadOnlyList<string> Problems)
{
    public bool IsEmpty => Files.Count == 0;

    public bool CanApply => Problems.Count == 0 && Files.Count > 0;
}

public sealed record PlannedChange(string Name, string Path, string NewText);
