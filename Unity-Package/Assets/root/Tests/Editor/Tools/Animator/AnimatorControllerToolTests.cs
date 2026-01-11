/*
┌──────────────────────────────────────────────────────────────────┐
│  Author: Tristyn Mackay (https://github.com/Tristyn-InMetaTech)  │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-MCP)    │
│  Copyright (c) 2025 Ivan Murzak                                  │
│  Licensed under the Apache License, Version 2.0.                 │
│  See the LICENSE file in the project root for more information.  │
└──────────────────────────────────────────────────────────────────┘
*/

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using com.IvanMurzak.Unity.MCP.Animation;
using com.IvanMurzak.Unity.MCP.Editor.Tests;
using com.IvanMurzak.Unity.MCP.Editor.Tests.Utils;
using com.IvanMurzak.Unity.MCP.Runtime.Utils;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    public partial class AnimatorControllerToolTests : BaseTest
    {
        [Test]
        public void AnimatorController_Create()
        {
            MainThreadInstaller.Init();

            var folderName = $"Unity-MCP-Animator-{Guid.NewGuid():N}";
            var folder = new CreateFolderExecutor("Assets", folderName);
            var assetPath = $"{folder.FolderPath}/Test.controller";

            var json = JsonTestUtils.Fill(@"{
                ""sourcePaths"": [""{assetPath}""]
            }", new Dictionary<string, object?>
            {
                { "{assetPath}", assetPath }
            });

            folder
                .AddChild(new CallToolExecutor(
                    typeof(AnimatorTools).GetMethod(nameof(AnimatorTools.CreateAnimatorControllers))!, json))
                .AddChild(new ValidateToolResultExecutor())
                .AddChild(() =>
                {
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
                    Assert.IsNotNull(controller, $"Expected AnimatorController at {assetPath}");
                })
                .Execute();
        }

        [Test]
        public void AnimatorController_ModifyAndGetData()
        {
            MainThreadInstaller.Init();

            var folderName = $"Unity-MCP-Animator-Modify-{Guid.NewGuid():N}";
            var folder = new CreateFolderExecutor("Assets", folderName);
            var controllerPath = $"{folder.FolderPath}/Animator.controller";
            var clipPath = $"{folder.FolderPath}/Motion.anim";
            const string layerName = "McpLayer";
            const string parameterName = "Speed";

            var createControllerJson = JsonTestUtils.Fill(@"{
                ""sourcePaths"": [""{controllerPath}""]
            }", new Dictionary<string, object?>
            {
                { "{controllerPath}", controllerPath }
            });

            var createClipJson = JsonTestUtils.Fill(@"{
                ""sourcePaths"": [""{clipPath}""]
            }", new Dictionary<string, object?>
            {
                { "{clipPath}", clipPath }
            });

            var modifyExecutor = new DynamicCallToolExecutor(
                typeof(AnimatorTools).GetMethod(nameof(AnimatorTools.ModifyAnimatorController))!,
                () => JsonTestUtils.Fill(@"{
                    ""animatorRef"": {
                        ""assetPath"": ""{controllerPath}""
                    },
                    ""modifications"": [
                        {
                            ""type"": ""AddParameter"",
                            ""parameterName"": ""Speed"",
                            ""parameterType"": ""Float"",
                            ""defaultFloat"": 0.5
                        },
                        { ""type"": ""AddLayer"", ""layerName"": ""McpLayer"" },
                        { ""type"": ""AddState"", ""layerName"": ""McpLayer"", ""stateName"": ""Idle"" },
                        { ""type"": ""AddState"", ""layerName"": ""McpLayer"", ""stateName"": ""Run"" },
                        { ""type"": ""SetDefaultState"", ""layerName"": ""McpLayer"", ""stateName"": ""Idle"" },
                        {
                            ""type"": ""SetStateMotion"",
                            ""layerName"": ""McpLayer"",
                            ""stateName"": ""Idle"",
                            ""motionAssetPath"": ""{clipPath}""
                        },
                        {
                            ""type"": ""SetStateSpeed"",
                            ""layerName"": ""McpLayer"",
                            ""stateName"": ""Idle"",
                            ""speed"": 1.2
                        },
                        {
                            ""type"": ""AddTransition"",
                            ""layerName"": ""McpLayer"",
                            ""sourceStateName"": ""Idle"",
                            ""destinationStateName"": ""Run"",
                            ""hasExitTime"": true,
                            ""exitTime"": 0.25
                        },
                        {
                            ""type"": ""AddAnyStateTransition"",
                            ""layerName"": ""McpLayer"",
                            ""destinationStateName"": ""Run"",
                            ""duration"": 0.1
                        },
                        {
                            ""type"": ""RemoveTransition"",
                            ""layerName"": ""McpLayer"",
                            ""sourceStateName"": ""Idle"",
                            ""destinationStateName"": ""Run""
                        },
                        { ""type"": ""RemoveState"", ""layerName"": ""McpLayer"", ""stateName"": ""Run"" },
                        { ""type"": ""RemoveLayer"", ""layerName"": ""McpLayer"" },
                        { ""type"": ""RemoveParameter"", ""parameterName"": ""Speed"" }
                    ]
                }", new Dictionary<string, object?>
                {
                    { "{controllerPath}", controllerPath },
                    { "{clipPath}", clipPath }
                }));

            var getDataExecutor = new DynamicCallToolExecutor(
                typeof(AnimatorTools).GetMethod(nameof(AnimatorTools.GetData))!,
                () => JsonTestUtils.Fill(@"{
                    ""animatorRef"": {
                        ""assetPath"": ""{controllerPath}""
                    }
                }", new Dictionary<string, object?>
                {
                    { "{controllerPath}", controllerPath }
                }));

            folder
                .AddChild(new CallToolExecutor(
                    typeof(AnimatorTools).GetMethod(nameof(AnimatorTools.CreateAnimatorControllers))!, createControllerJson))
                .AddChild(new ValidateToolResultExecutor())
                .AddChild(new CallToolExecutor(
                    typeof(AnimationTools).GetMethod(nameof(AnimationTools.CreateAnimationClips))!, createClipJson))
                .AddChild(new ValidateToolResultExecutor())
                .AddChild(modifyExecutor)
                .AddChild(new ValidateToolResultExecutor())
                .AddChild(getDataExecutor)
                .AddChild(new ValidateToolResultExecutor())
                .AddChild(() =>
                {
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
                    Assert.IsNotNull(controller);
                    Assert.IsFalse(controller!.layers.Any(layer => layer.name == layerName));
                    Assert.IsFalse(controller.parameters.Any(param => param.name == parameterName));
                })
                .Execute();
        }
    }
}
