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
using com.IvanMurzak.Unity.MCP.Editor.Tests.Utils;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    /// <summary>
    /// Test executor that manages an AnimationClip asset lifecycle via LazyNodeExecutor.
    /// Creates the clip when SetAction is called, deletes it in PostExecute.
    /// </summary>
    public class AnimationClipExecutor : LazyNodeExecutor
    {
        public string AssetPath { get; }
        public AnimationClip? Clip => AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetPath);

        public AnimationClipExecutor(string assetPath)
        {
            AssetPath = assetPath;
            SetAction(() => CreateClip());
        }

        private void CreateClip()
        {
            if (AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetPath) == null)
            {
                var clip = new AnimationClip { name = Path.GetFileNameWithoutExtension(AssetPath) };
                AssetDatabase.CreateAsset(clip, AssetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }

        protected override void PostExecute(object? input = null)
        {
            DeleteClip();
            base.PostExecute(input);
        }

        private void DeleteClip()
        {
            if (!string.IsNullOrEmpty(AssetPath) && AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetPath) != null)
            {
                AssetDatabase.DeleteAsset(AssetPath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
    }
}
