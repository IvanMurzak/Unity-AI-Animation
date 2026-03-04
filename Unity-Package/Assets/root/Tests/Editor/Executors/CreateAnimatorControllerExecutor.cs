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
using UnityEditor.Animations;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    public class CreateAnimatorControllerExecutor : BaseCreateAssetExecutor<AnimatorController>
    {
        public CreateAnimatorControllerExecutor(string controllerName, params string[] folders) : base(controllerName, folders)
        {
            if (!controllerName.EndsWith(".controller", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Controller name must end with '.controller'.", nameof(controllerName));

            SetAction(() =>
            {
                Debug.Log($"Creating AnimatorController at path: {AssetPath}");
                Asset = AnimatorController.CreateAnimatorControllerAtPath(AssetPath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            });
        }
    }
}
