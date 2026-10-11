# AGENTS.md — Unity-AI-Animation

Unity package `com.ivanmurzak.unity.mcp.animation` — MCP tools that let AI create, inspect and modify Unity
`AnimationClip` and `AnimatorController` assets. An extension of
[AI Game Developer](https://github.com/IvanMurzak/Unity-MCP) (`com.ivanmurzak.unity.mcp`), generated from
`Unity-AI-Tools-Template`; the user-facing tool list is in `README.md`.

## Layout

- `Unity-Package/` — Unity project hosting the package; only `Packages/com.ivanmurzak.unity.mcp.animation/`
  ships.
  - `package.json` — the version source of truth and the dependency pins.
  - `Editor/Scripts/Tools/` — the MCP tools (`AnimationTools` / `AnimatorTools`); new tools go here.
  - `Runtime/Data/` — serializable data contracts (modification and response types).
  - `Tests/Editor/` (EditMode tool tests + `Executors/`) and `Tests/Runtime/` (PlayMode).
- `Unity-Tests/<unity-version>/` — one test project per supported Unity version, consuming the package via
  `file:`. A new version must also be added to `test_pull_request.yml` and `release.yml`.
- `Installer/` — Unity project that exports the `.unitypackage` installer
  (`Assets/com.IvanMurzak/<name> Installer/`).
- `commands/` — PowerShell helpers (below) and `check-versions.py` (synced in by the release cascade — do not
  hand-edit; CI runs it as the `version-consistency` gate).
- `.github/workflows/` — `test_pull_request.yml` (PR test matrix), `test_unity_plugin.yml` (reusable test
  job), `release.yml`, `bump_version.yml`, `claude.yml` (`@claude` mentions).
- `docs/` — user docs: `Deploy-{OpenUPM,GitHub,npmjs}.md`, `Manual-Package-Rename.md`, `openupm-signing.md`,
  `img/`.

## Commands (PowerShell, from the repo root)

```powershell
./commands/bump-version.ps1 -NewVersion "x.y.z"   # package.json + Installer.cs in lock-step; -WhatIf to preview
./commands/get-version.ps1
./commands/update-ai-game-developer.ps1           # bump the com.ivanmurzak.unity.mcp pin; -WhatIf to preview
```

Tests run inside the Unity Editor (NUnit + `[UnityTest]`); CI runs them with `game-ci/unity-test-runner` in
EditMode / PlayMode / Standalone for every Unity version. `release.yml` releases on push to `main` only when
the `package.json` version has no tag yet (installer, signed UPM tarball for OpenUPM, test matrix, tag, GitHub
Release); dispatch `bump_version.yml` to bump in CI. Under `pull_request_target`, tests need the
maintainer-applied `ci-ok` label and abort if the PR touches `.github/workflows/`.

## Critical invariants

- **Main thread only.** Every Unity API call inside a tool MUST be wrapped in
  `MainThread.Instance.Run(() => { ... })` — MCP calls arrive off the main thread; only argument validation
  runs outside.
- **Tool shape.** Tools live in two `[AiToolType] static partial` classes, `AnimationTools` and
  `AnimatorTools` (namespace `com.IvanMurzak.Unity.MCP.Animation`), one operation per file
  (`Animation.Create.cs`, `Animation.GetData.cs`, `Animation.Modify.cs`, …). Each tool method carries
  `[AiTool(<id>, …)]`, `[AiSkillDescription]` / `[AiSkillBody]` and `[Description]` on the method and every
  parameter.
- **Modification contracts** (`Runtime/Data/`) are discriminated unions: `AnimationModification` +
  `ModificationType` (AnimationClip ops) and `AnimatorModification` + `AnimatorModificationType`
  (AnimatorController ops). Every field except `type` is nullable; the tool validates the fields each `type`
  requires at runtime.
- **Responses** carry a primary result field (e.g. `modifiedAsset`, `createdAssets`) plus a nullable
  `List<string>? errors` so a batch can partially succeed.
- **EntityId split.** Unity 6.5+ returns `UnityEngine.EntityId` from `GameObject.GetEntityId()`; pre-6.5
  returns `int` from `GetInstanceID()`. Files surfacing an instance ID ship as a `*.cs`
  (`#if UNITY_6000_5_OR_NEWER`) + `*.pre-Unity.6.5.cs` (`#if !UNITY_6000_5_OR_NEWER`) pair — e.g.
  `Animation.Create.cs` / `Animation.Create.pre-Unity.6.5.cs`. Keep both variants in sync.
- Every C# file starts with the copyright box header and `#nullable enable`.
