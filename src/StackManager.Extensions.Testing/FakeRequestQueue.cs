namespace StackManager.Extensions.Testing;

/// <summary>
/// A request queue for a test. A request waits until the test calls <see cref="ApproveAsync" /> or <see cref="Reject" />,
/// as the user does in the application. With autoApprove the request runs at once.
/// </summary>
public sealed class FakeRequestQueue : IRequestQueue
{
    private readonly Dictionary<string, Func<CancellationToken, Task<RequestOutcome>>> actions = [];
    private int next = 1;

    public List<PendingRequest> All { get; } = [];

    public PendingRequest Submit(string title, string detail, Func<CancellationToken, Task<RequestOutcome>> apply, bool autoApprove = false)
    {
        var request = new PendingRequest("r" + next++, "test", title, detail, DateTime.Now);
        All.Add(request);
        actions[request.Id] = apply;

        if (autoApprove)
        {
            ApproveAsync(request.Id).GetAwaiter().GetResult();
        }

        return request;
    }

    public PendingRequest? Get(string id)
    {
        return All.FirstOrDefault(item => item.Id == id);
    }

    public bool Cancel(string id)
    {
        var request = Get(id);

        if (request is not { State: RequestState.Pending })
        {
            return false;
        }

        request.State = RequestState.Cancelled;
        return true;
    }

    /// <summary>Approves the request and runs its action.</summary>
    public async Task ApproveAsync(string id, CancellationToken token = default)
    {
        var request = Get(id) ?? throw new KeyNotFoundException($"No request has the id {id}.");

        if (request.State != RequestState.Pending)
        {
            return;
        }

        request.State = RequestState.Approved;

        try
        {
            var outcome = await actions[id](token);
            request.State = outcome.Succeeded ? RequestState.Applied : RequestState.Failed;
            request.Message = outcome.Message;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            request.State = RequestState.Failed;
            request.Message = exception.Message;
        }
    }

    public void Reject(string id)
    {
        var request = Get(id) ?? throw new KeyNotFoundException($"No request has the id {id}.");

        if (request.State == RequestState.Pending)
        {
            request.State = RequestState.Rejected;
        }
    }
}
