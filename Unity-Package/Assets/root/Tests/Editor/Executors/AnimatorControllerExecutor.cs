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
using com.IvanMurzak.Unity.MCP.Editor.Tests.Utils;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    /// <summary>
    /// Test executor that manages an AnimatorController asset lifecycle via LazyNodeExecutor.
    /// Creates the controller when SetAction is called, deletes it in PostExecute.
    /// </summary>
    public class AnimatorControllerExecutor : LazyNodeExecutor
    {
        public string AssetPath { get; }
        public AnimatorController? Controller => AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetPath);

        public AnimatorControllerExecutor(string assetPath)
        {
            AssetPath = assetPath;
            SetAction(() => CreateController());
        }

        private void CreateController()
        {
            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetPath) == null)
            {
                AnimatorController.CreateAnimatorControllerAtPath(AssetPath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }

        protected override void PostExecute(object? input = null)
        {
            DeleteController();
            base.PostExecute(input);
        }

        private void DeleteController()
        {
            if (!string.IsNullOrEmpty(AssetPath) && AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetPath) != null)
            {
                AssetDatabase.DeleteAsset(AssetPath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
    }
}
