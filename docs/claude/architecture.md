# Architecture

## Project Layout

```
Unity-Package/Packages/com.ivanmurzak.unity.mcp.animation/     # The distributable Unity package source
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

## Package Dependencies

- `com.ivanmurzak.unity.mcp` — AI Game Developer platform (provides `McpPluginTool`, `MainThread`, `AssetObjectRef`, editor utilities)
- `com.unity.modules.animation` — Unity's built-in animation module
- OpenUPM scoped registry for `com.ivanmurzak.*` and `org.nuget.*` packages
