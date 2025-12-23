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
    public class AnimatorModification
    {
        [Description("Type of modification to apply.")]
        public AnimatorModificationType type;

        // Parameter fields
        [Description("Name of the parameter.")]
        public string? parameterName;

        [Description("Parameter type: Float, Int, Bool, Trigger.")]
        public string? parameterType;

        [Description("Default float value for the parameter.")]
        public float? defaultFloat;

        [Description("Default int value for the parameter.")]
        public int? defaultInt;

        [Description("Default bool value for the parameter.")]
        public bool? defaultBool;

        // Layer fields
        [Description("Name of the layer.")]
        public string? layerName;

        // State fields
        [Description("Name of the state.")]
        public string? stateName;

        [Description("Asset path to the motion (AnimationClip).")]
        public string? motionAssetPath;

        [Description("Speed multiplier for the state.")]
        public float? speed;

        // Transition fields
        [Description("Name of the source state for transitions.")]
        public string? sourceStateName;

        [Description("Name of the destination state for transitions.")]
        public string? destinationStateName;

        [Description("Whether the transition has exit time.")]
        public bool? hasExitTime;

        [Description("Exit time for the transition.")]
        public float? exitTime;

        [Description("Duration of the transition.")]
        public float? duration;

        [Description("Whether the transition has fixed duration.")]
        public bool? hasFixedDuration;

        [Description("Conditions for the transition.")]
        public AnimatorConditionData[]? conditions;
    }
}
