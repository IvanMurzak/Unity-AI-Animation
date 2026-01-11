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
    public partial class AnimatorToolTests : BaseTest
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
    }
}
