param(
    [ValidateSet("run", "build", "test", "install")]
    [string]$Command = "run",
    [string]$DreamSource = "../dream",
    [int]$Port = 8791,
    [switch]$Release
)
$ErrorActionPreference = "Stop"
$toolchain = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot $DreamSource)).Path
$env:DREAM_BIN = Join-Path $toolchain "target/release/dream.exe"
$dreamer = Join-Path $toolchain "target/release/dreamer.exe"
if (!(Test-Path -LiteralPath $env:DREAM_BIN) -or !(Test-Path -LiteralPath $dreamer)) {
    throw "Build the local tools first: cargo build --release -p dream -p dreamer (in $toolchain)"
}
Push-Location $PSScriptRoot
try {
    $arguments = @($Command)
    if ($Command -eq "run" -or $Command -eq "build") {
        $arguments += @("-p", "singularity")
        if ($Release) { $arguments += "--release" }
    }
    if ($Command -eq "run") { $arguments += @("--port", "$Port") }
    if ($Command -eq "test") { $arguments += "--node" }
    & $dreamer @arguments
    if ($LASTEXITCODE -ne 0) { throw "dreamer $Command failed (exit $LASTEXITCODE)" }
} finally {
    Pop-Location
}
