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
    public class AnimatorTransitionInfo
    {
        public string? destinationStateName;
        public bool hasExitTime;
        public float exitTime;
        public bool hasFixedDuration;
        public float duration;
        public float offset;
        public bool canTransitionToSelf;
        public List<AnimatorConditionInfo>? conditions;
    }
}
