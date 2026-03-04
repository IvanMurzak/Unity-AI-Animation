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
using com.IvanMurzak.Unity.MCP.Editor.Tests.Utils;
using UnityEditor;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    public class CreateAnimationClipExecutor : BaseCreateAssetExecutor<AnimationClip>
    {
        public CreateAnimationClipExecutor(string clipName, params string[] folders) : base(clipName, folders)
        {
            if (!clipName.EndsWith(".anim", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Clip name must end with the '.anim' file extension.", nameof(clipName));

            SetAction(() =>
            {
                Debug.Log($"Creating AnimationClip at path: {AssetPath}");
                Asset = new AnimationClip();
                AssetDatabase.CreateAsset(Asset, AssetPath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            });
        }
    }
}
