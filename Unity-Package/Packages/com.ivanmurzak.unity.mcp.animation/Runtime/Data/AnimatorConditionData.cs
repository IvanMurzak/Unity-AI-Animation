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
    public class AnimatorConditionData
    {
        [Description("Parameter name for the condition.")]
        public string? parameter;

        [Description("Condition mode: If, IfNot, Greater, Less, Equals, NotEqual.")]
        public string? mode;

        [Description("Threshold value for the condition.")]
        public float? threshold;
    }
}
