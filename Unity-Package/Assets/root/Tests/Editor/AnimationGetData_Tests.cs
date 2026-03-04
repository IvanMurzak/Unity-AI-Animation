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
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using com.IvanMurzak.Unity.MCP.Runtime.Data;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    [TestFixture]
    public class AnimationGetData_Tests
    {
        [Test]
        public void GetData_NullRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                AnimationTools.GetData(null!));
        }

        [Test]
        public void GetData_InvalidRef_ThrowsArgumentException()
        {
            // AssetObjectRef with no InstanceID, no path, no guid - IsValid() returns false
            var invalidRef = new AssetObjectRef();

            Assert.Throws<ArgumentException>(() =>
                AnimationTools.GetData(invalidRef));
        }

        [Test]
        public void GetData_NonExistentAsset_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                AnimationTools.GetData(new AssetObjectRef("Assets/Tests/NonExistent.anim")));
        }

        [Test]
        public void GetData_ValidEmptyClip_ReturnsBasicData()
        {
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var response = AnimationTools.GetData(animRef);

                Assert.IsNotNull(response);
                Assert.AreEqual("TestClip", response.name);
                Assert.GreaterOrEqual(response.frameRate, 0f);
                Assert.IsNotNull(response.wrapMode);
                Assert.IsNotNull(response.curveBindings);
                Assert.IsNotNull(response.objectReferenceBindings);
                Assert.IsNotNull(response.events);
                Assert.IsTrue(response.empty, "New empty clip should be reported as empty");
            }).Execute();
        }

        [Test]
        public void GetData_ClipWithCustomFrameRate_ReturnsCorrectFrameRate()
        {
            var clipEx = new CreateAnimationClipExecutor("animation.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var clip = clipEx.Asset ?? throw new InvalidOperationException("Clip should have been created by executor");
                clip.frameRate = 120f;
                EditorUtility.SetDirty(clip);
                AssetDatabase.SaveAssets();

                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var response = AnimationTools.GetData(animRef);

                Assert.AreEqual(120f, response.frameRate, 0.001f);
            }).Execute();
        }

        [Test]
        public void GetData_ClipWithWrapMode_ReturnsCorrectWrapMode()
        {
            var clipEx = new CreateAnimationClipExecutor("animation.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var clip = clipEx.Asset ?? throw new InvalidOperationException("Clip should have been created by executor");
                clip.wrapMode = WrapMode.Loop;
                EditorUtility.SetDirty(clip);
                AssetDatabase.SaveAssets();

                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var response = AnimationTools.GetData(animRef);

                Assert.AreEqual("Loop", response.wrapMode);
            }).Execute();
        }

        [Test]
        public void GetData_ClipWithCurve_ReturnsCurveBindings()
        {
            var clipEx = new CreateAnimationClipExecutor("animation.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var clip = clipEx.Asset ?? throw new InvalidOperationException("Clip should have been created by executor");
                var curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
                clip.SetCurve(string.Empty, typeof(Transform), "localPosition.x", curve);
                EditorUtility.SetDirty(clip);
                AssetDatabase.SaveAssets();

                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var response = AnimationTools.GetData(animRef);

                Assert.IsNotNull(response.curveBindings);
                Assert.GreaterOrEqual(response.curveBindings!.Count, 1);
                var binding = response.curveBindings[0];
                Assert.AreEqual("m_LocalPosition.x", binding.propertyName);
                Assert.AreEqual(2, binding.keyframeCount);
            }).Execute();
        }

        [Test]
        public void GetData_ClipWithEvent_ReturnsEventData()
        {
            var clipEx = new CreateAnimationClipExecutor("animation.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var clip = clipEx.Asset ?? throw new InvalidOperationException("Clip should have been created by executor");
                var animEvent = new AnimationEvent
                {
                    time = 0.5f,
                    functionName = "OnTestEvent",
                    intParameter = 42,
                    floatParameter = 3.14f,
                    stringParameter = "hello"
                };
                AnimationUtility.SetAnimationEvents(clip, new[] { animEvent });
                EditorUtility.SetDirty(clip);
                AssetDatabase.SaveAssets();

                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var response = AnimationTools.GetData(animRef);

                Assert.IsNotNull(response.events);
                Assert.AreEqual(1, response.events!.Count);
                var evt = response.events[0];
                Assert.AreEqual(0.5f, evt.time, 0.001f);
                Assert.AreEqual("OnTestEvent", evt.functionName);
                Assert.AreEqual(42, evt.intParameter);
                Assert.AreEqual(3.14f, evt.floatParameter, 0.001f);
                Assert.AreEqual("hello", evt.stringParameter);
            }).Execute();
        }

        [Test]
        public void GetData_ClipWithLegacyFlag_ReturnsLegacyTrue()
        {
            var clipEx = new CreateAnimationClipExecutor("animation.anim", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var clip = clipEx.Asset ?? throw new InvalidOperationException("Clip should have been created by executor");
                clip.legacy = true;
                EditorUtility.SetDirty(clip);
                AssetDatabase.SaveAssets();

                var animRef = new AssetObjectRef(clipEx.AssetPath);
                var response = AnimationTools.GetData(animRef);

                Assert.IsTrue(response.legacy);
            }).Execute();
        }
    }
}
