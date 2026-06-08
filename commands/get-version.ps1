$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptDir
$packageJsonPath = Join-Path $repoRoot "Unity-Package/Packages/com.ivanmurzak.unity.mcp.animation/package.json"
if (Test-Path $packageJsonPath) {
    $content = Get-Content $packageJsonPath -Raw
    if ($content -match '"version":\s*"([\d\.]+)"') {
        Write-Output $Matches[1]
        exit 0
    }
}
Write-Error "Could not find version in $packageJsonPath"
exit 1
