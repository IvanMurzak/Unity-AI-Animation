/*
┌─────────────────────────────────────────────────────────────────────────┐
│  Author: Ivan Murzak (https://github.com/IvanMurzak)                    │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-AI-Animation)  │
│  Copyright (c) 2025 Ivan Murzak                                         │
│  Licensed under the Apache License, Version 2.0.                        │
│  See the LICENSE file in the project root for more information.         │
└─────────────────────────────────────────────────────────────────────────┘
*/

#nullable enable

using System.ComponentModel;

namespace com.IvanMurzak.Unity.MCP.Animation
{
    public enum ModificationType
    {
        [Description("Add or modify an animation curve.")]
        SetCurve,

        [Description("Remove a specific animation curve.")]
        RemoveCurve,

        [Description("Remove all curves from the clip.")]
        ClearCurves,

        [Description("Set the frame rate of the clip.")]
        SetFrameRate,

        [Description("Set the wrap mode of the clip.")]
        SetWrapMode,

        [Description("Set whether the clip uses legacy animation system.")]
        SetLegacy,

        [Description("Add an animation event.")]
        AddEvent,

        [Description("Remove all animation events.")]
        ClearEvents
    }
}
