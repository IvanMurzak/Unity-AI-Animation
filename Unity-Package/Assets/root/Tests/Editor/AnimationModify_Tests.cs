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
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using com.IvanMurzak.McpPlugin;
using com.IvanMurzak.Unity.MCP.Runtime.Data;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    [TestFixture]
    public class AnimationModify_Tests
    {
        // ── Argument validation ─────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_NullRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                AnimationTools.ModifyAnimationClip(null!, new[] { new AnimationModification { type = ModificationType.ClearCurves } }));
        }

        [Test]
        public void ModifyAnimationClip_InvalidRef_ThrowsArgumentException()
        {
            var invalidRef = new AssetObjectRef();

            Assert.Throws<ArgumentException>(() =>
                AnimationTools.ModifyAnimationClip(invalidRef, new[] { new AnimationModification { type = ModificationType.ClearCurves } }));
        }

        [Test]
        public void ModifyAnimationClip_NullModifications_ThrowsArgumentNullException()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);

                Assert.Throws<ArgumentNullException>(() =>
                    AnimationTools.ModifyAnimationClip(animRef, null!));
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_EmptyModifications_ThrowsArgumentException()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);

                Assert.Throws<ArgumentException>(() =>
                    AnimationTools.ModifyAnimationClip(animRef, Array.Empty<AnimationModification>()));
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_NonExistentAsset_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                AnimationTools.ModifyAnimationClip(new AssetObjectRef("Assets/Tests/NonExistent.anim"), new[] { new AnimationModification { type = ModificationType.ClearCurves } }));
        }

        // ── SetCurve ────────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_SetCurve_Valid_CurveApplied()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification
                    {
                        type = ModificationType.SetCurve,
                        relativePath = string.Empty,
                        componentType = "UnityEngine.Transform",
                        propertyName = "m_LocalPosition.x",
                        keyframes = new[]
                        {
                            new AnimationKeyframe { time = 0f, value = 0f },
                            new AnimationKeyframe { time = 1f, value = 1f }
                        }
                    }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response);
                Assert.IsNull(response.errors, "Expected no errors");
                Assert.IsNotNull(response.modifiedAsset);

                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipEx.AssetPath);
                var bindings = AnimationUtility.GetCurveBindings(clip);
                Assert.IsTrue(bindings.Any(b => b.propertyName == "m_LocalPosition.x"),
                    "Curve should be applied to clip");
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetCurve_MissingComponentType_ReturnsError()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification
                    {
                        type = ModificationType.SetCurve,
                        propertyName = "m_LocalPosition.x",
                        keyframes = new[] { new AnimationKeyframe { time = 0f, value = 0f } }
                    }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response.errors);
                Assert.AreEqual(1, response.errors!.Count);
                StringAssert.Contains("componentType", response.errors[0]);
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetCurve_MissingPropertyName_ReturnsError()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification
                    {
                        type = ModificationType.SetCurve,
                        componentType = "UnityEngine.Transform",
                        keyframes = new[] { new AnimationKeyframe { time = 0f, value = 0f } }
                    }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response.errors);
                Assert.AreEqual(1, response.errors!.Count);
                StringAssert.Contains("propertyName", response.errors[0]);
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetCurve_MissingKeyframes_ReturnsError()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification
                    {
                        type = ModificationType.SetCurve,
                        componentType = "UnityEngine.Transform",
                        propertyName = "m_LocalPosition.x"
                        // No keyframes
                    }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response.errors);
                Assert.AreEqual(1, response.errors!.Count);
                StringAssert.Contains("keyframes", response.errors[0]);
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetCurve_InvalidComponentType_ReturnsError()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification
                    {
                        type = ModificationType.SetCurve,
                        componentType = "NotARealType.AtAll",
                        propertyName = "someProperty",
                        keyframes = new[] { new AnimationKeyframe { time = 0f, value = 0f } }
                    }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response.errors);
                Assert.AreEqual(1, response.errors!.Count);
            }).Execute();
        }

        // ── RemoveCurve ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_RemoveCurve_ExistingCurve_CurveRemoved()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var clip = clipEx.Asset ?? throw new InvalidOperationException("Clip should have been created by executor");
                clip.SetCurve(string.Empty, typeof(Transform), "m_LocalPosition.x", AnimationCurve.Linear(0f, 0f, 1f, 1f));
                clip.SetCurve(string.Empty, typeof(Transform), "m_LocalPosition.y", AnimationCurve.Linear(0f, 0f, 1f, 2f));
                EditorUtility.SetDirty(clip);
                AssetDatabase.SaveAssets();

                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification
                    {
                        type = ModificationType.RemoveCurve,
                        relativePath = string.Empty,
                        componentType = "UnityEngine.Transform",
                        propertyName = "m_LocalPosition.x"
                    }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response);
                Assert.IsNull(response.errors, "Expected no errors");

                var reloadedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipEx.AssetPath);
                var bindings = AnimationUtility.GetCurveBindings(reloadedClip);
                Assert.IsFalse(bindings.Any(b => b.propertyName == "m_LocalPosition.x"),
                    "Target curve should have been removed");
                Assert.IsTrue(bindings.Any(b => b.propertyName == "m_LocalPosition.y"),
                    "Unrelated curve should remain after removal");
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_RemoveCurve_MissingComponentType_ReturnsError()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification
                    {
                        type = ModificationType.RemoveCurve,
                        propertyName = "m_LocalPosition.x"
                    }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("componentType", response.errors![0]);
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_RemoveCurve_MissingPropertyName_ReturnsError()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification
                    {
                        type = ModificationType.RemoveCurve,
                        componentType = "UnityEngine.Transform"
                    }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("propertyName", response.errors![0]);
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_RemoveCurve_NonExistentCurve_SucceedsWithClipUnchanged()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var clip = clipEx.Asset ?? throw new InvalidOperationException("Clip should have been created by executor");
                clip.SetCurve(string.Empty, typeof(Transform), "m_LocalPosition.x", AnimationCurve.Linear(0f, 0f, 1f, 1f));
                EditorUtility.SetDirty(clip);
                AssetDatabase.SaveAssets();

                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification
                    {
                        type = ModificationType.RemoveCurve,
                        relativePath = string.Empty,
                        componentType = "UnityEngine.Transform",
                        propertyName = "m_LocalPosition.y" // Does not exist on the clip
                    }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNull(response.errors, "Removing a non-existent curve should not produce an error");

                var reloadedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipEx.AssetPath);
                var bindings = AnimationUtility.GetCurveBindings(reloadedClip);
                Assert.IsTrue(bindings.Any(b => b.propertyName == "m_LocalPosition.x"),
                    "Existing curve should remain untouched");
            }).Execute();
        }

        // ── ClearCurves ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_ClearCurves_RemovesAllCurves()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var clip = clipEx.Asset ?? throw new InvalidOperationException("Clip should have been created by executor");
                clip.SetCurve(string.Empty, typeof(Transform), "m_LocalPosition.x", AnimationCurve.Linear(0f, 0f, 1f, 1f));
                clip.SetCurve(string.Empty, typeof(Transform), "m_LocalPosition.y", AnimationCurve.Linear(0f, 0f, 1f, 1f));
                EditorUtility.SetDirty(clip);
                AssetDatabase.SaveAssets();

                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[] { new AnimationModification { type = ModificationType.ClearCurves } };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNull(response.errors);
                var reloadedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipEx.AssetPath);
                var bindings = AnimationUtility.GetCurveBindings(reloadedClip);
                Assert.AreEqual(0, bindings.Length, "All curves should be cleared");
            }).Execute();
        }

        // ── SetFrameRate ────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_SetFrameRate_Valid_FrameRateUpdated()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification { type = ModificationType.SetFrameRate, frameRate = 120f }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNull(response.errors);
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipEx.AssetPath);
                Assert.AreEqual(120f, clip.frameRate, 0.001f);
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetFrameRate_MissingValue_ReturnsError()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification { type = ModificationType.SetFrameRate }
                    // No frameRate value
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("frameRate", response.errors![0]);
            }).Execute();
        }

        // ── SetWrapMode ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_SetWrapMode_Valid_WrapModeUpdated()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification { type = ModificationType.SetWrapMode, wrapMode = WrapMode.Loop }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNull(response.errors);
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipEx.AssetPath);
                Assert.AreEqual(WrapMode.Loop, clip.wrapMode);
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetWrapMode_MissingValue_ReturnsError()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification { type = ModificationType.SetWrapMode }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("wrapMode", response.errors![0]);
            }).Execute();
        }

        // ── SetLegacy ───────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_SetLegacy_True_LegacyFlagSet()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification { type = ModificationType.SetLegacy, legacy = true }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNull(response.errors);
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipEx.AssetPath);
                Assert.IsTrue(clip.legacy);
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetLegacy_False_LegacyFlagCleared()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var clip = clipEx.Asset ?? throw new InvalidOperationException("Clip should have been created by executor");
                clip.legacy = true;
                EditorUtility.SetDirty(clip);
                AssetDatabase.SaveAssets();

                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification { type = ModificationType.SetLegacy, legacy = false }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNull(response.errors);
                var reloadedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipEx.AssetPath);
                Assert.IsFalse(reloadedClip.legacy);
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetLegacy_MissingValue_ReturnsError()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification { type = ModificationType.SetLegacy }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("legacy", response.errors![0]);
            }).Execute();
        }

        // ── AddEvent ────────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_AddEvent_Valid_EventAdded()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification
                    {
                        type = ModificationType.AddEvent,
                        time = 0.5f,
                        functionName = "OnAnimationEvent",
                        intParameter = 7,
                        floatParameter = 2.5f,
                        stringParameter = "test"
                    }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNull(response.errors);

                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipEx.AssetPath);
                var events = AnimationUtility.GetAnimationEvents(clip);
                Assert.AreEqual(1, events.Length);
                Assert.AreEqual(0.5f, events[0].time, 0.001f);
                Assert.AreEqual("OnAnimationEvent", events[0].functionName);
                Assert.AreEqual(7, events[0].intParameter);
                Assert.AreEqual(2.5f, events[0].floatParameter, 0.001f);
                Assert.AreEqual("test", events[0].stringParameter);
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_AddEvent_MissingTime_ReturnsError()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification
                    {
                        type = ModificationType.AddEvent,
                        functionName = "OnAnimationEvent"
                        // No time
                    }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("time", response.errors![0]);
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_AddEvent_MissingFunctionName_ReturnsError()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification
                    {
                        type = ModificationType.AddEvent,
                        time = 0.5f
                        // No functionName
                    }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("functionName", response.errors![0]);
            }).Execute();
        }

        // ── ClearEvents ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_ClearEvents_RemovesAllEvents()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var clip = clipEx.Asset ?? throw new InvalidOperationException("Clip should have been created by executor");
                AnimationUtility.SetAnimationEvents(clip, new[]
                {
                    new AnimationEvent { time = 0.1f, functionName = "Event1" },
                    new AnimationEvent { time = 0.5f, functionName = "Event2" }
                });
                EditorUtility.SetDirty(clip);
                AssetDatabase.SaveAssets();

                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[] { new AnimationModification { type = ModificationType.ClearEvents } };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNull(response.errors);
                var reloadedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipEx.AssetPath);
                var events = AnimationUtility.GetAnimationEvents(reloadedClip);
                Assert.AreEqual(0, events.Length, "All events should be cleared");
            }).Execute();
        }

        // ── Multiple modifications ───────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_MultipleModifications_AllApplied()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification { type = ModificationType.SetFrameRate, frameRate = 30f },
                    new AnimationModification { type = ModificationType.SetWrapMode, wrapMode = WrapMode.PingPong },
                    new AnimationModification
                    {
                        type = ModificationType.SetCurve,
                        relativePath = string.Empty,
                        componentType = "UnityEngine.Transform",
                        propertyName = "m_LocalPosition.x",
                        keyframes = new[]
                        {
                            new AnimationKeyframe { time = 0f, value = 0f },
                            new AnimationKeyframe { time = 1f, value = 5f }
                        }
                    },
                    new AnimationModification
                    {
                        type = ModificationType.AddEvent,
                        time = 0.0f,
                        functionName = "OnStart"
                    }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNull(response.errors, "All modifications should succeed");
                Assert.IsNotNull(response.modifiedAsset);

                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipEx.AssetPath);
                Assert.AreEqual(30f, clip.frameRate, 0.001f);
                Assert.AreEqual(WrapMode.PingPong, clip.wrapMode);
                var bindings = AnimationUtility.GetCurveBindings(clip);
                Assert.IsTrue(bindings.Any(b => b.propertyName == "m_LocalPosition.x"));
                var events = AnimationUtility.GetAnimationEvents(clip);
                Assert.AreEqual(1, events.Length);
                Assert.AreEqual("OnStart", events[0].functionName);
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_MultipleModificationsWithOneInvalid_AppliesValidAndCollectsErrors()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[]
                {
                    new AnimationModification { type = ModificationType.SetFrameRate, frameRate = 24f },
                    new AnimationModification { type = ModificationType.SetWrapMode }, // Missing value - error
                    new AnimationModification { type = ModificationType.SetLegacy, legacy = true }
                };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response.errors);
                Assert.AreEqual(1, response.errors!.Count, "Exactly one modification should fail");

                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipEx.AssetPath);
                Assert.AreEqual(24f, clip.frameRate, 0.001f, "Valid SetFrameRate should apply");
                Assert.IsTrue(clip.legacy, "Valid SetLegacy should apply");
            }).Execute();
        }

        [Test]
        public void ModifyAnimationClip_Response_ContainsModifiedAssetInfo()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var mods = new[] { new AnimationModification { type = ModificationType.ClearCurves } };

                var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                Assert.IsNotNull(response.modifiedAsset);
                Assert.AreEqual(clipEx.AssetPath, response.modifiedAsset!.path);
                Assert.AreEqual("TestClip", response.modifiedAsset.name);
                Assert.NotZero(response.modifiedAsset.instanceId);
            }).Execute();
        }
    }
}
