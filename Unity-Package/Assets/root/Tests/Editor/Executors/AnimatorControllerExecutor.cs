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
using UnityEditor.Animations;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    /// <summary>
    /// Test executor that creates an AnimatorController asset for testing and cleans it up afterward.
    /// Usage: Create in SetUp, call Teardown in TearDown.
    /// </summary>
    public class AnimatorControllerExecutor
    {
        public string AssetPath { get; }
        public AnimatorController? Controller => AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetPath);

        public AnimatorControllerExecutor(string assetPath)
        {
            AssetPath = assetPath;
        }

        /// <summary>Creates the AnimatorController asset on disk and returns self for chaining.</summary>
        public AnimatorControllerExecutor Setup()
        {
            var directory = Path.GetDirectoryName(AssetPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }

            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetPath) == null)
            {
                AnimatorController.CreateAnimatorControllerAtPath(AssetPath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }

            return this;
        }

        /// <summary>Deletes the AnimatorController asset from disk.</summary>
        public void Teardown()
        {
            if (!string.IsNullOrEmpty(AssetPath) && AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetPath) != null)
            {
                AssetDatabase.DeleteAsset(AssetPath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
    }
}
