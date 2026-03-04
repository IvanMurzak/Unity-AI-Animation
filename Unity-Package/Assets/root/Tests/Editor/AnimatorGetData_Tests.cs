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
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using com.IvanMurzak.McpPlugin;
using com.IvanMurzak.Unity.MCP.Runtime.Data;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    [TestFixture]
    public class AnimatorGetData_Tests
    {
        private const string TestFolder = "Assets/Tests/MCP/Animator/GetDataTests";
        private const string TestControllerPath = TestFolder + "/TestController.controller";

        private AnimatorControllerExecutor _controllerExecutor = null!;

        [SetUp]
        public void SetUp()
        {
            _controllerExecutor = new AnimatorControllerExecutor(TestControllerPath);
            _controllerExecutor.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            _controllerExecutor.Teardown();

            if (AssetDatabase.IsValidFolder(TestFolder))
            {
                AssetDatabase.DeleteAsset(TestFolder);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }

        [Test]
        public void GetData_NullRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                AnimatorTools.GetData(null!));
        }

        [Test]
        public void GetData_InvalidRef_ThrowsArgumentException()
        {
            var invalidRef = new AssetObjectRef();

            Assert.Throws<ArgumentException>(() =>
                AnimatorTools.GetData(invalidRef));
        }

        [Test]
        public void GetData_NonExistentAsset_ThrowsException()
        {
            var ref_ = new AssetObjectRef($"{TestFolder}/NonExistent.controller");

            Assert.Throws<Exception>(() =>
                AnimatorTools.GetData(ref_));
        }

        [Test]
        public void GetData_ValidController_ReturnsBasicData()
        {
            var animatorRef = new AssetObjectRef(TestControllerPath);
            var response = AnimatorTools.GetData(animatorRef);

            Assert.IsNotNull(response);
            Assert.AreEqual("TestController", response.name);
            Assert.IsNotNull(response.parameters);
            Assert.IsNotNull(response.layers);
        }

        [Test]
        public void GetData_NewController_HasBaseLayer()
        {
            var animatorRef = new AssetObjectRef(TestControllerPath);
            var response = AnimatorTools.GetData(animatorRef);

            Assert.IsNotNull(response.layers);
            Assert.GreaterOrEqual(response.layers!.Count, 1, "New controller should have Base Layer");
            Assert.AreEqual("Base Layer", response.layers[0].name);
        }

        [Test]
        public void GetData_ControllerWithAddedParameter_ReturnsParameter()
        {
            // Add a Float parameter directly
            var controller = _controllerExecutor.Controller!;
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();

            var animatorRef = new AssetObjectRef(TestControllerPath);
            var response = AnimatorTools.GetData(animatorRef);

            Assert.IsNotNull(response.parameters);
            Assert.AreEqual(1, response.parameters!.Count);
            Assert.AreEqual("Speed", response.parameters[0].name);
            Assert.AreEqual("Float", response.parameters[0].type);
        }

        [Test]
        public void GetData_ControllerWithMultipleParameters_ReturnsAllParameters()
        {
            var controller = _controllerExecutor.Controller!;
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("IsJumping", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Score", AnimatorControllerParameterType.Int);
            controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();

            var animatorRef = new AssetObjectRef(TestControllerPath);
            var response = AnimatorTools.GetData(animatorRef);

            Assert.IsNotNull(response.parameters);
            Assert.AreEqual(4, response.parameters!.Count);

            var paramNames = new System.Collections.Generic.HashSet<string>(new[] { "Speed", "IsJumping", "Score", "Attack" });
            foreach (var param in response.parameters)
                Assert.IsTrue(paramNames.Contains(param.name), $"Unexpected param: {param.name}");
        }

        [Test]
        public void GetData_ControllerWithState_ReturnsStateInLayer()
        {
            var controller = _controllerExecutor.Controller!;
            var layer = controller.layers[0];
            layer.stateMachine.AddState("Idle");
            // Need to reassign layers for changes to take effect
            var layers = controller.layers;
            controller.layers = layers;
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();

            var animatorRef = new AssetObjectRef(TestControllerPath);
            var response = AnimatorTools.GetData(animatorRef);

            Assert.IsNotNull(response.layers);
            var baseLayer = response.layers![0];
            Assert.IsNotNull(baseLayer.states);
            Assert.IsTrue(baseLayer.states!.Count >= 1, "Should have at least 'Idle' state");
        }

        [Test]
        public void GetData_LayerInfo_IncludesDefaultStateName()
        {
            var controller = _controllerExecutor.Controller!;
            var layer = controller.layers[0];
            var idleState = layer.stateMachine.AddState("Idle");
            layer.stateMachine.defaultState = idleState;
            var layers = controller.layers;
            controller.layers = layers;
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();

            var animatorRef = new AssetObjectRef(TestControllerPath);
            var response = AnimatorTools.GetData(animatorRef);

            var baseLayer = response.layers![0];
            Assert.AreEqual("Idle", baseLayer.defaultStateName);
        }
    }
}
