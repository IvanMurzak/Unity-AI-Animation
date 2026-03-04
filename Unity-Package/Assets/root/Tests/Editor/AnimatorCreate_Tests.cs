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
using UnityEditor.Animations;
using com.IvanMurzak.Unity.MCP.Editor.Tests.Utils;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    [TestFixture]
    public class AnimatorCreate_Tests
    {
        [Test]
        public void CreateAnimatorControllers_NullPaths_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                AnimatorTools.CreateAnimatorControllers(null!));
        }

        [Test]
        public void CreateAnimatorControllers_EmptyPathsArray_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                AnimatorTools.CreateAnimatorControllers(Array.Empty<string>()));
        }

        [Test]
        public void CreateAnimatorControllers_ValidPath_CreatesAssetAndReturnsInfo()
        {
            var folderEx = new CreateFolderExecutor("Assets", "Tests");
            folderEx.AddChild(() =>
            {
                var assetPath = $"{folderEx.FolderPath}/TestController.controller";

                var response = AnimatorTools.CreateAnimatorControllers(new[] { assetPath });

                Assert.IsNotNull(response);
                Assert.IsNull(response.errors, "Expected no errors for valid path");
                Assert.IsNotNull(response.createdAssets);
                Assert.AreEqual(1, response.createdAssets!.Count);
                Assert.AreEqual(assetPath, response.createdAssets[0].path);
                Assert.AreEqual("TestController", response.createdAssets[0].name);
                Assert.NotZero(response.createdAssets[0].instanceId);

                var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
                Assert.IsNotNull(controller, "AnimatorController should exist at given path");
            }).Execute();
        }

        [Test]
        public void CreateAnimatorControllers_MultiplePaths_CreatesAllAssets()
        {
            var folderEx = new CreateFolderExecutor("Assets", "Tests");
            folderEx.AddChild(() =>
            {
                var paths = new[]
                {
                    $"{folderEx.FolderPath}/Controller1.controller",
                    $"{folderEx.FolderPath}/Controller2.controller",
                    $"{folderEx.FolderPath}/Controller3.controller"
                };

                var response = AnimatorTools.CreateAnimatorControllers(paths);

                Assert.IsNotNull(response);
                Assert.IsNull(response.errors, "Expected no errors");
                Assert.IsNotNull(response.createdAssets);
                Assert.AreEqual(3, response.createdAssets!.Count);

                foreach (var path in paths)
                    Assert.IsNotNull(AssetDatabase.LoadAssetAtPath<AnimatorController>(path), $"Asset should exist at {path}");
            }).Execute();
        }

        [Test]
        public void CreateAnimatorControllers_PathWithoutAssetsPrefix_ReturnsError()
        {
            var response = AnimatorTools.CreateAnimatorControllers(new[] { "NoPrefix/TestController.controller" });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.errors);
            Assert.AreEqual(1, response.errors!.Count);
            Assert.IsNull(response.createdAssets);
        }

        [Test]
        public void CreateAnimatorControllers_PathWithoutControllerExtension_ReturnsError()
        {
            var response = AnimatorTools.CreateAnimatorControllers(new[] { "Assets/Tests/TestController.txt" });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.errors);
            Assert.AreEqual(1, response.errors!.Count);
            Assert.IsNull(response.createdAssets);
        }

        [Test]
        public void CreateAnimatorControllers_EmptyStringPath_ReturnsError()
        {
            var response = AnimatorTools.CreateAnimatorControllers(new[] { string.Empty });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.errors);
            Assert.AreEqual(1, response.errors!.Count);
            Assert.IsNull(response.createdAssets);
        }

        [Test]
        public void CreateAnimatorControllers_NestedFolderPath_CreatesFoldersAndAsset()
        {
            var folderEx = new CreateFolderExecutor("Assets", "Tests", "SubFolder", "Deep");
            folderEx.AddChild(() =>
            {
                var assetPath = $"{folderEx.FolderPath}/Controller.controller";

                var response = AnimatorTools.CreateAnimatorControllers(new[] { assetPath });

                Assert.IsNotNull(response);
                Assert.IsNull(response.errors);
                Assert.IsNotNull(response.createdAssets);
                Assert.AreEqual(1, response.createdAssets!.Count);

                var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
                Assert.IsNotNull(controller, "Controller should exist at nested path");
            }).Execute();
        }

        [Test]
        public void CreateAnimatorControllers_MixedValidAndInvalid_CreatesValidReturnsErrorsForInvalid()
        {
            var folderEx = new CreateFolderExecutor("Assets", "Tests");
            folderEx.AddChild(() =>
            {
                var validPath = $"{folderEx.FolderPath}/ValidController.controller";

                var response = AnimatorTools.CreateAnimatorControllers(new[]
                {
                    validPath,
                    "NoPrefixController.controller",
                    $"{folderEx.FolderPath}/WrongExt.anim",
                    string.Empty
                });

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.createdAssets);
                Assert.AreEqual(1, response.createdAssets!.Count);
                Assert.AreEqual(validPath, response.createdAssets[0].path);

                Assert.IsNotNull(response.errors);
                Assert.AreEqual(3, response.errors!.Count);
            }).Execute();
        }

        [Test]
        public void CreateAnimatorControllers_NewController_HasDefaultBaseLayer()
        {
            var folderEx = new CreateFolderExecutor("Assets", "Tests");
            folderEx.AddChild(() =>
            {
                var assetPath = $"{folderEx.FolderPath}/NewController.controller";

                AnimatorTools.CreateAnimatorControllers(new[] { assetPath });

                var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
                Assert.IsNotNull(controller);
                Assert.GreaterOrEqual(controller!.layers.Length, 1, "New controller should have at least Base Layer");
            }).Execute();
        }
    }
}
