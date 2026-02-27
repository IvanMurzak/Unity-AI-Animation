# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Project Is

A Unity package (`com.ivanmurzak.unity.mcp.animation`, v1.0.34) that provides MCP tools for controlling Unity's AnimationClip and AnimatorController assets via AI. It is an extension plugin for the [AI Game Developer](https://github.com/IvanMurzak/Unity-MCP) (`com.ivanmurzak.unity.mcp`) platform.

## Project Layout

```
Unity-Package/Assets/root/     # The distributable Unity package source
  Editor/Scripts/Tools/        # MCP tool implementations (Editor-only, UnityEditor API)
  Runtime/Data/                # Serializable data contracts (modification types, response types)
  Tests/Editor/                # Unity EditMode tests
  Tests/Runtime/               # Unity PlayMode/Runtime tests
Unity-Tests/                   # Separate Unity projects used to run tests
  2022.3.62f3/
  2023.2.22f1/
  6000.3.1f1/
Installer/                     # Unity project that builds the .unitypackage installer
commands/                      # PowerShell dev scripts (version bumping, dependency updates)
.github/workflows/             # CI/CD: release, test on PR, reusable test workflow
```

## Architecture

### MCP Tool Registration

Tools use attributes from `com.IvanMurzak.McpPlugin`:
- `[McpPluginToolType]` on a partial class — declares the class as a tool container
- `[McpPluginTool(toolId, Title = "...")]` on a static method — declares an individual MCP tool
- `[Description("...")]` on parameters — descriptions exposed to the AI

`AnimationTools` and `AnimatorTools` are `static partial` classes split across four files each (`Animation.cs`, `Animation.Create.cs`, `Animation.GetData.cs`, `Animation.Modify.cs`).

### Main Thread Execution

All Unity Editor API calls must run on the main thread. Every tool body is wrapped in `MainThread.Instance.Run(() => { ... })`. Code outside that lambda (argument validation) runs on the calling thread.

### Data Contracts (Runtime/Data/)

Modification operations use discriminated-union-style data classes:
- `AnimationModification` + `ModificationType` enum — for AnimationClip operations (SetCurve, RemoveCurve, ClearCurves, SetFrameRate, SetWrapMode, SetLegacy, AddEvent, ClearEvents)
- `AnimatorModification` + `AnimatorModificationType` enum — for AnimatorController operations (AddParameter, RemoveParameter, AddLayer, RemoveLayer, AddState, RemoveState, SetDefaultState, AddTransition, RemoveTransition, AddAnyStateTransition, SetStateMotion, SetStateSpeed)

All fields other than `type` are nullable; the tool validates which fields are required for each `type` at runtime.

### Response Pattern

Tool response objects have:
- A primary result field (e.g., `modifiedAsset`, `createdAssets`)
- A nullable `List<string>? errors` for batch operations that partially succeed

## Development Commands

All scripts are in `commands/` and must be run from the repo root (they internally `Push-Location` to the root).

```powershell
# Preview a version bump without applying it
.\commands\bump-version.ps1 -NewVersion "1.0.35" -WhatIf

# Apply a version bump (updates package.json, Installer.cs, README.md)
.\commands\bump-version.ps1 -NewVersion "1.0.35"

# Get the current package version
.\commands\get-version.ps1

# Update the AI Game Developer dependency to latest GitHub release
.\commands\update-ai-game-developer.ps1

# Update dependency (preview only)
.\commands\update-ai-game-developer.ps1 -WhatIf
```

## Version Management

Version is the single source of truth from `Unity-Package/Assets/root/package.json` (`"version"` field). `bump-version.ps1` propagates it to:
- `Unity-Package/Assets/root/package.json`
- `Installer/Assets/AI Animation Installer/Installer.cs` (constant `Version`)
- Download URLs in both `README.md` and `Unity-Package/Assets/root/README.md`

## CI/CD

The release workflow (`.github/workflows/release.yml`) triggers on push to `main`:
1. Reads version from `package.json`
2. Skips release if the git tag already exists
3. Builds the `.unitypackage` installer using Unity 2022.3.62f3
4. Runs tests across Unity 2022.3.62f3, 2023.2.22f1, 6000.3.1f1 in editmode/playmode/standalone
5. Creates a GitHub release with the installer artifact
6. Publishes to OpenUPM

Required repository secrets: `UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD`.

Pull request tests require a maintainer to apply the `ci-ok` label before secrets are exposed to contributor code.

## Package Dependencies

- `com.ivanmurzak.unity.mcp` — AI Game Developer platform (provides `McpPluginTool`, `MainThread`, `AssetObjectRef`, editor utilities)
- `com.unity.modules.animation` — Unity's built-in animation module
- OpenUPM scoped registry for `com.ivanmurzak.*` and `org.nuget.*` packages
