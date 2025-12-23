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
    public class AnimatorStateInfo
    {
        public string name = string.Empty;
        public string tag = string.Empty;
        public float speed;
        public bool speedParameterActive;
        public string speedParameter = string.Empty;
        public float cycleOffset;
        public bool cycleOffsetParameterActive;
        public string cycleOffsetParameter = string.Empty;
        public bool mirror;
        public bool mirrorParameterActive;
        public string mirrorParameter = string.Empty;
        public bool writeDefaultValues;
        public string? motionName;
        public List<AnimatorTransitionInfo>? transitions;
    }
}
