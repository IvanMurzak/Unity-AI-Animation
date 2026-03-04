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
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    [TestFixture]
    public class AnimationCreate_Tests
    {
        private const string TestFolder = "Assets/Tests/MCP/Animation/CreateTests";
        private readonly List<string> _createdPaths = new();

        [TearDown]
        public void TearDown()
        {
            foreach (var path in _createdPaths)
            {
                if (!string.IsNullOrEmpty(path) && AssetDatabase.LoadAssetAtPath<AnimationClip>(path) != null)
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
            var assetPath = $"{TestFolder}/TestClip.anim";
            _createdPaths.Add(assetPath);

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
        }

        [Test]
        public void CreateAnimationClips_MultiplePaths_CreatesAllAssets()
        {
            var paths = new[]
            {
                $"{TestFolder}/Clip1.anim",
                $"{TestFolder}/Clip2.anim",
                $"{TestFolder}/Clip3.anim"
            };
            foreach (var p in paths) _createdPaths.Add(p);

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
            var response = AnimationTools.CreateAnimationClips(new[] { $"{TestFolder}/TestClip.txt" });

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
            var assetPath = $"{TestFolder}/SubFolder/Nested/DeepClip.anim";
            _createdPaths.Add(assetPath);

            var response = AnimationTools.CreateAnimationClips(new[] { assetPath });

            Assert.IsNotNull(response);
            Assert.IsNull(response.errors, "Expected no errors for nested path");
            Assert.IsNotNull(response.createdAssets);
            Assert.AreEqual(1, response.createdAssets!.Count);

            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
            Assert.IsNotNull(clip, "Asset should exist at nested path");
        }

        [Test]
        public void CreateAnimationClips_MixedValidAndInvalid_CreatesValidReturnsErrorsForInvalid()
        {
            var validPath = $"{TestFolder}/ValidClip.anim";
            _createdPaths.Add(validPath);

            var response = AnimationTools.CreateAnimationClips(new[]
            {
                validPath,
                "BadPath/NoPrefixClip.anim",
                $"{TestFolder}/WrongExtension.txt",
                string.Empty
            });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.createdAssets, "Should have created the valid asset");
            Assert.AreEqual(1, response.createdAssets!.Count);
            Assert.AreEqual(validPath, response.createdAssets[0].path);

            Assert.IsNotNull(response.errors, "Should have errors for the invalid paths");
            Assert.AreEqual(3, response.errors!.Count);
        }

        [Test]
        public void CreateAnimationClips_PathAlreadyExists_CreatesNewAsset()
        {
            var assetPath = $"{TestFolder}/ExistingClip.anim";
            _createdPaths.Add(assetPath);

            // Create once
            var response1 = AnimationTools.CreateAnimationClips(new[] { assetPath });
            Assert.IsNull(response1.errors);

            // Create again - should overwrite/succeed
            var response2 = AnimationTools.CreateAnimationClips(new[] { assetPath });
            Assert.IsNotNull(response2);
        }
    }
}
