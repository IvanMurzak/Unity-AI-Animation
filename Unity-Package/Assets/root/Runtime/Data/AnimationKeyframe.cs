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
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Animation
{
    public class AnimationKeyframe
    {
        [Description("Time in seconds.")]
        public float time;

        [Description("Value at this keyframe.")]
        public float value;

        [Description("Incoming tangent (slope). Default: 0.")]
        public float? inTangent;

        [Description("Outgoing tangent (slope). Default: 0.")]
        public float? outTangent;

        [Description("Weighted mode: None (0), In (1), Out (2), Both (3). Default: None.")]
        public WeightedMode? weightedMode;

        [Description("Incoming weight. Default: 0.33.")]
        public float? inWeight;

        [Description("Outgoing weight. Default: 0.33.")]
        public float? outWeight;
    }
}
