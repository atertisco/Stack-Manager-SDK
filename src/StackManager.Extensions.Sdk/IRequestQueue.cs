namespace StackManager.Extensions;

/// <summary>
/// The list of changes that wait for the user. An extension submits a request.
/// The user approves or rejects it on the Extensions page. Only then does the host call Apply.
/// </summary>
public interface IRequestQueue
{
    /// <summary>
    /// Adds a request. If autoApprove is true, the host runs it at once.
    /// The extension decides this from a setting that the user controls.
    /// </summary>
    PendingRequest Submit(string title, string detail, Func<CancellationToken, Task<RequestOutcome>> apply, bool autoApprove = false);

    PendingRequest? Get(string id);

    bool Cancel(string id);
}

public enum RequestState
{
    Pending,
    Approved,
    Applied,
    Rejected,
    Failed,
    Expired,
    Cancelled
}

public sealed record RequestOutcome(bool Succeeded, string Message);

public sealed class PendingRequest(string id, string source, string title, string detail, DateTime created)
{
    public string Id { get; } = id;

    public string Source { get; } = source;

    public string Title { get; } = title;

    /// <summary>The diff or the command, as text. The host shows it in its own window.</summary>
    public string Detail { get; } = detail;

    public DateTime Created { get; } = created;

    public RequestState State { get; set; } = RequestState.Pending;

    public string? Message { get; set; }
}
