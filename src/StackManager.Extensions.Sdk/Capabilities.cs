namespace StackManager.Extensions;

/// <summary>Names of the rights that an extension can declare. The host shows them to the user. The host does not enforce them.</summary>
public static class Capabilities
{
    public const string ReadStatus = "read.status";

    public const string ReadConfig = "read.config";

    public const string ReadLogs = "read.logs";

    public const string WriteConfig = "write.config";

    public const string ServiceControl = "service.control";

    public const string UpdatePrepare = "update.prepare";

    public const string UpdateApply = "update.apply";

    public static IReadOnlyList<CapabilityInfo> All { get; } =
    [
        new(ReadStatus, "Read the state of the services, the versions, the ports and the checks.", "Low"),
        new(ReadConfig, "Read the configuration files. Password values are removed.", "Low"),
        new(ReadLogs, "Read the end of the Apache, PHP and MariaDB logs.", "Medium"),
        new(WriteConfig, "Change configuration files.", "Medium"),
        new(ServiceControl, "Start, stop, restart or reload the Apache and MariaDB services.", "Medium"),
        new(UpdatePrepare, "Check for, download and stage updates.", "Medium"),
        new(UpdateApply, "Apply a staged update.", "High")
    ];

    public static CapabilityInfo? Find(string name)
    {
        return All.FirstOrDefault(item => item.Name == name);
    }
}

public sealed record CapabilityInfo(string Name, string Description, string Risk);
