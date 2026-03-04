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
using com.IvanMurzak.Unity.MCP.Editor.Tests.Utils;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    [TestFixture]
    public class AnimationCreate_Tests
    {
        [Test]
        public void CreateAnimationClips_NullPaths_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                AnimationTools.CreateAnimationClips(null!));
        }

        [Test]
        public void CreateAnimationClips_EmptyPathsArray_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                AnimationTools.CreateAnimationClips(Array.Empty<string>()));
        }

        [Test]
        public void CreateAnimationClips_ValidPath_CreatesAssetAndReturnsInfo()
        {
            var folderEx = new CreateFolderExecutor("Assets", "Tests");
            folderEx.AddChild(() =>
            {
                var assetPath = $"{folderEx.FolderPath}/TestClip.anim";

                var response = AnimationTools.CreateAnimationClips(new[] { assetPath });

                Assert.IsNotNull(response);
                Assert.IsNull(response.errors, "Expected no errors for valid path");
                Assert.IsNotNull(response.createdAssets);
                Assert.AreEqual(1, response.createdAssets!.Count);
                Assert.AreEqual(assetPath, response.createdAssets[0].path);
                Assert.AreEqual("TestClip", response.createdAssets[0].name);
                Assert.NotZero(response.createdAssets[0].instanceId);

                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
                Assert.IsNotNull(clip, "Asset should exist at the given path");
            }).Execute();
        }

        [Test]
        public void CreateAnimationClips_MultiplePaths_CreatesAllAssets()
        {
            var folderEx = new CreateFolderExecutor("Assets", "Tests");
            folderEx.AddChild(() =>
            {
                var paths = new[]
                {
                    $"{folderEx.FolderPath}/Clip1.anim",
                    $"{folderEx.FolderPath}/Clip2.anim",
                    $"{folderEx.FolderPath}/Clip3.anim"
                };

                var response = AnimationTools.CreateAnimationClips(paths);

                Assert.IsNotNull(response);
                Assert.IsNull(response.errors, "Expected no errors for valid paths");
                Assert.IsNotNull(response.createdAssets);
                Assert.AreEqual(3, response.createdAssets!.Count);

                foreach (var path in paths)
                {
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                    Assert.IsNotNull(clip, $"Asset should exist at {path}");
                }
            }).Execute();
        }

        [Test]
        public void CreateAnimationClips_PathWithoutAssetsPrefix_ReturnsError()
        {
            var response = AnimationTools.CreateAnimationClips(new[] { "SomeFolder/TestClip.anim" });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.errors);
            Assert.AreEqual(1, response.errors!.Count);
            Assert.IsNull(response.createdAssets);
        }

        [Test]
        public void CreateAnimationClips_PathWithoutAnimExtension_ReturnsError()
        {
            var response = AnimationTools.CreateAnimationClips(new[] { "Assets/Tests/TestClip.txt" });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.errors);
            Assert.AreEqual(1, response.errors!.Count);
            Assert.IsNull(response.createdAssets);
        }

        [Test]
        public void CreateAnimationClips_EmptyStringPath_ReturnsError()
        {
            var response = AnimationTools.CreateAnimationClips(new[] { string.Empty });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.errors);
            Assert.AreEqual(1, response.errors!.Count);
            Assert.IsNull(response.createdAssets);
        }

        [Test]
        public void CreateAnimationClips_NestedFolderPath_CreatesFoldersAndAsset()
        {
            var folderEx = new CreateFolderExecutor("Assets", "Tests", "SubFolder", "Nested");
            folderEx.AddChild(() =>
            {
                var assetPath = $"{folderEx.FolderPath}/DeepClip.anim";

                var response = AnimationTools.CreateAnimationClips(new[] { assetPath });

                Assert.IsNotNull(response);
                Assert.IsNull(response.errors, "Expected no errors for nested path");
                Assert.IsNotNull(response.createdAssets);
                Assert.AreEqual(1, response.createdAssets!.Count);

                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
                Assert.IsNotNull(clip, "Asset should exist at nested path");
            }).Execute();
        }

        [Test]
        public void CreateAnimationClips_MixedValidAndInvalid_CreatesValidReturnsErrorsForInvalid()
        {
            var folderEx = new CreateFolderExecutor("Assets", "Tests");
            folderEx.AddChild(() =>
            {
                var validPath = $"{folderEx.FolderPath}/ValidClip.anim";

                var response = AnimationTools.CreateAnimationClips(new[]
                {
                    validPath,
                    "BadPath/NoPrefixClip.anim",
                    $"{folderEx.FolderPath}/WrongExtension.txt",
                    string.Empty
                });

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.createdAssets, "Should have created the valid asset");
                Assert.AreEqual(1, response.createdAssets!.Count);
                Assert.AreEqual(validPath, response.createdAssets[0].path);

                Assert.IsNotNull(response.errors, "Should have errors for the invalid paths");
                Assert.AreEqual(3, response.errors!.Count);
            }).Execute();
        }

        [Test]
        public void CreateAnimationClips_PathAlreadyExists_OverwritesAndReturnsAssetInfo()
        {
            var folderEx = new CreateFolderExecutor("Assets", "Tests");
            folderEx.AddChild(() =>
            {
                var assetPath = $"{folderEx.FolderPath}/ExistingClip.anim";

                // Create once
                var response1 = AnimationTools.CreateAnimationClips(new[] { assetPath });
                Assert.IsNull(response1.errors);
                var originalInstanceId = response1.createdAssets![0].instanceId;

                // Create again - should overwrite and succeed
                var response2 = AnimationTools.CreateAnimationClips(new[] { assetPath });
                Assert.IsNotNull(response2);
                Assert.IsNull(response2.errors, "Overwriting an existing asset should not produce errors");
                Assert.IsNotNull(response2.createdAssets);
                Assert.AreEqual(1, response2.createdAssets!.Count);
                Assert.AreEqual(assetPath, response2.createdAssets[0].path);
                Assert.NotZero(response2.createdAssets[0].instanceId);

                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
                Assert.IsNotNull(clip, "Asset should still exist after overwrite");
            }).Execute();
        }
    }
}
