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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using com.IvanMurzak.McpPlugin;
using com.IvanMurzak.ReflectorNet.Utils;
using AIGD;
using com.IvanMurzak.Unity.MCP.Runtime.Extensions;
using UnityEditor;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Animation
{
    public static partial class AnimationTools
    {
        public const string AnimationGetDataToolId = "animation-get-data";
        [AiTool
        (
            AnimationGetDataToolId,
            Title = "Animation / Get Data",
            ReadOnlyHint = true,
            DestructiveHint = false,
            IdempotentHint = true,
            OpenWorldHint = false
        )]
        [AiSkillDescription("Inspect a Unity `AnimationClip` asset — name, length, frame rate, wrap mode, " +
            "looping/legacy/humanMotion flags, local bounds, and the full set of float curves, object-reference " +
            "curves, and events. Pair with '" + AnimationModifyToolId + "' to write changes back.")]
        [AiSkillBody("Inspect a Unity `AnimationClip` asset. Returns the high-level clip metadata plus the " +
            "complete set of float curve bindings, object-reference curve bindings, and animation events. Pair " +
            "with '" + AnimationModifyToolId + "' to write changes back.\n\n" +
            "## Inputs\n\n" +
            "- `animRef` — reference to the `AnimationClip` asset (path must start with `Assets/` and end with " +
            "`.anim`).\n\n" +
            "## Returned fields\n\n" +
            "- `name`, `length`, `frameRate`, `wrapMode`, `isLooping`, `hasGenericRootTransform`, " +
            "`hasMotionCurves`, `hasMotionFloatCurves`, `hasRootCurves`, `humanMotion`, `legacy`, `localBounds`, " +
            "`empty`.\n" +
            "- `curveBindings` — float curve bindings (`path`, `propertyName`, `type`, `isPPtrCurve`, " +
            "`isDiscreteCurve`, `keyframeCount`).\n" +
            "- `objectReferenceBindings` — object-reference curve bindings (same shape as `curveBindings`).\n" +
            "- `events` — animation events (`time`, `functionName`, `intParameter`, `floatParameter`, " +
            "`stringParameter`).")]
        [Description(@"Get data about a Unity AnimationClip asset file. Returns information such as name, length, frame rate, wrap mode, animation curves, and events.")]
        public static GetDataResponse GetData
        (
            [Description("Reference to the animation asset. The path should start with 'Assets/' and end with '.anim'.")]
            AssetObjectRef animRef
        )
        {
            if (animRef == null)
                throw new ArgumentNullException(nameof(animRef));

            if (!animRef.IsValid(out var animRefValidationError))
                throw new ArgumentException(animRefValidationError, nameof(animRef));

            return MainThread.Instance.Run(() =>
            {
                var animation = animRef.FindAssetObject<AnimationClip>();
                if (animation == null)
                    throw new System.Exception($"AnimationClip not found.");

                var response = new GetDataResponse
                {
                    name = animation.name,
                    length = animation.length,
                    frameRate = animation.frameRate,
                    wrapMode = animation.wrapMode.ToString(),
                    isLooping = animation.isLooping,
                    hasGenericRootTransform = animation.hasGenericRootTransform,
                    hasMotionCurves = animation.hasMotionCurves,
                    hasMotionFloatCurves = animation.hasMotionFloatCurves,
                    hasRootCurves = animation.hasRootCurves,
                    humanMotion = animation.humanMotion,
                    legacy = animation.legacy,
                    localBounds = animation.localBounds.ToString(),
                    empty = animation.empty
                };

                // Get curve bindings
                var curveBindings = AnimationUtility.GetCurveBindings(animation);
                response.curveBindings = new List<CurveBindingInfo>();
                foreach (var binding in curveBindings)
                {
                    var curve = AnimationUtility.GetEditorCurve(animation, binding);
                    response.curveBindings.Add(new CurveBindingInfo
                    {
                        path = binding.path,
                        propertyName = binding.propertyName,
                        type = binding.type.FullName ?? binding.type.Name,
                        isPPtrCurve = binding.isPPtrCurve,
                        isDiscreteCurve = binding.isDiscreteCurve,
                        keyframeCount = curve?.length ?? 0
                    });
                }

                // Get object reference curve bindings
                var objectBindings = AnimationUtility.GetObjectReferenceCurveBindings(animation);
                response.objectReferenceBindings = new List<CurveBindingInfo>();
                foreach (var binding in objectBindings)
                {
                    var keyframes = AnimationUtility.GetObjectReferenceCurve(animation, binding);
                    response.objectReferenceBindings.Add(new CurveBindingInfo
                    {
                        path = binding.path,
                        propertyName = binding.propertyName,
                        type = binding.type.FullName ?? binding.type.Name,
                        isPPtrCurve = binding.isPPtrCurve,
                        isDiscreteCurve = binding.isDiscreteCurve,
                        keyframeCount = keyframes?.Length ?? 0
                    });
                }

                // Get animation events
                var events = AnimationUtility.GetAnimationEvents(animation);
                response.events = new List<AnimationEventInfo>();
                foreach (var evt in events)
                {
                    response.events.Add(new AnimationEventInfo
                    {
                        time = evt.time,
                        functionName = evt.functionName,
                        intParameter = evt.intParameter,
                        floatParameter = evt.floatParameter,
                        stringParameter = evt.stringParameter
                    });
                }

                return response;
            });
        }

        public class CurveBindingInfo
        {
            public string path = string.Empty;
            public string propertyName = string.Empty;
            public string type = string.Empty;
            public bool isPPtrCurve;
            public bool isDiscreteCurve;
            public int keyframeCount;
        }

        public class AnimationEventInfo
        {
            public float time;
            public string functionName = string.Empty;
            public int intParameter;
            public float floatParameter;
            public string stringParameter = string.Empty;
        }

        public class GetDataResponse
        {
            public string name = string.Empty;
            public float length;
            public float frameRate;
            public string wrapMode = string.Empty;
            public bool isLooping;
            public bool hasGenericRootTransform;
            public bool hasMotionCurves;
            public bool hasMotionFloatCurves;
            public bool hasRootCurves;
            public bool humanMotion;
            public bool legacy;
            public string localBounds = string.Empty;
            public bool empty;
            public List<CurveBindingInfo>? curveBindings;
            public List<CurveBindingInfo>? objectReferenceBindings;
            public List<AnimationEventInfo>? events;
        }
    }
}
