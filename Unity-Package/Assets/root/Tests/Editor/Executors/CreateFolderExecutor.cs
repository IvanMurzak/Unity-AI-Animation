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
using UnityEditor;
using com.IvanMurzak.Unity.MCP.Editor.Tests.Utils;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    /// <summary>
    /// Test executor that manages folder creation and deletion via LazyNodeExecutor.
    /// Creates the full folder hierarchy when SetAction is called, deletes it in PostExecute.
    /// </summary>
    public class CreateFolderExecutor : LazyNodeExecutor
    {
        private readonly string[] _folderParts;
        private readonly List<string> _createdFolders = new();

        public CreateFolderExecutor(params string[] folderParts)
        {
            _folderParts = folderParts;
            SetAction(() => CreateFolders());
        }

        private void CreateFolders()
        {
            string currentPath = string.Empty;
            foreach (var part in _folderParts)
            {
                currentPath = string.IsNullOrEmpty(currentPath) ? part : $"{currentPath}/{part}";

                if (!AssetDatabase.IsValidFolder(currentPath))
                {
                    var parentPath = currentPath.Contains("/")
                        ? currentPath.Substring(0, currentPath.LastIndexOf("/"))
                        : string.Empty;

                    string folderName = currentPath.Contains("/")
                        ? currentPath.Substring(currentPath.LastIndexOf("/") + 1)
                        : part;

                    AssetDatabase.CreateFolder(parentPath, folderName);
                    _createdFolders.Add(currentPath);
                }
            }

            if (_createdFolders.Count > 0)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }

        protected override void PostExecute(object? input = null)
        {
            DeleteFolders();
            base.PostExecute(input);
        }

        private void DeleteFolders()
        {
            // Delete in reverse order (deepest first)
            for (int i = _createdFolders.Count - 1; i >= 0; i--)
            {
                var folder = _createdFolders[i];
                if (AssetDatabase.IsValidFolder(folder))
                {
                    AssetDatabase.DeleteAsset(folder);
                }
            }

            if (_createdFolders.Count > 0)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }

            _createdFolders.Clear();
        }
    }
}
