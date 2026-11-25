using Hello;
using StackManager.Extensions;
using StackManager.Extensions.Testing;

// A test needs no framework. This program checks the extension with the fake host and returns the number of failed checks.
// With xunit or NUnit, put each check in a test method and use the same fake objects.

var failures = 0;

void Check(string name, bool ok)
{
    Console.WriteLine((ok ? "PASS " : "FAIL ") + name);

    if (!ok)
        failures++;
}

using var context = new FakeExtensionContext("hello");
var instance = new HelloExtension();
instance.Start(context);

Check("the settings form has the greeting", context.Registry.Fields.Any(field => field.Key == "greeting"));
Check("the status button is added", context.Registry.Commands.Any(command => command.Id == "status"));
Check("the status line is set", context.Registry.StatusLine == "Ready.");

var result = await context.Registry.RunAsync("status");
Check("the message uses the default greeting and the versions", result.Message == "Hello! Apache 2.4.71, PHP 8.4.0.");
Check("the text lists the services", result.Text is not null && result.Text.Contains("Apache: Running"));

context.Settings.Set("greeting", "Hi");
result = await context.Registry.RunAsync("status");
Check("a changed setting is used", result.Message.StartsWith("Hi!"));

instance.Stop();
Check("the log has the start and the stop", context.Log.Lines.SequenceEqual(["info: Started.", "info: Stopped."]));

return failures;
