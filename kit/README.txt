STACK MANAGER EXTENSIONS SDK, version 0.1.0 (preview)
=====================================================

Everything that you need to write an extension for Stack Manager, test it, pack it into an .smx
file and install it, without the source code of the application.

An extension is a .NET library that Stack Manager loads. It can add settings, buttons and a status
page, read the state of the stack, and ask for changes that the user approves.


WHAT IS IN THE KIT

  lib\                  The DLLs, each with an XML file for the editor:
                          StackManager.Extensions.Sdk.dll        The API of an extension.
                          StackManager.Extensions.Packaging.dll  Makes and checks .smx packages.
                          StackManager.Extensions.Testing.dll    A fake host for tests.
  tools\smx\            The pack tool: smx pack, smx validate.
  templates\extension\  A "dotnet new" template for a new extension.
  samples\              Hello (an extension) and Hello.Tests (its tests).
  docs\                 The documentation, as Word documents. Start with 01-getting-started.docx.
  LICENSE.txt           The license of the kit (BSD 3-Clause).


WHAT YOU NEED

  - The .NET 10 SDK.
  - Stack Manager, to install and try your extension.


FIVE MINUTES

Replace C:\StackManagerSdk with the folder where you unzipped the kit. Run in a console:

  setx STACKMANAGER_SDK C:\StackManagerSdk
  dotnet new install C:\StackManagerSdk\templates\extension

Open a new console, then:

  set PATH=%PATH%;C:\StackManagerSdk\tools\smx
  dotnet new smx-extension -n MyLogTools --publisher "Your name"
  cd MyLogTools
  powershell -ExecutionPolicy Bypass -File .\build.ps1

The package is dist\mylogtools-1.0.0.0.smx. In Stack Manager open Extensions, select
Install from file, select the package, and select Enable.


DOCUMENTATION (folder docs)

  01-getting-started.docx           From an empty folder to a running extension.
  02-manifest-and-packaging.docx    The fields of extension.json, the package, smx, versions.
  03-api-reference.docx             Every interface and type of the SDK, with examples.
  04-security-and-trust.docx        The trust model, what the user sees, rules for developers.
  05-testing.docx                   Testing without the application.
  06-install-and-debug.docx         Install, states, logs, debugging, update and remove.
  07-mcp-extension.docx             The MCP server extension: settings, tools, safety.
  08-compatibility-and-limits.docx  Versions, what is not built, behavior to know, change log.


LICENSE

The kit is free to use under the BSD 3-Clause license, see LICENSE.txt. If you pass on the DLLs of
the kit, in source or binary form, keep the copyright notice and the license text with them. Your
own extension is yours and can have any license.


STATUS

This is a private preview. The API can change before version 1.0.
