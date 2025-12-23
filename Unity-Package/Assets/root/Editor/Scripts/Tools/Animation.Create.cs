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

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using com.IvanMurzak.McpPlugin;
using com.IvanMurzak.ReflectorNet.Utils;
using UnityEditor;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Animation
{
    public static partial class AnimationTools
    {
        [McpPluginTool
        (
            "animation-create",
            Title = "Animation / Create"
        )]
        [Description(@"Create Unity's Animation asset files (AnimationClip). Creates folders recursively if they do not exist. Each path should start with 'Assets/' and end with '.anim'.")]
        public static CreateAnimationResponse CreateAnimationClips
        (
            [Description("The paths of the animation assets to create. Each path should start with 'Assets/' and end with '.anim'.")]
            string[] sourcePaths
        )
        {
            return MainThread.Instance.Run(() =>
            {
                if (sourcePaths == null || sourcePaths.Length == 0)
                    throw new System.Exception("The sourcePaths array is empty or null.");

                var response = new CreateAnimationResponse();

                foreach (var assetPath in sourcePaths)
                {
                    if (string.IsNullOrEmpty(assetPath))
                    {
                        response.errors ??= new();
                        response.errors.Add("Asset path is empty or null.");
                        continue;
                    }

                    if (!assetPath.StartsWith("Assets/"))
                    {
                        response.errors ??= new();
                        response.errors.Add($"Asset path '{assetPath}' must start with 'Assets/'.");
                        continue;
                    }

                    if (!assetPath.EndsWith(".anim"))
                    {
                        response.errors ??= new();
                        response.errors.Add($"Asset path '{assetPath}' must end with '.anim'.");
                        continue;
                    }

                    // Create all folders in the path if they do not exist
                    var directory = Path.GetDirectoryName(assetPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                    }

                    // Create a new AnimationClip
                    var animationClip = new AnimationClip
                    {
                        name = Path.GetFileNameWithoutExtension(assetPath)
                    };

                    AssetDatabase.CreateAsset(animationClip, assetPath);

                    response.createdAssets ??= new();
                    response.createdAssets.Add(new CreatedAnimationInfo
                    {
                        path = assetPath,
                        instanceId = animationClip.GetInstanceID(),
                        name = animationClip.name
                    });
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                EditorApplication.RepaintProjectWindow();
                EditorApplication.RepaintHierarchyWindow();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

                return response;
            });
        }

        public class CreatedAnimationInfo
        {
            public string path = string.Empty;
            public int instanceId;
            public string name = string.Empty;
        }

        public class CreateAnimationResponse
        {
            public List<CreatedAnimationInfo>? createdAssets;
            public List<string>? errors;
        }
    }
}