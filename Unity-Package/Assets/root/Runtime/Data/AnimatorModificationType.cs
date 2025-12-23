/*
┌─────────────────────────────────────────────────────────────────────────┐
│  Author: Ivan Murzak (https://github.com/IvanMurzak)                    │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-AI-Animation)  │
│  Copyright (c) 2025 Ivan Murzak                                         │
│  Licensed under the Apache License, Version 2.0.                        │
│  See the LICENSE file in the project root for more information.         │
└─────────────────────────────────────────────────────────────────────────┘
*/

namespace com.IvanMurzak.Unity.MCP.Animation
{
    public enum AnimatorModificationType
    {
        AddParameter,
        RemoveParameter,
        AddLayer,
        RemoveLayer,
        AddState,
        RemoveState,
        SetDefaultState,
        AddTransition,
        RemoveTransition,
        AddAnyStateTransition,
        SetStateMotion,
        SetStateSpeed
    }
}
