namespace StackManager.Extensions;

/// <summary>The version of the extension API. An extension must be built for the same version.</summary>
public static class ApiVersion
{
    public const string Current = "0.1";

    public static bool IsCompatible(string? built)
    {
        if (!Version.TryParse(built, out var other) || !Version.TryParse(Current, out var current))
        {
            return false;
        }

        return other.Major == current.Major && (current.Major > 0 || other.Minor == current.Minor);
    }
}
