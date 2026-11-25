# Builds the SDK kit for developers: dist\StackManager-Extensions-SDK-<version>.zip
# Run it from sdk\kit. The documentation, template, samples and README are in this folder.
# The DLLs and the pack tool are built from src.
param(
    [string]$Output = (Join-Path $PSScriptRoot "..\dist")
)

$ErrorActionPreference = "Stop"
$sdk = Resolve-Path (Join-Path $PSScriptRoot "..")
$sdkProject = Join-Path $sdk "src\StackManager.Extensions.Sdk\StackManager.Extensions.Sdk.csproj"
$version = ([xml](Get-Content $sdkProject)).Project.PropertyGroup.Version | Where-Object { $_ } | Select-Object -First 1
$name = "StackManager-Extensions-SDK-$version"
$work = Join-Path ([IO.Path]::GetTempPath()) ("smxkit-" + [Guid]::NewGuid().ToString("N"))
$kit = Join-Path $work $name
$lib = Join-Path $kit "lib"
$tool = Join-Path $kit "tools\smx"

New-Item -ItemType Directory -Force $lib, $tool | Out-Null

foreach ($project in "Sdk", "Packaging", "Testing") {
    $folder = Join-Path $sdk "src\StackManager.Extensions.$project"
    dotnet build $folder -c Release
    if ($LASTEXITCODE -ne 0) { throw "Building $project failed." }
    $built = Join-Path $folder "bin\Release\net10.0"
    Copy-Item (Join-Path $built "StackManager.Extensions.$project.dll"), (Join-Path $built "StackManager.Extensions.$project.xml") $lib
}

dotnet publish (Join-Path $sdk "src\StackManager.Extensions.Pack") -c Release -o $tool
if ($LASTEXITCODE -ne 0) { throw "Publishing the pack tool failed." }

Get-ChildItem $tool -Include *.pdb, *.xml -Recurse | Remove-Item -Force

if (-not (Test-Path (Join-Path $tool "smx.exe"))) {
    [IO.File]::WriteAllText((Join-Path $tool "smx.cmd"), "@echo off`r`ndotnet `"%~dp0smx.dll`" %*`r`n")
}

foreach ($item in "docs", "templates", "samples", "README.txt") {
    Copy-Item (Join-Path $PSScriptRoot $item) $kit -Recurse
}

Copy-Item (Join-Path $sdk "LICENSE") (Join-Path $kit "LICENSE.txt")

# The sample projects may have been built here. Leave the build folders out of the kit.
Get-ChildItem $kit -Recurse -Directory | Where-Object { $_.Name -in "bin", "obj", "dist" } | Remove-Item -Recurse -Force

New-Item -ItemType Directory -Force $Output | Out-Null
$zip = Join-Path (Resolve-Path $Output) "$name.zip"
if (Test-Path $zip) { Remove-Item $zip }
Add-Type -AssemblyName System.IO.Compression.FileSystem
[IO.Compression.ZipFile]::CreateFromDirectory($work, $zip)
Remove-Item -Recurse -Force $work
Write-Host "Kit: $zip"
