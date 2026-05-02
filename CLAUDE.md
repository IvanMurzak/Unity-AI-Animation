# CLAUDE.md

## What this is

Unity package `com.ivanmurzak.unity.mcp.animation` providing MCP tools that let AI control Unity's `AnimationClip` and `AnimatorController` assets. Extension plugin for the [AI Game Developer](https://github.com/IvanMurzak/Unity-MCP) (`com.ivanmurzak.unity.mcp`) platform.

## Build / run

Run all PowerShell scripts from the repo root (they `Push-Location` internally):

```powershell
.\commands\bump-version.ps1 -NewVersion "1.0.35" -WhatIf   # preview bump
.\commands\bump-version.ps1 -NewVersion "1.0.35"           # apply bump
.\commands\get-version.ps1                                 # current version
.\commands\update-ai-game-developer.ps1 [-WhatIf]          # bump AI Game Developer dep
```

## Critical invariants

- **Main thread only.** All Unity Editor API calls must run on the main thread. Wrap every tool body in `MainThread.Instance.Run(() => { ... })`; only argument validation runs outside the lambda.
- Tools live in `static partial` classes (`AnimationTools`, `AnimatorTools`) split one operation per file.
- `#nullable enable` and the copyright header are required at the top of every C# file.

## Find detail in

- `docs/claude/architecture.md` — project layout, MCP tool registration, data contracts (`AnimationModification`/`AnimatorModification`), response pattern, package dependencies
- `docs/claude/release.md` — version management and `bump-version.ps1` propagation targets
- `docs/claude/ci.md` — release workflow, multi-version Unity test matrix, required secrets, `ci-ok` label gate
