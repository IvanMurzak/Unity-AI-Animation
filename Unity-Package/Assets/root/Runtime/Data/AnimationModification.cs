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
    public class AnimationModification
    {
        [Description("Type of modification to apply.")]
        public ModificationType type;

        // Curve-related properties
        [Description("Relative path to the target GameObject (empty string for root).")]
        public string? relativePath;

        [Description("Component type name (e.g., 'Transform', 'SpriteRenderer').")]
        public string? componentType;

        [Description("Property name to animate (e.g., 'localPosition.x', 'm_LocalScale.y').")]
        public string? propertyName;

        [Description("Array of keyframes for the animation curve.")]
        public AnimationKeyframe[]? keyframes;

        // Property-related
        [Description("Frame rate for the animation clip.")]
        public float? frameRate;

        [Description("Wrap mode for the animation clip.")]
        public WrapMode? wrapMode;

        [Description("Whether the clip uses legacy animation system.")]
        public bool? legacy;

        // Event-related
        [Description("Time in seconds when the event should fire.")]
        public float? time;

        [Description("Name of the function to call.")]
        public string? functionName;

        [Description("String parameter for the event.")]
        public string? stringParameter;

        [Description("Float parameter for the event.")]
        public float? floatParameter;

        [Description("Integer parameter for the event.")]
        public int? intParameter;
    }
}
