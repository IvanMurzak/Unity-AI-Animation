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

using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    /// <summary>
    /// Test executor that creates an AnimationClip asset for testing and cleans it up afterward.
    /// Usage: Create in SetUp, call Teardown in TearDown.
    /// </summary>
    public class AnimationClipExecutor
    {
        public string AssetPath { get; }
        public AnimationClip? Clip => AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetPath);

        public AnimationClipExecutor(string assetPath)
        {
            AssetPath = assetPath;
        }

        /// <summary>Creates the AnimationClip asset on disk and returns self for chaining.</summary>
        public AnimationClipExecutor Setup()
        {
            var directory = Path.GetDirectoryName(AssetPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }

            if (AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetPath) == null)
            {
                var clip = new AnimationClip { name = Path.GetFileNameWithoutExtension(AssetPath) };
                AssetDatabase.CreateAsset(clip, AssetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }

            return this;
        }

        /// <summary>Deletes the AnimationClip asset from disk.</summary>
        public void Teardown()
        {
            if (!string.IsNullOrEmpty(AssetPath) && AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetPath) != null)
            {
                AssetDatabase.DeleteAsset(AssetPath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
    }
}
