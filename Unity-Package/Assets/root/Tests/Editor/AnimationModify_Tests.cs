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

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    [TestFixture]
    public class AnimationModify_Tests
    {
        private const string TestFolder = "Assets/Tests/MCP/Animation/ModifyTests";
        private const string TestClipPath = TestFolder + "/TestClip.anim";

        private AnimationClipExecutor _clipExecutor = null!;

        [SetUp]
        public void SetUp()
        {
            _clipExecutor = new AnimationClipExecutor(TestClipPath);
            _clipExecutor.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            _clipExecutor.Teardown();

            if (AssetDatabase.IsValidFolder(TestFolder))
            {
                AssetDatabase.DeleteAsset(TestFolder);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }

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
            var animRef = new AssetObjectRef(TestClipPath);

            Assert.Throws<ArgumentNullException>(() =>
                AnimationTools.ModifyAnimationClip(animRef, null!));
        }

        [Test]
        public void ModifyAnimationClip_EmptyModifications_ThrowsArgumentException()
        {
            var animRef = new AssetObjectRef(TestClipPath);

            Assert.Throws<ArgumentException>(() =>
                AnimationTools.ModifyAnimationClip(animRef, Array.Empty<AnimationModification>()));
        }

        [Test]
        public void ModifyAnimationClip_NonExistentAsset_ThrowsException()
        {
            var animRef = new AssetObjectRef($"{TestFolder}/NonExistent.anim");

            Assert.Throws<Exception>(() =>
                AnimationTools.ModifyAnimationClip(animRef, new[] { new AnimationModification { type = ModificationType.ClearCurves } }));
        }

        // ── SetCurve ────────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_SetCurve_Valid_CurveApplied()
        {
            var animRef = new AssetObjectRef(TestClipPath);
            var mods = new[]
            {
                new AnimationModification
                {
                    type = ModificationType.SetCurve,
                    relativePath = "",
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

            var clip = _clipExecutor.Clip!;
            var bindings = AnimationUtility.GetCurveBindings(clip);
            Assert.IsTrue(bindings.Any(b => b.propertyName == "m_LocalPosition.x"),
                "Curve should be applied to clip");
        }

        [Test]
        public void ModifyAnimationClip_SetCurve_MissingComponentType_ReturnsError()
        {
            var animRef = new AssetObjectRef(TestClipPath);
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
        }

        [Test]
        public void ModifyAnimationClip_SetCurve_MissingPropertyName_ReturnsError()
        {
            var animRef = new AssetObjectRef(TestClipPath);
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
        }

        [Test]
        public void ModifyAnimationClip_SetCurve_MissingKeyframes_ReturnsError()
        {
            var animRef = new AssetObjectRef(TestClipPath);
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
        }

        [Test]
        public void ModifyAnimationClip_SetCurve_InvalidComponentType_ReturnsError()
        {
            var animRef = new AssetObjectRef(TestClipPath);
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
        }

        // ── RemoveCurve ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_RemoveCurve_ExistingCurve_CurveRemoved()
        {
            // First add a curve directly
            var clip = _clipExecutor.Clip!;
            clip.SetCurve("", typeof(Transform), "m_LocalPosition.x", AnimationCurve.Linear(0f, 0f, 1f, 1f));
            EditorUtility.SetDirty(clip);
            AssetDatabase.SaveAssets();

            var animRef = new AssetObjectRef(TestClipPath);
            var mods = new[]
            {
                new AnimationModification
                {
                    type = ModificationType.RemoveCurve,
                    relativePath = "",
                    componentType = "UnityEngine.Transform",
                    propertyName = "m_LocalPosition.x"
                }
            };

            var response = AnimationTools.ModifyAnimationClip(animRef, mods);

            Assert.IsNotNull(response);
            Assert.IsNull(response.errors, "Expected no errors");

            var bindings = AnimationUtility.GetCurveBindings(_clipExecutor.Clip!);
            Assert.IsFalse(bindings.Any(b => b.propertyName == "m_LocalPosition.x"),
                "Curve should have been removed");
        }

        [Test]
        public void ModifyAnimationClip_RemoveCurve_MissingComponentType_ReturnsError()
        {
            var animRef = new AssetObjectRef(TestClipPath);
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
        }

        [Test]
        public void ModifyAnimationClip_RemoveCurve_MissingPropertyName_ReturnsError()
        {
            var animRef = new AssetObjectRef(TestClipPath);
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
        }

        // ── ClearCurves ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_ClearCurves_RemovesAllCurves()
        {
            // Add multiple curves
            var clip = _clipExecutor.Clip!;
            clip.SetCurve("", typeof(Transform), "m_LocalPosition.x", AnimationCurve.Linear(0f, 0f, 1f, 1f));
            clip.SetCurve("", typeof(Transform), "m_LocalPosition.y", AnimationCurve.Linear(0f, 0f, 1f, 1f));
            EditorUtility.SetDirty(clip);
            AssetDatabase.SaveAssets();

            var animRef = new AssetObjectRef(TestClipPath);
            var mods = new[] { new AnimationModification { type = ModificationType.ClearCurves } };

            var response = AnimationTools.ModifyAnimationClip(animRef, mods);

            Assert.IsNull(response.errors);
            var bindings = AnimationUtility.GetCurveBindings(_clipExecutor.Clip!);
            Assert.AreEqual(0, bindings.Length, "All curves should be cleared");
        }

        // ── SetFrameRate ────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_SetFrameRate_Valid_FrameRateUpdated()
        {
            var animRef = new AssetObjectRef(TestClipPath);
            var mods = new[]
            {
                new AnimationModification { type = ModificationType.SetFrameRate, frameRate = 120f }
            };

            var response = AnimationTools.ModifyAnimationClip(animRef, mods);

            Assert.IsNull(response.errors);
            Assert.AreEqual(120f, _clipExecutor.Clip!.frameRate, 0.001f);
        }

        [Test]
        public void ModifyAnimationClip_SetFrameRate_MissingValue_ReturnsError()
        {
            var animRef = new AssetObjectRef(TestClipPath);
            var mods = new[]
            {
                new AnimationModification { type = ModificationType.SetFrameRate }
                // No frameRate value
            };

            var response = AnimationTools.ModifyAnimationClip(animRef, mods);

            Assert.IsNotNull(response.errors);
            StringAssert.Contains("frameRate", response.errors![0]);
        }

        // ── SetWrapMode ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_SetWrapMode_Valid_WrapModeUpdated()
        {
            var animRef = new AssetObjectRef(TestClipPath);
            var mods = new[]
            {
                new AnimationModification { type = ModificationType.SetWrapMode, wrapMode = WrapMode.Loop }
            };

            var response = AnimationTools.ModifyAnimationClip(animRef, mods);

            Assert.IsNull(response.errors);
            Assert.AreEqual(WrapMode.Loop, _clipExecutor.Clip!.wrapMode);
        }

        [Test]
        public void ModifyAnimationClip_SetWrapMode_MissingValue_ReturnsError()
        {
            var animRef = new AssetObjectRef(TestClipPath);
            var mods = new[]
            {
                new AnimationModification { type = ModificationType.SetWrapMode }
            };

            var response = AnimationTools.ModifyAnimationClip(animRef, mods);

            Assert.IsNotNull(response.errors);
            StringAssert.Contains("wrapMode", response.errors![0]);
        }

        // ── SetLegacy ───────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_SetLegacy_True_LegacyFlagSet()
        {
            var animRef = new AssetObjectRef(TestClipPath);
            var mods = new[]
            {
                new AnimationModification { type = ModificationType.SetLegacy, legacy = true }
            };

            var response = AnimationTools.ModifyAnimationClip(animRef, mods);

            Assert.IsNull(response.errors);
            Assert.IsTrue(_clipExecutor.Clip!.legacy);
        }

        [Test]
        public void ModifyAnimationClip_SetLegacy_False_LegacyFlagCleared()
        {
            // Set legacy to true first
            var clip = _clipExecutor.Clip!;
            clip.legacy = true;
            EditorUtility.SetDirty(clip);
            AssetDatabase.SaveAssets();

            var animRef = new AssetObjectRef(TestClipPath);
            var mods = new[]
            {
                new AnimationModification { type = ModificationType.SetLegacy, legacy = false }
            };

            var response = AnimationTools.ModifyAnimationClip(animRef, mods);

            Assert.IsNull(response.errors);
            Assert.IsFalse(_clipExecutor.Clip!.legacy);
        }

        [Test]
        public void ModifyAnimationClip_SetLegacy_MissingValue_ReturnsError()
        {
            var animRef = new AssetObjectRef(TestClipPath);
            var mods = new[]
            {
                new AnimationModification { type = ModificationType.SetLegacy }
            };

            var response = AnimationTools.ModifyAnimationClip(animRef, mods);

            Assert.IsNotNull(response.errors);
            StringAssert.Contains("legacy", response.errors![0]);
        }

        // ── AddEvent ────────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_AddEvent_Valid_EventAdded()
        {
            var animRef = new AssetObjectRef(TestClipPath);
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

            var events = AnimationUtility.GetAnimationEvents(_clipExecutor.Clip!);
            Assert.AreEqual(1, events.Length);
            Assert.AreEqual(0.5f, events[0].time, 0.001f);
            Assert.AreEqual("OnAnimationEvent", events[0].functionName);
            Assert.AreEqual(7, events[0].intParameter);
            Assert.AreEqual(2.5f, events[0].floatParameter, 0.001f);
            Assert.AreEqual("test", events[0].stringParameter);
        }

        [Test]
        public void ModifyAnimationClip_AddEvent_MissingTime_ReturnsError()
        {
            var animRef = new AssetObjectRef(TestClipPath);
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
        }

        [Test]
        public void ModifyAnimationClip_AddEvent_MissingFunctionName_ReturnsError()
        {
            var animRef = new AssetObjectRef(TestClipPath);
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
        }

        // ── ClearEvents ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_ClearEvents_RemovesAllEvents()
        {
            var clip = _clipExecutor.Clip!;
            AnimationUtility.SetAnimationEvents(clip, new[]
            {
                new AnimationEvent { time = 0.1f, functionName = "Event1" },
                new AnimationEvent { time = 0.5f, functionName = "Event2" }
            });
            EditorUtility.SetDirty(clip);
            AssetDatabase.SaveAssets();

            var animRef = new AssetObjectRef(TestClipPath);
            var mods = new[] { new AnimationModification { type = ModificationType.ClearEvents } };

            var response = AnimationTools.ModifyAnimationClip(animRef, mods);

            Assert.IsNull(response.errors);
            var events = AnimationUtility.GetAnimationEvents(_clipExecutor.Clip!);
            Assert.AreEqual(0, events.Length, "All events should be cleared");
        }

        // ── Multiple modifications ───────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_MultipleModifications_AllApplied()
        {
            var animRef = new AssetObjectRef(TestClipPath);
            var mods = new[]
            {
                new AnimationModification { type = ModificationType.SetFrameRate, frameRate = 30f },
                new AnimationModification { type = ModificationType.SetWrapMode, wrapMode = WrapMode.PingPong },
                new AnimationModification
                {
                    type = ModificationType.SetCurve,
                    relativePath = "",
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

            var clip = _clipExecutor.Clip!;
            Assert.AreEqual(30f, clip.frameRate, 0.001f);
            Assert.AreEqual(WrapMode.PingPong, clip.wrapMode);
            var bindings = AnimationUtility.GetCurveBindings(clip);
            Assert.IsTrue(bindings.Any(b => b.propertyName == "m_LocalPosition.x"));
            var events = AnimationUtility.GetAnimationEvents(clip);
            Assert.AreEqual(1, events.Length);
            Assert.AreEqual("OnStart", events[0].functionName);
        }

        [Test]
        public void ModifyAnimationClip_MultipleModificationsWithOneInvalid_AppliesValidAndCollectsErrors()
        {
            var animRef = new AssetObjectRef(TestClipPath);
            var mods = new[]
            {
                new AnimationModification { type = ModificationType.SetFrameRate, frameRate = 24f },
                new AnimationModification { type = ModificationType.SetWrapMode }, // Missing value - error
                new AnimationModification { type = ModificationType.SetLegacy, legacy = true }
            };

            var response = AnimationTools.ModifyAnimationClip(animRef, mods);

            Assert.IsNotNull(response.errors);
            Assert.AreEqual(1, response.errors!.Count, "Exactly one modification should fail");

            var clip = _clipExecutor.Clip!;
            Assert.AreEqual(24f, clip.frameRate, 0.001f, "Valid SetFrameRate should apply");
            Assert.IsTrue(clip.legacy, "Valid SetLegacy should apply");
        }

        [Test]
        public void ModifyAnimationClip_Response_ContainsModifiedAssetInfo()
        {
            var animRef = new AssetObjectRef(TestClipPath);
            var mods = new[] { new AnimationModification { type = ModificationType.ClearCurves } };

            var response = AnimationTools.ModifyAnimationClip(animRef, mods);

            Assert.IsNotNull(response.modifiedAsset);
            Assert.AreEqual(TestClipPath, response.modifiedAsset!.path);
            Assert.AreEqual("TestClip", response.modifiedAsset.name);
            Assert.NotZero(response.modifiedAsset.instanceId);
        }
    }
}
