param(
    [ValidateSet("run", "build", "test", "install")]
    [string]$Command = "run",
    [string]$DreamSource = "../dream",
    [ValidateSet("frontend", "backend")]
    [string]$Service = "frontend",
    [int]$Port = 8791,
    [int]$ApiPort = 8792,
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
    if ($Command -eq "test") {
        & $dreamer test -p common --node
        if ($LASTEXITCODE -ne 0) { throw "Shared tests failed" }
        & $env:DREAM_BIN test be/src/backend_tests.dream
        if ($LASTEXITCODE -ne 0) { throw "Backend tests failed" }
        return
    }
    if ($Command -eq "run" -or $Command -eq "build") {
        $package = if ($Service -eq "backend") { "singularity_api" } else { "singularity" }
        $arguments += @("-p", $package)
        if ($Release) { $arguments += "--release" }
    }
    if ($Command -eq "run" -and $Service -eq "frontend") { $arguments += @("--port", "$Port") }
    if ($Service -eq "backend") { $env:SINGULARITY_PORT = "$ApiPort" }
    & $dreamer @arguments
    if ($LASTEXITCODE -ne 0) { throw "dreamer $Command failed (exit $LASTEXITCODE)" }
} finally {
    Pop-Location
}
