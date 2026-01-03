#nullable enable

using System;
using System.Collections.Generic;
using com.IvanMurzak.Unity.MCP.Animation;
using com.IvanMurzak.Unity.MCP.Editor.Tests;
using com.IvanMurzak.Unity.MCP.Editor.Tests.Utils;
using com.IvanMurzak.Unity.MCP.Runtime.Utils;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    public class AnimationClipToolExecutionTests : BaseTest
    {
        [Test]
        public void CreateAnimationClip_GetData_Modify_Succeeds()
        {
            MainThreadInstaller.Init();

            var folderName = $"Unity-MCP-Animation-{Guid.NewGuid():N}";
            var folder = new CreateFolderExecutor("Assets", folderName);
            var clipPath = $"{folder.FolderPath}/Clip.anim";

            var createJson = JsonTestUtils.Fill(@"{
                ""sourcePaths"": [""{clipPath}""]
            }", new Dictionary<string, object?>
            {
                { "{clipPath}", clipPath }
            });

            var getDataExecutor = new DynamicCallToolExecutor(
                typeof(AnimationTools).GetMethod(nameof(AnimationTools.GetData))!,
                () => JsonTestUtils.Fill(@"{
                    ""animRef"": {
                        ""assetPath"": ""{clipPath}""
                    }
                }", new Dictionary<string, object?>
                {
                    { "{clipPath}", clipPath }
                }));

            var modifyExecutor = new DynamicCallToolExecutor(
                typeof(AnimationTools).GetMethod(nameof(AnimationTools.ModifyAnimationClip))!,
                () => JsonTestUtils.Fill(@"{
                    ""animRef"": {
                        ""assetPath"": ""{clipPath}""
                    },
                    ""modifications"": [
                        {
                            ""type"": ""SetCurve"",
                            ""componentType"": ""UnityEngine.Transform"",
                            ""propertyName"": ""localPosition.x"",
                            ""keyframes"": [
                                { ""time"": 0.0, ""value"": 0.0 },
                                { ""time"": 0.5, ""value"": 1.0 }
                            ]
                        },
                        {
                            ""type"": ""RemoveCurve"",
                            ""componentType"": ""UnityEngine.Transform"",
                            ""propertyName"": ""localPosition.x""
                        },
                        { ""type"": ""ClearCurves"" },
                        { ""type"": ""SetFrameRate"", ""frameRate"": 30 },
                        { ""type"": ""SetWrapMode"", ""wrapMode"": ""Loop"" },
                        { ""type"": ""SetLegacy"", ""legacy"": true },
                        { ""type"": ""AddEvent"", ""time"": 0.1, ""functionName"": ""OnTestEvent"" },
                        { ""type"": ""ClearEvents"" }
                    ]
                }", new Dictionary<string, object?>
                {
                    { "{clipPath}", clipPath }
                }));

            folder
                .AddChild(new CallToolExecutor(
                    typeof(AnimationTools).GetMethod(nameof(AnimationTools.CreateAnimationClips))!, createJson))
                .AddChild(new ValidateToolResultExecutor())
                .AddChild(getDataExecutor)
                .AddChild(new ValidateToolResultExecutor())
                .AddChild(modifyExecutor)
                .AddChild(new ValidateToolResultExecutor())
                .AddChild(() =>
                {
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
                    Assert.IsNotNull(clip);
                    Assert.AreEqual(30f, clip!.frameRate);
                    Assert.AreEqual(WrapMode.Loop, clip.wrapMode);
                    Assert.IsTrue(clip.legacy);
                })
                .Execute();
        }
    }
}
