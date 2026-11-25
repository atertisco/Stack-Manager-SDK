# Stack Manager Extensions SDK

Build extensions for [Stack Manager](https://stackmanager.synsoft.eu/), the Windows manager for a local Apache, PHP and MariaDB stack. An extension is a .NET library that Stack Manager loads. It can add settings, buttons and a status page, read the state of the stack, and ask for changes that the user approves.

> **Status: preview, version 0.1.** The API can change before 1.0.

## What you get

| Part | What it is |
|------|------------|
| `StackManager.Extensions.Sdk.dll` | The API: `IExtension`, `IExtensionContext`, `IStackHost`, `IRequestQueue` and the other types an extension uses. |
| `StackManager.Extensions.Packaging.dll` | Makes and checks `.smx` packages. For a build program or a build server. |
| `StackManager.Extensions.Testing.dll` | A fake host and a fake context, to test an extension without Stack Manager. |
| `smx` | The command line tool: `smx pack` and `smx validate`. |
| Template | `dotnet new smx-extension` creates a ready project. |
| Sample | `Hello` is a small extension with tests. |
| Documentation | Eight Word documents, from the first extension to the limits of the API. |

The SDK is released on its own, with its own version. It does not need the source code of Stack Manager.

## Quick start

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download) and Stack Manager.

1. Download the kit `StackManager-Extensions-SDK-<version>.zip` from the [releases](../../releases) and unzip it, for example to `C:\StackManagerSdk`.
2. Set up the template and the pack tool:

   ```
   setx STACKMANAGER_SDK C:\StackManagerSdk
   dotnet new install C:\StackManagerSdk\templates\extension
   ```

3. Open a new console and create an extension:

   ```
   set PATH=%PATH%;C:\StackManagerSdk\tools\smx
   dotnet new smx-extension -n MyLogTools --publisher "Your name"
   cd MyLogTools
   powershell -ExecutionPolicy Bypass -File .\build.ps1
   ```

4. The package is `dist\mylogtools-1.0.0.0.smx`. In Stack Manager open **Extensions**, select **Install from file**, check the review window, and enable the extension.

The full walk-through is in `docs/01-getting-started.docx` in the kit.

## A minimal extension

```csharp
using StackManager.Extensions;

namespace MyLogTools;

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
    }

    public void Stop() => context?.Log.Info("Stopped.");

    private async Task<CommandResult> ShowStatusAsync(CancellationToken token)
    {
        var greeting = context!.Settings.Get("greeting", "Hello");
        var status = await context.Host.GetStatusAsync(token);
        return new CommandResult($"{greeting}! Apache {status.ApacheVersion}, PHP {status.PhpVersion}.");
    }
}
```

The extension has a manifest, `extension.json`, next to the library. The template creates both.

## How it works

- An extension runs **inside Stack Manager with full trust** and the rights of the user, which can be administrator. The user sees the publisher and the declared rights before the install. Declared rights are information. They are not enforced.
- An extension **cannot change the stack by itself**. It prepares a typed change (a `php.ini` value, an Apache directive or module, a virtual host, a MariaDB option, a service action) and sends it as a request. The user approves it on the Requests page. Each request and result is written to an audit log.
- A package, `.smx`, is a zip file with a manifest and a SHA-256 checksum for every file. Stack Manager refuses a package with a file that is changed, missing or not listed, and checks the files again at each start.
- Extensions are loaded from memory, so installed files are not locked and an update can replace them.

The security model is in `docs/04-security-and-trust.docx`.

## Documentation

The documents are in `kit/docs` in this repository and in `docs` in the kit.

| Document | Content |
|----------|---------|
| `01-getting-started.docx` | From an empty folder to a running extension. |
| `02-manifest-and-packaging.docx` | The fields of `extension.json`, the package, `smx`, versions. |
| `03-api-reference.docx` | Every interface and type, with examples. |
| `04-security-and-trust.docx` | The trust model and the rules for developers. |
| `05-testing.docx` | Testing without Stack Manager. |
| `06-install-and-debug.docx` | Install, states, logs, debugging, update and remove. |
| `07-mcp-extension.docx` | The MCP server extension that ships with Stack Manager. |
| `08-compatibility-and-limits.docx` | Versions, limits, behavior to know, change log, license. |

## Build from source

```
dotnet build StackManager.Sdk.slnx
dotnet pack src\StackManager.Extensions.Sdk -c Release -o dist\nuget
```

To make the kit zip, run `kit\build-kit.ps1`. It builds the DLLs, publishes `smx`, adds the documentation, the template and the samples, and writes `dist\StackManager-Extensions-SDK-<version>.zip`.

```
src\    The projects: Sdk, Packaging, Testing and Pack (the smx tool)
kit\    What goes into the kit: docs, templates, samples, README.txt, build-kit.ps1
dist\   The output. Not in git.
```

The NuGet packages `StackManager.Extensions.Sdk`, `StackManager.Extensions.Packaging` and `StackManager.Extensions.Testing` come from the same sources.

## Versions

The SDK has its own version number, in the `.csproj` files. An extension names the API version it was written for in `apiVersion` in its manifest. While the major version is 0, the minor version must be the same. Changes are in [CHANGELOG.md](CHANGELOG.md).

## License

BSD 3-Clause. See [LICENSE](LICENSE). Copyright (c) 2026, Synsoft Solutions.

The license covers the SDK only. Your extension is yours and can have any license. Stack Manager itself is not part of the SDK.
