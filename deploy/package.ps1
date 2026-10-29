param(
    [string]$PackagesSource = "../../dream-packages",
    [string]$Archive = "../.verification/deploy/dream-0.0.17-linux-x64.tar.gz",
    [string]$OutputDirectory = "dist",
    [string]$DreamSource = "../dream"
)
$ErrorActionPreference = "Stop"
$root = Split-Path $PSScriptRoot -Parent
$packedCompiler = (Resolve-Path (Join-Path $PSScriptRoot $Archive)).Path
$expected = "c25604d2127ea2c1107128dbf27693969686c2e36c50e61f1b36069cdc468cee"
if ((Get-FileHash -LiteralPath $packedCompiler -Algorithm SHA256).Hash.ToLowerInvariant() -ne $expected) {
    throw "Dream v0.0.17 Linux x64 archive checksum mismatch"
}
$packages = (Resolve-Path (Join-Path $PSScriptRoot $PackagesSource)).Path
$staging = Join-Path $root (".verification/deploy/source-" + [Guid]::NewGuid().ToString("N"))
$output = [IO.Path]::GetFullPath((Join-Path $root $OutputDirectory))
if ((Test-Path -LiteralPath $output) -and (Get-ChildItem -LiteralPath $output -Force | Select-Object -First 1)) {
    throw "dist already exists. Move it aside before creating a new release."
}
& (Join-Path $root "dev.ps1") build -Release -DreamSource $DreamSource
if ($LASTEXITCODE -ne 0) { throw "Frontend build failed" }
New-Item -ItemType Directory -Force "$staging/singularity", "$staging/dream-packages", $output | Out-Null
Copy-Item -LiteralPath "$root/dream.toml" -Destination "$staging/singularity/"
foreach ($member in @("fe", "be", "common")) {
    $destination = "$staging/singularity/$member"
    New-Item -ItemType Directory -Force $destination | Out-Null
    Copy-Item -LiteralPath "$root/$member/dream.toml", "$root/$member/src" -Destination $destination -Recurse
}
Copy-Item -LiteralPath "$root/fe/index.html", "$root/fe/bootstrap.js", "$root/fe/styles", "$root/fe/assets" -Destination "$staging/singularity/fe/" -Recurse
New-Item -ItemType Directory -Force "$staging/singularity/fe/target/web" | Out-Null
Copy-Item -LiteralPath "$root/fe/target/web/main.wasm", "$root/fe/target/web/main.web.runtime.js" -Destination "$staging/singularity/fe/target/web/"
foreach ($package in @("youtube_explode", "markup", "sha1")) {
    $destination = "$staging/dream-packages/$package"
    New-Item -ItemType Directory -Force $destination | Out-Null
    Copy-Item -LiteralPath "$packages/$package/dream.toml", "$packages/$package/src" -Destination $destination -Recurse
}
& docker run --rm --platform linux/amd64 `
    --mount "type=bind,source=$packedCompiler,target=/input/dream.tar.gz,readonly" `
    --mount "type=bind,source=$staging,target=/work" `
    --mount "type=bind,source=$output,target=/out" `
    --mount "type=bind,source=$PSScriptRoot/build-linux.sh,target=/build.sh,readonly" `
    ubuntu:24.04 sh /build.sh
if ($LASTEXITCODE -ne 0) { throw "Local Linux packaging failed" }
Copy-Item -LiteralPath "$PSScriptRoot/Dockerfile" -Destination "$output/Dockerfile"
Write-Host "Prebuilt backend is ready in $output"
