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

using System.Collections.Generic;

namespace com.IvanMurzak.Unity.MCP.Animation
{
    public class AnimatorLayerInfo
    {
        public string name = string.Empty;
        public float defaultWeight;
        public string blendingMode = string.Empty;
        public int syncedLayerIndex;
        public bool iKPass;
        public string? defaultStateName;
        public List<AnimatorStateInfo>? states;
        public List<string>? subStateMachines;
        public List<AnimatorTransitionInfo>? anyStateTransitions;
    }
}
