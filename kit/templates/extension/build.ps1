# Builds the extension and writes <id>-<version>.smx to the output folder.
# The smx tool is taken from the SMX environment variable, or from the PATH.
# With -Symbols the package has the .pdb files, so that a debugger can stop in the extension.
param(
    [string]$Output = (Join-Path $PSScriptRoot "dist"),
    [string]$Smx = $(if ($env:SMX) { $env:SMX } else { "smx" }),
    [switch]$Symbols
)

$ErrorActionPreference = "Stop"
$work = Join-Path ([IO.Path]::GetTempPath()) ("smx-" + [Guid]::NewGuid().ToString("N"))

dotnet publish $PSScriptRoot -c Release -o $work
if ($LASTEXITCODE -ne 0) { throw "The build failed." }

New-Item -ItemType Directory -Force $Output | Out-Null
$manifest = Get-Content (Join-Path $work "extension.json") | ConvertFrom-Json
$file = Join-Path $Output ($manifest.id + "-" + $manifest.version + ".smx")

$flags = if ($Symbols) { @("--symbols") } else { @() }
& $Smx pack $work $file @flags
if ($LASTEXITCODE -ne 0) { throw "Packing failed." }

& $Smx validate $file
if ($LASTEXITCODE -ne 0) { throw "The package is not valid." }

Remove-Item -Recurse -Force $work
