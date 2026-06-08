/*
┌─────────────────────────────────────────────────────────────────────────┐
│  Author: Ivan Murzak (https://github.com/IvanMurzak)                    │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-AI-Animation)  │
│  Copyright (c) 2025 Ivan Murzak                                         │
│  Licensed under the Apache License, Version 2.0.                        │
│  See the LICENSE file in the project root for more information.         │
└─────────────────────────────────────────────────────────────────────────┘
*/

using System.ComponentModel;

namespace com.IvanMurzak.Unity.MCP.Animation
{
    public enum AnimatorModificationType
    {
        [Description("Add a new parameter to the controller.")]
        AddParameter,

        [Description("Remove an existing parameter.")]
        RemoveParameter,

        [Description("Add a new layer to the controller.")]
        AddLayer,

        [Description("Remove an existing layer.")]
        RemoveLayer,

        [Description("Add a new state to a layer.")]
        AddState,

        [Description("Remove an existing state from a layer.")]
        RemoveState,

        [Description("Set the default state for a layer.")]
        SetDefaultState,

        [Description("Add a transition between two states.")]
        AddTransition,

        [Description("Remove a transition between two states.")]
        RemoveTransition,

        [Description("Add a transition from Any State.")]
        AddAnyStateTransition,

        [Description("Set the motion (AnimationClip) for a state.")]
        SetStateMotion,

        [Description("Set the speed multiplier for a state.")]
        SetStateSpeed
    }
}
