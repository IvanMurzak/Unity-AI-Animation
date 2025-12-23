#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Updates com.ivanmurzak.unity.mcp package to the latest version

.DESCRIPTION
    Fetches the latest version from GitHub releases and updates the dependency
    version in package.json and manifest.json files.

.PARAMETER WhatIf
    Preview changes without applying them

.EXAMPLE
    .\update-ai-game-developer.ps1

.EXAMPLE
    .\update-ai-game-developer.ps1 -WhatIf
#>

param(
    [switch]$WhatIf
)

# Set location to repository root (parent of commands folder)
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptDir
Push-Location $repoRoot

# Script configuration
$ErrorActionPreference = "Stop"
$PackageName = "com.ivanmurzak.unity.mcp"
$GitHubRepo = "IvanMurzak/Unity-MCP"

# Files to update
$TargetFiles = @(
    "Unity-Package/Assets/root/package.json",
    "Unity-Package/Packages/manifest.json"
)

function Write-ColorText {
    param([string]$Text, [string]$Color = "White")
    Write-Host $Text -ForegroundColor $Color
}

function Get-LatestVersionFromGitHub {
    param([string]$Repo)

    try {
        # Try to get the latest release first (most reliable)
        $releaseUrl = "https://api.github.com/repos/$Repo/releases/latest"
        $headers = @{ "User-Agent" = "PowerShell" }

        try {
            $release = Invoke-RestMethod -Uri $releaseUrl -Headers $headers -TimeoutSec 30
            $tagName = $release.tag_name
            Write-ColorText "   Found latest release: $tagName" "Gray"
        }
        catch {
            # Fallback to tags if no releases exist
            Write-ColorText "   No releases found, checking tags..." "Gray"
            $tagsUrl = "https://api.github.com/repos/$Repo/tags"
            $tags = Invoke-RestMethod -Uri $tagsUrl -Headers $headers -TimeoutSec 30

            if ($tags.Count -eq 0) {
                throw "No tags found in repository"
            }

            $tagName = $tags[0].name
            Write-ColorText "   Found latest tag: $tagName" "Gray"
        }

        # Remove 'v' prefix if present (e.g., v1.0.0 -> 1.0.0)
        $version = $tagName -replace '^v', ''

        return $version
    }
    catch {
        throw "Failed to fetch version from GitHub: $($_.Exception.Message)"
    }
}

function Get-CurrentVersion {
    param([string]$FilePath, [string]$PackageName)

    if (-not (Test-Path $FilePath)) {
        return $null
    }

    $content = Get-Content $FilePath -Raw
    $pattern = [regex]::Escape("`"$PackageName`"") + ':\s*"([^"]+)"'

    if ($content -match $pattern) {
        return $Matches[1]
    }

    return $null
}

function Update-PackageVersion {
    param(
        [string]$FilePath,
        [string]$PackageName,
        [string]$NewVersion,
        [bool]$PreviewOnly = $false
    )

    if (-not (Test-Path $FilePath)) {
        Write-ColorText "   File not found: $FilePath" "Yellow"
        return $null
    }

    $content = Get-Content $FilePath -Raw
    $originalContent = $content

    # Pattern to match the package dependency line
    $pattern = '("' + [regex]::Escape($PackageName) + '":\s*")[^"]+"'
    $replacement = '${1}' + $NewVersion + '"'

    $newContent = $content -replace $pattern, $replacement

    if ($originalContent -eq $newContent) {
        Write-ColorText "   No changes needed in: $FilePath" "Gray"
        return $null
    }

    if (-not $PreviewOnly) {
        Set-Content -Path $FilePath -Value $newContent -NoNewline
    }

    return @{
        Path            = $FilePath
        OriginalContent = $originalContent
        NewContent      = $newContent
    }
}

# Main execution
try {
    Write-ColorText "🔄 Update AI Game Developer Package" "Cyan"
    Write-ColorText "=====================================" "Cyan"

    # Get current version from first file
    $currentVersion = Get-CurrentVersion -FilePath $TargetFiles[0] -PackageName $PackageName
    if ($currentVersion) {
        Write-ColorText "📋 Current version: $currentVersion" "White"
    }
    else {
        Write-ColorText "📋 Current version: not found" "Yellow"
    }

    # Fetch latest version from GitHub
    Write-ColorText "`n🌐 Fetching latest version from GitHub..." "Cyan"
    $latestVersion = Get-LatestVersionFromGitHub -Repo $GitHubRepo
    Write-ColorText "📋 Latest version: $latestVersion" "White"

    if ($currentVersion -eq $latestVersion) {
        Write-ColorText "`n✅ Already up to date!" "Green"
        Pop-Location
        exit 0
    }

    Write-ColorText "`n🔍 Updating files..." "Cyan"

    $updatedFiles = @()
    foreach ($file in $TargetFiles) {
        Write-ColorText "   Processing: $file" "Gray"
        $result = Update-PackageVersion -FilePath $file -PackageName $PackageName -NewVersion $latestVersion -PreviewOnly $WhatIf
        if ($result) {
            $updatedFiles += $result
            Write-ColorText "   ✓ Updated: $file" "Green"
        }
    }

    if ($WhatIf) {
        Write-ColorText "`n📋 Preview Summary:" "Cyan"
        Write-ColorText "   Files to update: $($updatedFiles.Count)" "White"
        Write-ColorText "   Version change: $currentVersion → $latestVersion" "White"
        Write-ColorText "`n✅ Preview completed. Run without -WhatIf to apply changes." "Green"
    }
    else {
        if ($updatedFiles.Count -gt 0) {
            Write-ColorText "`n🎉 Update completed successfully!" "Green"
            Write-ColorText "   Updated $($updatedFiles.Count) file(s)" "White"
            Write-ColorText "   Version: $currentVersion → $latestVersion" "White"
            Write-ColorText "`n💡 Remember to commit these changes to git" "Cyan"
        }
        else {
            Write-ColorText "`n⚠️  No files were updated" "Yellow"
        }
    }

    Pop-Location
}
catch {
    Write-ColorText "`n❌ Script failed: $($_.Exception.Message)" "Red"
    Pop-Location
    exit 1
}
