# AGENTS.md — Unity-AI-Animation

Unity package `com.ivanmurzak.unity.mcp.animation` — makes Unity `AnimationClip` / `AnimatorController` assets and the
`Animator` component available to Unity-MCP's RQL tools (`rql-query` / `rql-mutate`; no extension MCP
tools): the `animation` source (CreateClip, CreateController, animators), clip curve / key / event / settings views and
methods, controller layer / parameter views and state-machine methods, transition conditions. An extension of
[AI Game Developer](https://github.com/IvanMurzak/Unity-MCP) (`com.ivanmurzak.unity.mcp`), generated from
`Unity-AI-Tools-Template`; the former tool → RQL table is in `README.md`.

## Layout

- `Unity-Package/` — Unity project hosting the package; only `Packages/com.ivanmurzak.unity.mcp.animation/`
  ships.
  - `package.json` — the version source of truth and the dependency pins.
  - `Editor/Scripts/Rql/` — the RQL module (`AnimationModule`: source, adapters, guide), one adapter per file
    (`<Namespace_Type>_Adapter.cs`), the `AnimationEditorApi` source object, the views (`AnimationCurveView`,
    `KeyframeView`, `ObjectCurveView`, `AnimationEventView`, `AnimationClipSettingsView`, `AnimatorLayerView`,
    `AnimatorParameterView`) and shared helpers (`AnimationEdit`).
  - `Runtime/` — runtime assembly (currently empty).
  - `Tests/Editor/Rql/` (EditMode, `AnimationTestBase` + one file per adapter / view / source) and `Tests/Runtime/`
    (PlayMode).
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

- **RQL surface.** Everything is registered by `AnimationModule` (`IRqlModule`, Order 100, found through TypeCache) and
  documented in its `AddGuide("Animation", …)` text, which is rendered into the `unity-rql` skill: keep the guide examples
  runnable (`AnimationModuleTests.EveryGuideExampleRuns` executes them in order).
- **Copies.** Unity returns copies for clip curves (`AnimationUtility.GetEditorCurve`), events, clip settings, and the
  controller's `layers` / `parameters` arrays. Never expose them raw (writes would be lost): the views re-read on every get
  and write the whole value back on every set.
- **Undo.** Calls on a clip / controller / transition are recorded by the engine (the asset is the owner). View setters
  have a plain owner, so they record their asset themselves (`AnimationEdit.Edit`). Creating an asset is not undoable.
- **Animator.** Most `Animator` properties describe the playing graph and log when read in Edit Mode: the adapter keeps
  only an allowlist of configuration members (`UnityEngine_Animator_Adapter.Safe`).
- **Name clash.** Inside `com.IvanMurzak.Unity.MCP.*` namespaces short names may resolve to namespaces; alias Unity types
  inside the namespace block (`using AnimatorController = UnityEditor.Animations.AnimatorController;`).
- Every C# file starts with the copyright box header and `#nullable enable`.
