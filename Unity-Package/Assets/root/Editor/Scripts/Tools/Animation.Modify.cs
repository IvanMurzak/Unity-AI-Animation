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
using com.IvanMurzak.Unity.MCP.Runtime.Data;
using com.IvanMurzak.Unity.MCP.Runtime.Extensions;
using UnityEditor;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Animation
{
    public static partial class AnimationTools
    {
        [McpPluginTool
        (
            "animation-modify",
            Title = "Animation / Modify"
        )]
        [Description(@"Modify Unity's AnimationClip asset. Apply an array of modifications including setting curves, clearing curves, setting properties, and managing animation events.")]
        public static ModifyAnimationResponse ModifyAnimationClip
        (
            [Description("Reference to the AnimationClip asset to modify.")]
            AssetObjectRef animRef,

            [Description(@"Array of modifications to apply. Each modification has a 'type' field (ModificationType enum):
- SetCurve: Add or modify an animation curve. Requires: relativePath, componentType (e.g., 'Transform'), propertyName (e.g., 'localPosition.x'), keyframes array.
- RemoveCurve: Remove a specific curve. Requires: relativePath, componentType, propertyName.
- ClearCurves: Remove all curves from the clip.
- SetFrameRate: Set the frame rate. Requires: frameRate.
- SetWrapMode: Set the wrap mode. Requires: wrapMode (WrapMode enum).
- SetLegacy: Set legacy mode. Requires: legacy.
- AddEvent: Add an animation event. Requires: time, functionName. Optional: stringParameter, floatParameter, intParameter.
- ClearEvents: Remove all animation events.")]
            AnimationModification[] modifications
        )
        {
            return MainThread.Instance.Run(() =>
            {
                var animation = animRef.FindAssetObject<AnimationClip>();
                if (animation == null)
                    throw new Exception("AnimationClip not found.");

                if (modifications == null || modifications.Length == 0)
                    throw new Exception("Modifications array is empty or null.");

                var response = new ModifyAnimationResponse();
                var eventsList = new List<AnimationEvent>(AnimationUtility.GetAnimationEvents(animation));

                for (int i = 0; i < modifications.Length; i++)
                {
                    var mod = modifications[i];
                    try
                    {
                        ApplyModification(animation, mod, eventsList);
                    }
                    catch (Exception ex)
                    {
                        response.errors ??= new List<string>();
                        response.errors.Add($"[{i}] {mod.type}: {ex.Message}");
                    }
                }

                // Apply collected events
                AnimationUtility.SetAnimationEvents(animation, eventsList.ToArray());

                EditorUtility.SetDirty(animation);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                EditorApplication.RepaintProjectWindow();
                EditorApplication.RepaintHierarchyWindow();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

                var assetPath = AssetDatabase.GetAssetPath(animation);
                response.modifiedAsset = new ModifyAnimationInfo
                {
                    path = assetPath,
                    instanceId = animation.GetInstanceID(),
                    name = animation.name
                };

                return response;
            });
        }

        private static void ApplyModification(AnimationClip clip, AnimationModification mod, List<AnimationEvent> eventsList)
        {
            switch (mod.type)
            {
                case ModificationType.SetCurve:
                    ApplySetCurve(clip, mod);
                    break;

                case ModificationType.RemoveCurve:
                    ApplyRemoveCurve(clip, mod);
                    break;

                case ModificationType.ClearCurves:
                    clip.ClearCurves();
                    break;

                case ModificationType.SetFrameRate:
                    if (!mod.frameRate.HasValue)
                        throw new Exception("frameRate is required for SetFrameRate.");
                    clip.frameRate = mod.frameRate.Value;
                    break;

                case ModificationType.SetWrapMode:
                    if (!mod.wrapMode.HasValue)
                        throw new Exception("wrapMode is required for SetWrapMode.");
                    clip.wrapMode = mod.wrapMode.Value;
                    break;

                case ModificationType.SetLegacy:
                    if (!mod.legacy.HasValue)
                        throw new Exception("legacy is required for SetLegacy.");
                    clip.legacy = mod.legacy.Value;
                    break;

                case ModificationType.AddEvent:
                    ApplyAddEvent(eventsList, mod);
                    break;

                case ModificationType.ClearEvents:
                    eventsList.Clear();
                    break;

                default:
                    throw new Exception($"Unknown modification type: {mod.type}");
            }
        }

        private static void ApplySetCurve(AnimationClip clip, AnimationModification mod)
        {
            if (string.IsNullOrEmpty(mod.componentType))
                throw new Exception("componentType is required for setCurve.");
            if (string.IsNullOrEmpty(mod.propertyName))
                throw new Exception("propertyName is required for setCurve.");
            if (mod.keyframes == null || mod.keyframes.Length == 0)
                throw new Exception("keyframes array is required for setCurve.");

            var type = ResolveComponentType(mod.componentType);
            if (type == null)
                throw new Exception($"Could not resolve component type: {mod.componentType}");

            var curve = new AnimationCurve();
            foreach (var kf in mod.keyframes)
            {
                var keyframe = new Keyframe(kf.time, kf.value)
                {
                    inTangent = kf.inTangent ?? 0f,
                    outTangent = kf.outTangent ?? 0f,
                    inWeight = kf.inWeight ?? 0.33f,
                    outWeight = kf.outWeight ?? 0.33f,
                    weightedMode = kf.weightedMode ?? WeightedMode.None
                };
                curve.AddKey(keyframe);
            }

            clip.SetCurve(mod.relativePath ?? "", type, mod.propertyName, curve);
        }

        private static void ApplyRemoveCurve(AnimationClip clip, AnimationModification mod)
        {
            if (string.IsNullOrEmpty(mod.componentType))
                throw new Exception("componentType is required for removeCurve.");
            if (string.IsNullOrEmpty(mod.propertyName))
                throw new Exception("propertyName is required for removeCurve.");

            var type = ResolveComponentType(mod.componentType);
            if (type == null)
                throw new Exception($"Could not resolve component type: {mod.componentType}");

            clip.SetCurve(mod.relativePath ?? "", type, mod.propertyName, null);
        }

        private static void ApplyAddEvent(List<AnimationEvent> eventsList, AnimationModification mod)
        {
            if (!mod.time.HasValue)
                throw new Exception("time is required for addEvent.");
            if (string.IsNullOrEmpty(mod.functionName))
                throw new Exception("functionName is required for addEvent.");

            var animEvent = new AnimationEvent
            {
                time = mod.time.Value,
                functionName = mod.functionName,
                stringParameter = mod.stringParameter ?? "",
                floatParameter = mod.floatParameter ?? 0f,
                intParameter = mod.intParameter ?? 0
            };
            eventsList.Add(animEvent);
        }

        private static Type? ResolveComponentType(string typeName)
        {
            // Try direct type lookup
            var type = Type.GetType(typeName);
            if (type != null) return type;

            // Try common Unity namespaces
            var unityNamespaces = new[]
            {
                "UnityEngine.",
                "UnityEngine.UI.",
                "UnityEngine.Rendering.",
                "TMPro."
            };

            foreach (var ns in unityNamespaces)
            {
                type = Type.GetType($"{ns}{typeName}, UnityEngine");
                if (type != null) return type;

                type = Type.GetType($"{ns}{typeName}, UnityEngine.CoreModule");
                if (type != null) return type;

                type = Type.GetType($"{ns}{typeName}, UnityEngine.UIModule");
                if (type != null) return type;
            }

            // Search all loaded assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null) return type;

                foreach (var ns in unityNamespaces)
                {
                    type = assembly.GetType($"{ns}{typeName}");
                    if (type != null) return type;
                }
            }

            return null;
        }

        #region Response Data Classes

        public class ModifyAnimationInfo
        {
            public string path = string.Empty;
            public int instanceId;
            public string name = string.Empty;
        }

        public class ModifyAnimationResponse
        {
            public ModifyAnimationInfo? modifiedAsset;
            public List<string>? errors;
        }

        #endregion
    }
}