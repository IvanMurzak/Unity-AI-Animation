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
using System.ComponentModel;
using com.IvanMurzak.McpPlugin;
using com.IvanMurzak.ReflectorNet.Utils;
using com.IvanMurzak.Unity.MCP.Runtime.Data;
using com.IvanMurzak.Unity.MCP.Runtime.Extensions;
using UnityEditor.Animations;

namespace com.IvanMurzak.Unity.MCP.Animation
{
    public static partial class AnimatorTools
    {
        [McpPluginTool
        (
            "animator-get-data",
            Title = "Animator / Get Data"
        )]
        [Description(@"Get data about a Unity AnimatorController asset file. Returns information such as name, layers, parameters, and states.")]
        public static GetAnimatorDataResponse GetData
        (
            [Description("Reference to the AnimatorController asset. The path should start with 'Assets/' and end with '.controller'.")]
            AssetObjectRef animatorRef
        )
        {
            return MainThread.Instance.Run(() =>
            {
                var controller = animatorRef.FindAssetObject<AnimatorController>();
                if (controller == null)
                    throw new System.Exception($"AnimatorController not found.");

                var response = new GetAnimatorDataResponse
                {
                    name = controller.name
                };

                // Get parameters
                response.parameters = new List<AnimatorParameterInfo>();
                foreach (var param in controller.parameters)
                {
                    response.parameters.Add(new AnimatorParameterInfo
                    {
                        name = param.name,
                        type = param.type.ToString(),
                        defaultFloat = param.defaultFloat,
                        defaultInt = param.defaultInt,
                        defaultBool = param.defaultBool
                    });
                }

                // Get layers
                response.layers = new List<AnimatorLayerInfo>();
                foreach (var layer in controller.layers)
                {
                    var layerInfo = new AnimatorLayerInfo
                    {
                        name = layer.name,
                        defaultWeight = layer.defaultWeight,
                        blendingMode = layer.blendingMode.ToString(),
                        syncedLayerIndex = layer.syncedLayerIndex,
                        iKPass = layer.iKPass
                    };

                    // Get states in the layer's state machine
                    if (layer.stateMachine != null)
                    {
                        layerInfo.states = new List<AnimatorStateInfo>();
                        foreach (var childState in layer.stateMachine.states)
                        {
                            var state = childState.state;
                            var stateInfo = new AnimatorStateInfo
                            {
                                name = state.name,
                                tag = state.tag,
                                speed = state.speed,
                                speedParameterActive = state.speedParameterActive,
                                speedParameter = state.speedParameter,
                                cycleOffset = state.cycleOffset,
                                cycleOffsetParameterActive = state.cycleOffsetParameterActive,
                                cycleOffsetParameter = state.cycleOffsetParameter,
                                mirror = state.mirror,
                                mirrorParameterActive = state.mirrorParameterActive,
                                mirrorParameter = state.mirrorParameter,
                                writeDefaultValues = state.writeDefaultValues,
                                motionName = state.motion != null ? state.motion.name : null
                            };

                            // Get transitions
                            stateInfo.transitions = new List<AnimatorTransitionInfo>();
                            foreach (var transition in state.transitions)
                            {
                                var transitionInfo = new AnimatorTransitionInfo
                                {
                                    destinationStateName = transition.destinationState?.name,
                                    hasExitTime = transition.hasExitTime,
                                    exitTime = transition.exitTime,
                                    hasFixedDuration = transition.hasFixedDuration,
                                    duration = transition.duration,
                                    offset = transition.offset,
                                    canTransitionToSelf = transition.canTransitionToSelf
                                };

                                // Get conditions
                                transitionInfo.conditions = new List<AnimatorConditionInfo>();
                                foreach (var condition in transition.conditions)
                                {
                                    transitionInfo.conditions.Add(new AnimatorConditionInfo
                                    {
                                        parameter = condition.parameter,
                                        mode = condition.mode.ToString(),
                                        threshold = condition.threshold
                                    });
                                }

                                stateInfo.transitions.Add(transitionInfo);
                            }

                            layerInfo.states.Add(stateInfo);
                        }

                        // Get sub-state machines
                        layerInfo.subStateMachines = new List<string>();
                        foreach (var childMachine in layer.stateMachine.stateMachines)
                        {
                            layerInfo.subStateMachines.Add(childMachine.stateMachine.name);
                        }

                        // Get default state
                        layerInfo.defaultStateName = layer.stateMachine.defaultState?.name;

                        // Get any state transitions
                        layerInfo.anyStateTransitions = new List<AnimatorTransitionInfo>();
                        foreach (var transition in layer.stateMachine.anyStateTransitions)
                        {
                            var transitionInfo = new AnimatorTransitionInfo
                            {
                                destinationStateName = transition.destinationState?.name,
                                hasExitTime = transition.hasExitTime,
                                exitTime = transition.exitTime,
                                hasFixedDuration = transition.hasFixedDuration,
                                duration = transition.duration,
                                offset = transition.offset,
                                canTransitionToSelf = transition.canTransitionToSelf
                            };

                            transitionInfo.conditions = new List<AnimatorConditionInfo>();
                            foreach (var condition in transition.conditions)
                            {
                                transitionInfo.conditions.Add(new AnimatorConditionInfo
                                {
                                    parameter = condition.parameter,
                                    mode = condition.mode.ToString(),
                                    threshold = condition.threshold
                                });
                            }

                            layerInfo.anyStateTransitions.Add(transitionInfo);
                        }
                    }

                    response.layers.Add(layerInfo);
                }

                return response;
            });
        }

        #region GetData Response Classes

        public class GetAnimatorDataResponse
        {
            public string name = string.Empty;
            public List<AnimatorParameterInfo>? parameters;
            public List<AnimatorLayerInfo>? layers;
        }

        #endregion
    }
}
