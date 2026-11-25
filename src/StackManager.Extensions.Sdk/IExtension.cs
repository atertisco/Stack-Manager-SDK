namespace StackManager.Extensions;

/// <summary>The entry point of an extension. The host creates one instance and calls Start, then Stop.</summary>
public interface IExtension
{
    void Start(IExtensionContext context);

    void Stop();
}
