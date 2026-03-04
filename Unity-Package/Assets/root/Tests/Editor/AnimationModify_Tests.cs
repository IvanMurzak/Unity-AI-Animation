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
using com.IvanMurzak.Unity.MCP.Editor.Tests.Utils;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    [TestFixture]
    public class AnimationModify_Tests
    {
        private const string TestFolder = "Assets/Tests/MCP/Animation/ModifyTests";
        private const string TestClipPath = TestFolder + "/TestClip.anim";

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
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animRef = new AssetObjectRef(TestClipPath);

                    Assert.Throws<ArgumentNullException>(() =>
                        AnimationTools.ModifyAnimationClip(animRef, null!));
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_EmptyModifications_ThrowsArgumentException()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animRef = new AssetObjectRef(TestClipPath);

                    Assert.Throws<ArgumentException>(() =>
                        AnimationTools.ModifyAnimationClip(animRef, Array.Empty<AnimationModification>()));
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_NonExistentAsset_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                AnimationTools.ModifyAnimationClip(new AssetObjectRef($"{TestFolder}/NonExistent.anim"), new[] { new AnimationModification { type = ModificationType.ClearCurves } }));
        }

        // ── SetCurve ────────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_SetCurve_Valid_CurveApplied()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animRef = new AssetObjectRef(TestClipPath);
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

                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
                    var bindings = AnimationUtility.GetCurveBindings(clip);
                    Assert.IsTrue(bindings.Any(b => b.propertyName == "m_LocalPosition.x"),
                        "Curve should be applied to clip");
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetCurve_MissingComponentType_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
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
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetCurve_MissingPropertyName_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
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
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetCurve_MissingKeyframes_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
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
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetCurve_InvalidComponentType_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
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
                }));
            folderExecutor.Execute();
        }

        // ── RemoveCurve ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_RemoveCurve_ExistingCurve_CurveRemoved()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    // First add a curve directly
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
                    clip.SetCurve(string.Empty, typeof(Transform), "m_LocalPosition.x", AnimationCurve.Linear(0f, 0f, 1f, 1f));
                    EditorUtility.SetDirty(clip);
                    AssetDatabase.SaveAssets();

                    var animRef = new AssetObjectRef(TestClipPath);
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

                    // Reload the clip to get the updated version
                    var reloadedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
                    var bindings = AnimationUtility.GetCurveBindings(reloadedClip);
                    Assert.IsFalse(bindings.Any(b => b.propertyName == "m_LocalPosition.x"),
                        "Curve should have been removed");
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_RemoveCurve_MissingComponentType_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
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
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_RemoveCurve_MissingPropertyName_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
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
                }));
            folderExecutor.Execute();
        }

        // ── ClearCurves ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_ClearCurves_RemovesAllCurves()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    // Add multiple curves
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
                    clip.SetCurve(string.Empty, typeof(Transform), "m_LocalPosition.x", AnimationCurve.Linear(0f, 0f, 1f, 1f));
                    clip.SetCurve(string.Empty, typeof(Transform), "m_LocalPosition.y", AnimationCurve.Linear(0f, 0f, 1f, 1f));
                    EditorUtility.SetDirty(clip);
                    AssetDatabase.SaveAssets();

                    var animRef = new AssetObjectRef(TestClipPath);
                    var mods = new[] { new AnimationModification { type = ModificationType.ClearCurves } };

                    var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                    Assert.IsNull(response.errors);
                    var reloadedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
                    var bindings = AnimationUtility.GetCurveBindings(reloadedClip);
                    Assert.AreEqual(0, bindings.Length, "All curves should be cleared");
                }));
            folderExecutor.Execute();
        }

        // ── SetFrameRate ────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_SetFrameRate_Valid_FrameRateUpdated()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animRef = new AssetObjectRef(TestClipPath);
                    var mods = new[]
                    {
                        new AnimationModification { type = ModificationType.SetFrameRate, frameRate = 120f }
                    };

                    var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                    Assert.IsNull(response.errors);
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
                    Assert.AreEqual(120f, clip.frameRate, 0.001f);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetFrameRate_MissingValue_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
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
                }));
            folderExecutor.Execute();
        }

        // ── SetWrapMode ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_SetWrapMode_Valid_WrapModeUpdated()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animRef = new AssetObjectRef(TestClipPath);
                    var mods = new[]
                    {
                        new AnimationModification { type = ModificationType.SetWrapMode, wrapMode = WrapMode.Loop }
                    };

                    var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                    Assert.IsNull(response.errors);
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
                    Assert.AreEqual(WrapMode.Loop, clip.wrapMode);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetWrapMode_MissingValue_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animRef = new AssetObjectRef(TestClipPath);
                    var mods = new[]
                    {
                        new AnimationModification { type = ModificationType.SetWrapMode }
                    };

                    var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                    Assert.IsNotNull(response.errors);
                    StringAssert.Contains("wrapMode", response.errors![0]);
                }));
            folderExecutor.Execute();
        }

        // ── SetLegacy ───────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_SetLegacy_True_LegacyFlagSet()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animRef = new AssetObjectRef(TestClipPath);
                    var mods = new[]
                    {
                        new AnimationModification { type = ModificationType.SetLegacy, legacy = true }
                    };

                    var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                    Assert.IsNull(response.errors);
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
                    Assert.IsTrue(clip.legacy);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetLegacy_False_LegacyFlagCleared()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    // Set legacy to true first
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
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
                    var reloadedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
                    Assert.IsFalse(reloadedClip.legacy);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_SetLegacy_MissingValue_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animRef = new AssetObjectRef(TestClipPath);
                    var mods = new[]
                    {
                        new AnimationModification { type = ModificationType.SetLegacy }
                    };

                    var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                    Assert.IsNotNull(response.errors);
                    StringAssert.Contains("legacy", response.errors![0]);
                }));
            folderExecutor.Execute();
        }

        // ── AddEvent ────────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_AddEvent_Valid_EventAdded()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
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

                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
                    var events = AnimationUtility.GetAnimationEvents(clip);
                    Assert.AreEqual(1, events.Length);
                    Assert.AreEqual(0.5f, events[0].time, 0.001f);
                    Assert.AreEqual("OnAnimationEvent", events[0].functionName);
                    Assert.AreEqual(7, events[0].intParameter);
                    Assert.AreEqual(2.5f, events[0].floatParameter, 0.001f);
                    Assert.AreEqual("test", events[0].stringParameter);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_AddEvent_MissingTime_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
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
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_AddEvent_MissingFunctionName_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
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
                }));
            folderExecutor.Execute();
        }

        // ── ClearEvents ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_ClearEvents_RemovesAllEvents()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
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
                    var reloadedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
                    var events = AnimationUtility.GetAnimationEvents(reloadedClip);
                    Assert.AreEqual(0, events.Length, "All events should be cleared");
                }));
            folderExecutor.Execute();
        }

        // ── Multiple modifications ───────────────────────────────────────────────

        [Test]
        public void ModifyAnimationClip_MultipleModifications_AllApplied()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animRef = new AssetObjectRef(TestClipPath);
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

                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
                    Assert.AreEqual(30f, clip.frameRate, 0.001f);
                    Assert.AreEqual(WrapMode.PingPong, clip.wrapMode);
                    var bindings = AnimationUtility.GetCurveBindings(clip);
                    Assert.IsTrue(bindings.Any(b => b.propertyName == "m_LocalPosition.x"));
                    var events = AnimationUtility.GetAnimationEvents(clip);
                    Assert.AreEqual(1, events.Length);
                    Assert.AreEqual("OnStart", events[0].functionName);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_MultipleModificationsWithOneInvalid_AppliesValidAndCollectsErrors()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
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

                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(TestClipPath);
                    Assert.AreEqual(24f, clip.frameRate, 0.001f, "Valid SetFrameRate should apply");
                    Assert.IsTrue(clip.legacy, "Valid SetLegacy should apply");
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimationClip_Response_ContainsModifiedAssetInfo()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animation", "ModifyTests");
            folderExecutor.Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animRef = new AssetObjectRef(TestClipPath);
                    var mods = new[] { new AnimationModification { type = ModificationType.ClearCurves } };

                    var response = AnimationTools.ModifyAnimationClip(animRef, mods);

                    Assert.IsNotNull(response.modifiedAsset);
                    Assert.AreEqual(TestClipPath, response.modifiedAsset!.path);
                    Assert.AreEqual("TestClip", response.modifiedAsset.name);
                    Assert.NotZero(response.modifiedAsset.instanceId);
                }));
            folderExecutor.Execute();
        }
    }
}
