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
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    [TestFixture]
    public class AnimatorCreate_Tests
    {
        private const string TestFolder = "Assets/Tests/MCP/Animator/CreateTests";
        private readonly List<string> _createdPaths = new();

        [TearDown]
        public void TearDown()
        {
            foreach (var path in _createdPaths)
            {
                if (!string.IsNullOrEmpty(path) && AssetDatabase.LoadAssetAtPath<AnimatorController>(path) != null)
                    AssetDatabase.DeleteAsset(path);
            }
            _createdPaths.Clear();

            if (AssetDatabase.IsValidFolder(TestFolder))
            {
                AssetDatabase.DeleteAsset(TestFolder);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }

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
            var assetPath = $"{TestFolder}/TestController.controller";
            _createdPaths.Add(assetPath);

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
        }

        [Test]
        public void CreateAnimatorControllers_MultiplePaths_CreatesAllAssets()
        {
            var paths = new[]
            {
                $"{TestFolder}/Controller1.controller",
                $"{TestFolder}/Controller2.controller",
                $"{TestFolder}/Controller3.controller"
            };
            foreach (var p in paths) _createdPaths.Add(p);

            var response = AnimatorTools.CreateAnimatorControllers(paths);

            Assert.IsNotNull(response);
            Assert.IsNull(response.errors, "Expected no errors");
            Assert.IsNotNull(response.createdAssets);
            Assert.AreEqual(3, response.createdAssets!.Count);

            foreach (var path in paths)
                Assert.IsNotNull(AssetDatabase.LoadAssetAtPath<AnimatorController>(path), $"Asset should exist at {path}");
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
            var response = AnimatorTools.CreateAnimatorControllers(new[] { $"{TestFolder}/TestController.txt" });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.errors);
            Assert.AreEqual(1, response.errors!.Count);
            Assert.IsNull(response.createdAssets);
        }

        [Test]
        public void CreateAnimatorControllers_EmptyStringPath_ReturnsError()
        {
            var response = AnimatorTools.CreateAnimatorControllers(new[] { "" });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.errors);
            Assert.AreEqual(1, response.errors!.Count);
            Assert.IsNull(response.createdAssets);
        }

        [Test]
        public void CreateAnimatorControllers_NestedFolderPath_CreatesFoldersAndAsset()
        {
            var assetPath = $"{TestFolder}/SubFolder/Deep/Controller.controller";
            _createdPaths.Add(assetPath);

            var response = AnimatorTools.CreateAnimatorControllers(new[] { assetPath });

            Assert.IsNotNull(response);
            Assert.IsNull(response.errors);
            Assert.IsNotNull(response.createdAssets);
            Assert.AreEqual(1, response.createdAssets!.Count);

            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
            Assert.IsNotNull(controller, "Controller should exist at nested path");
        }

        [Test]
        public void CreateAnimatorControllers_MixedValidAndInvalid_CreatesValidReturnsErrorsForInvalid()
        {
            var validPath = $"{TestFolder}/ValidController.controller";
            _createdPaths.Add(validPath);

            var response = AnimatorTools.CreateAnimatorControllers(new[]
            {
                validPath,
                "NoPrefixController.controller",
                $"{TestFolder}/WrongExt.anim",
                ""
            });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.createdAssets);
            Assert.AreEqual(1, response.createdAssets!.Count);
            Assert.AreEqual(validPath, response.createdAssets[0].path);

            Assert.IsNotNull(response.errors);
            Assert.AreEqual(3, response.errors!.Count);
        }

        [Test]
        public void CreateAnimatorControllers_NewController_HasDefaultBaseLayer()
        {
            var assetPath = $"{TestFolder}/NewController.controller";
            _createdPaths.Add(assetPath);

            AnimatorTools.CreateAnimatorControllers(new[] { assetPath });

            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
            Assert.IsNotNull(controller);
            Assert.GreaterOrEqual(controller!.layers.Length, 1, "New controller should have at least Base Layer");
        }
    }
}
