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
using UnityEditor.Animations;

namespace com.IvanMurzak.Unity.MCP.Animation
{
    public static partial class AnimatorTools
    {
        [McpPluginTool
        (
            "animator-create",
            Title = "Animator / Create"
        )]
        [Description(@"Create Unity's AnimatorController asset files. Creates folders recursively if they do not exist. Each path should start with 'Assets/' and end with '.controller'.")]
        public static CreateAnimatorResponse CreateAnimatorControllers
        (
            [Description("The paths of the animator controller assets to create. Each path should start with 'Assets/' and end with '.controller'.")]
            string[] sourcePaths
        )
        {
            return MainThread.Instance.Run(() =>
            {
                if (sourcePaths == null || sourcePaths.Length == 0)
                    throw new System.Exception("The sourcePaths array is empty or null.");

                var response = new CreateAnimatorResponse();

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

                    if (!assetPath.EndsWith(".controller"))
                    {
                        response.errors ??= new();
                        response.errors.Add($"Asset path '{assetPath}' must end with '.controller'.");
                        continue;
                    }

                    // Create all folders in the path if they do not exist
                    var directory = Path.GetDirectoryName(assetPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                    }

                    // Create a new AnimatorController
                    var controller = AnimatorController.CreateAnimatorControllerAtPath(assetPath);
                    controller.name = Path.GetFileNameWithoutExtension(assetPath);

                    response.createdAssets ??= new();
                    response.createdAssets.Add(new CreatedAnimatorInfo
                    {
                        path = assetPath,
                        instanceId = controller.GetInstanceID(),
                        name = controller.name
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

        public class CreatedAnimatorInfo
        {
            public string path = string.Empty;
            public int instanceId;
            public string name = string.Empty;
        }

        public class CreateAnimatorResponse
        {
            public List<CreatedAnimatorInfo>? createdAssets;
            public List<string>? errors;
        }
    }
}
