using StackManager.Extensions;

namespace MyExtension;

public sealed class Extension : IExtension
{
    private IExtensionContext? context;

    public void Start(IExtensionContext extensionContext)
    {
        context = extensionContext;

        extensionContext.Registry.AddSettings(
        [
            new SettingField("greeting", "Greeting", SettingKind.Text, "Hello", "The first word of the message.")
        ]);

        extensionContext.Registry.AddCommand(new ExtensionCommand("status", "Show the status", ShowStatusAsync));
        extensionContext.Registry.SetStatus("Ready.");
        extensionContext.Log.Info("Started.");
    }

    public void Stop()
    {
        context?.Log.Info("Stopped.");
    }

    private async Task<CommandResult> ShowStatusAsync(CancellationToken token)
    {
        var greeting = context!.Settings.Get("greeting", "Hello");
        var status = await context.Host.GetStatusAsync(token);
        var lines = status.Services.Select(service => $"{service.Name}: {service.State}");
        return new CommandResult($"{greeting}! Apache {status.ApacheVersion}, PHP {status.PhpVersion}.", string.Join(Environment.NewLine, lines));
    }
}
