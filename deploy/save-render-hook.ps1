$ErrorActionPreference = "Stop"
$secureHook = Read-Host "Paste the Render Deploy Hook URL (input hidden)" -AsSecureString
$hook = [Net.NetworkCredential]::new("", $secureHook).Password
try {
    $uri = [Uri]$hook
    if ($uri.Scheme -ne "https" -or $uri.Host -ne "api.render.com" -or $uri.AbsolutePath -ne "/deploy/srv-dbgvge0u01pc73b5uh50" -or !$uri.Query.Contains("key=")) {
        throw "Expected the deploy hook for the existing Singularity Render service"
    }
    $hook | & gh secret set RENDER_DEPLOY_HOOK --repo sps014/Singularity
    if ($LASTEXITCODE -ne 0) { throw "Could not store the GitHub Actions secret" }
    Write-Host "RENDER_DEPLOY_HOOK is saved in GitHub Actions secrets."
} finally {
    $hook = $null
    $secureHook.Dispose()
}
