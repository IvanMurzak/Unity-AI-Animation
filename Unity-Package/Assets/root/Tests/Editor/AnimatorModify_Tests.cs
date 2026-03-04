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
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using com.IvanMurzak.McpPlugin;
using com.IvanMurzak.Unity.MCP.Runtime.Data;
using com.IvanMurzak.Unity.MCP.Editor.Tests.Utils;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    [TestFixture]
    public class AnimatorModify_Tests
    {
        private const string TestFolder = "Assets/Tests/MCP/Animator/ModifyTests";
        private const string TestControllerPath = TestFolder + "/TestController.controller";
        private const string TestClipPath = TestFolder + "/TestClip.anim";
        private const string BaseLayerName = "Base Layer";


        // ── Argument validation ─────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_NullRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                AnimatorTools.ModifyAnimatorController(null!, new[] { new AnimatorModification { type = AnimatorModificationType.AddLayer, layerName = "X" } }));
        }

        [Test]
        public void ModifyAnimatorController_InvalidRef_ThrowsArgumentException()
        {
            var invalidRef = new AssetObjectRef();

            Assert.Throws<ArgumentException>(() =>
                AnimatorTools.ModifyAnimatorController(invalidRef, new[] { new AnimatorModification { type = AnimatorModificationType.AddLayer, layerName = "X" } }));
        }

        [Test]
        public void ModifyAnimatorController_NullModifications_ThrowsArgumentNullException()
        {
            var animatorRef = new AssetObjectRef(TestControllerPath);

            Assert.Throws<ArgumentNullException>(() =>
                AnimatorTools.ModifyAnimatorController(animatorRef, null!));
        }

        [Test]
        public void ModifyAnimatorController_EmptyModifications_ThrowsArgumentException()
        {
            var animatorRef = new AssetObjectRef(TestControllerPath);

            Assert.Throws<ArgumentException>(() =>
                AnimatorTools.ModifyAnimatorController(animatorRef, Array.Empty<AnimatorModification>()));
        }

        [Test]
        public void ModifyAnimatorController_NonExistentAsset_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                AnimatorTools.ModifyAnimatorController(new AssetObjectRef($"{TestFolder}/NonExistent.controller"), new[] { new AnimatorModification { type = AnimatorModificationType.AddLayer, layerName = "X" } }));
        }

        // ── AddParameter ────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_AddParameter_Float_ParameterAdded()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddParameter,
                            parameterName = "Speed",
                            parameterType = "Float",
                            defaultFloat = 1.5f
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    var param = controller.parameters.FirstOrDefault(p => p.name == "Speed");
                    Assert.IsNotNull(param);
                    Assert.AreEqual(AnimatorControllerParameterType.Float, param!.type);
                    Assert.AreEqual(1.5f, param.defaultFloat, 0.001f);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddParameter_Int_ParameterAdded()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddParameter,
                            parameterName = "Score",
                            parameterType = "Int",
                            defaultInt = 10
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var param = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath).parameters.FirstOrDefault(p => p.name == "Score");
                    Assert.IsNotNull(param);
                    Assert.AreEqual(AnimatorControllerParameterType.Int, param!.type);
                    Assert.AreEqual(10, param.defaultInt);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddParameter_Bool_ParameterAdded()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddParameter,
                            parameterName = "IsGrounded",
                            parameterType = "Bool",
                            defaultBool = true
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var param = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath).parameters.FirstOrDefault(p => p.name == "IsGrounded");
                    Assert.IsNotNull(param);
                    Assert.AreEqual(AnimatorControllerParameterType.Bool, param!.type);
                    Assert.IsTrue(param.defaultBool);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddParameter_Trigger_ParameterAdded()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddParameter,
                            parameterName = "Attack",
                            parameterType = "Trigger"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var param = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath).parameters.FirstOrDefault(p => p.name == "Attack");
                    Assert.IsNotNull(param);
                    Assert.AreEqual(AnimatorControllerParameterType.Trigger, param!.type);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddParameter_MissingName_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddParameter,
                            parameterType = "Float"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                    StringAssert.Contains("parameterName", response.errors![0]);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddParameter_MissingType_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddParameter,
                            parameterName = "Speed"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                    StringAssert.Contains("parameterType", response.errors![0]);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddParameter_InvalidType_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddParameter,
                            parameterName = "Speed",
                            parameterType = "NotARealType"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                }));
            folderExecutor.Execute();
        }

        // ── RemoveParameter ─────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_RemoveParameter_Valid_ParameterRemoved()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();

                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.RemoveParameter,
                            parameterName = "Speed"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var remaining = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath).parameters.FirstOrDefault(p => p.name == "Speed");
                    Assert.IsNull(remaining, "Parameter should have been removed");
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_RemoveParameter_MissingName_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification { type = AnimatorModificationType.RemoveParameter }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                    StringAssert.Contains("parameterName", response.errors![0]);
                }));
            folderExecutor.Execute();
        }

        // ── AddLayer ────────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_AddLayer_Valid_LayerAdded()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddLayer,
                            layerName = "UpperBody"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    var layer = controller.layers.FirstOrDefault(l => l.name == "UpperBody");
                    Assert.IsNotNull(layer.stateMachine, "Layer 'UpperBody' should have been added");
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddLayer_MissingName_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification { type = AnimatorModificationType.AddLayer }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                    StringAssert.Contains("layerName", response.errors![0]);
                }));
            folderExecutor.Execute();
        }

        // ── RemoveLayer ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_RemoveLayer_Valid_LayerRemoved()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    int initialLayerCount = controller.layers.Length;
                    controller.AddLayer("ExtraLayer");
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();

                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.RemoveLayer,
                            layerName = "ExtraLayer"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    // Check that the layer was removed by verifying the layer count decreased
                    var updatedController = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    Assert.IsNotNull(updatedController, "Controller reference was lost after modification");
                    Assert.AreEqual(initialLayerCount, updatedController.layers.Length,
                        "Layer count should return to initial count after removing the extra layer");
                    Assert.IsFalse(updatedController.layers.Any(l => l.name == "ExtraLayer"),
                        "Layer 'ExtraLayer' should have been removed");
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_RemoveLayer_MissingName_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification { type = AnimatorModificationType.RemoveLayer }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                    StringAssert.Contains("layerName", response.errors![0]);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_RemoveLayer_NonExistentLayer_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.RemoveLayer,
                            layerName = "DoesNotExist"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                }));
            folderExecutor.Execute();
        }

        // ── AddState ────────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_AddState_Valid_StateAdded()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddState,
                            layerName = BaseLayerName,
                            stateName = "Idle"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var layer = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath).layers[0];
                    var state = layer.stateMachine.states.FirstOrDefault(s => s.state.name == "Idle");
                    Assert.IsNotNull(state.state, "State 'Idle' should have been added");
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddState_MissingLayerName_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddState,
                            stateName = "Idle"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                    StringAssert.Contains("layerName", response.errors![0]);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddState_MissingStateName_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddState,
                            layerName = BaseLayerName
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                    StringAssert.Contains("stateName", response.errors![0]);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddState_NonExistentLayer_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddState,
                            layerName = "NonExistentLayer",
                            stateName = "Idle"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                }));
            folderExecutor.Execute();
        }

        // ── RemoveState ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_RemoveState_Valid_StateRemoved()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    var layer = controller.layers[0];
                    layer.stateMachine.AddState("Walk");
                    var layers = controller.layers;
                    controller.layers = layers;
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();

                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.RemoveState,
                            layerName = BaseLayerName,
                            stateName = "Walk"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath).layers[0];
                    var state = updatedLayer.stateMachine.states.FirstOrDefault(s => s.state.name == "Walk");
                    Assert.IsNull(state.state, "State 'Walk' should have been removed");
                }));
            folderExecutor.Execute();
        }

        // ── SetDefaultState ─────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_SetDefaultState_Valid_DefaultStateSet()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    var layer = controller.layers[0];
                    layer.stateMachine.AddState("Idle");
                    var layers = controller.layers;
                    controller.layers = layers;
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();

                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.SetDefaultState,
                            layerName = BaseLayerName,
                            stateName = "Idle"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath).layers[0];
                    Assert.IsNotNull(updatedLayer.stateMachine.defaultState);
                    Assert.AreEqual("Idle", updatedLayer.stateMachine.defaultState!.name);
                }));
            folderExecutor.Execute();
        }

        // ── AddTransition ───────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_AddTransition_Valid_TransitionAdded()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    var layer = controller.layers[0];
                    layer.stateMachine.AddState("Idle");
                    layer.stateMachine.AddState("Walk");
                    var layers = controller.layers;
                    controller.layers = layers;
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();

                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddTransition,
                            layerName = BaseLayerName,
                            sourceStateName = "Idle",
                            destinationStateName = "Walk",
                            hasExitTime = true,
                            exitTime = 0.9f,
                            duration = 0.1f
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath).layers[0];
                    var idleState = updatedLayer.stateMachine.states.First(s => s.state.name == "Idle").state;
                    Assert.IsTrue(idleState.transitions.Any(t => t.destinationState?.name == "Walk"),
                        "Transition from Idle to Walk should exist");
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddTransition_WithCondition_TransitionHasCondition()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
                    var layer = controller.layers[0];
                    layer.stateMachine.AddState("Idle");
                    layer.stateMachine.AddState("Walk");
                    var layers = controller.layers;
                    controller.layers = layers;
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();

                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddTransition,
                            layerName = BaseLayerName,
                            sourceStateName = "Idle",
                            destinationStateName = "Walk",
                            hasExitTime = false,
                            conditions = new[]
                            {
                                new AnimatorConditionData
                                {
                                    parameter = "Speed",
                                    mode = "Greater",
                                    threshold = 0.1f
                                }
                            }
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath).layers[0];
                    var idleState = updatedLayer.stateMachine.states.First(s => s.state.name == "Idle").state;
                    var transition = idleState.transitions.First(t => t.destinationState?.name == "Walk");
                    Assert.AreEqual(1, transition.conditions.Length);
                    Assert.AreEqual("Speed", transition.conditions[0].parameter);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddTransition_MissingSourceState_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddTransition,
                            layerName = BaseLayerName,
                            destinationStateName = "Walk"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                    StringAssert.Contains("sourceStateName", response.errors![0]);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddTransition_MissingDestinationState_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddTransition,
                            layerName = BaseLayerName,
                            sourceStateName = "Idle"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                    StringAssert.Contains("destinationStateName", response.errors![0]);
                }));
            folderExecutor.Execute();
        }

        // ── RemoveTransition ────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_RemoveTransition_Valid_TransitionRemoved()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    // Setup: create states and transition
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    var layer = controller.layers[0];
                    var idleState = layer.stateMachine.AddState("Idle");
                    var walkState = layer.stateMachine.AddState("Walk");
                    idleState.AddTransition(walkState);
                    var layers = controller.layers;
                    controller.layers = layers;
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();

                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.RemoveTransition,
                            layerName = BaseLayerName,
                            sourceStateName = "Idle",
                            destinationStateName = "Walk"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath).layers[0];
                    var updatedIdleState = updatedLayer.stateMachine.states.First(s => s.state.name == "Idle").state;
                    Assert.IsFalse(updatedIdleState.transitions.Any(t => t.destinationState?.name == "Walk"),
                        "Transition should have been removed");
                }));
            folderExecutor.Execute();
        }

        // ── AddAnyStateTransition ────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_AddAnyStateTransition_Valid_TransitionAdded()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    var layer = controller.layers[0];
                    layer.stateMachine.AddState("Death");
                    var layers = controller.layers;
                    controller.layers = layers;
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();

                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddAnyStateTransition,
                            layerName = BaseLayerName,
                            destinationStateName = "Death",
                            hasExitTime = false,
                            duration = 0.1f
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath).layers[0];
                    Assert.IsTrue(updatedLayer.stateMachine.anyStateTransitions.Any(t => t.destinationState?.name == "Death"),
                        "Any-state transition to Death should exist");
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddAnyStateTransition_MissingDestination_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddAnyStateTransition,
                            layerName = BaseLayerName
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                    StringAssert.Contains("destinationStateName", response.errors![0]);
                }));
            folderExecutor.Execute();
        }

        // ── SetStateMotion ───────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_SetStateMotion_WithExistingClip_MotionSet()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new AnimationClipExecutor(TestClipPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    var layer = controller.layers[0];
                    layer.stateMachine.AddState("Walk");
                    var layers = controller.layers;
                    controller.layers = layers;
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();

                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.SetStateMotion,
                            layerName = BaseLayerName,
                            stateName = "Walk",
                            motionAssetPath = TestClipPath
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath).layers[0];
                    var walkState = updatedLayer.stateMachine.states.First(s => s.state.name == "Walk").state;
                    Assert.IsNotNull(walkState.motion, "Motion should be set on the Walk state");
                    Assert.AreEqual("TestClip", walkState.motion!.name);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_SetStateMotion_WithNonExistentMotion_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    var layer = controller.layers[0];
                    layer.stateMachine.AddState("Walk");
                    var layers = controller.layers;
                    controller.layers = layers;
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();

                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.SetStateMotion,
                            layerName = BaseLayerName,
                            stateName = "Walk",
                            motionAssetPath = $"{TestFolder}/NonExistentClip.anim"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_SetStateMotion_MissingMotionPath_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    var layer = controller.layers[0];
                    layer.stateMachine.AddState("Walk");
                    var layers = controller.layers;
                    controller.layers = layers;
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();

                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.SetStateMotion,
                            layerName = BaseLayerName,
                            stateName = "Walk"
                            // Missing motionAssetPath
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                    StringAssert.Contains("motionAssetPath", response.errors![0]);
                }));
            folderExecutor.Execute();
        }

        // ── SetStateSpeed ────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_SetStateSpeed_Valid_SpeedSet()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    var layer = controller.layers[0];
                    layer.stateMachine.AddState("Run");
                    var layers = controller.layers;
                    controller.layers = layers;
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();

                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.SetStateSpeed,
                            layerName = BaseLayerName,
                            stateName = "Run",
                            speed = 2.0f
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors);
                    var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath).layers[0];
                    var runState = updatedLayer.stateMachine.states.First(s => s.state.name == "Run").state;
                    Assert.AreEqual(2.0f, runState.speed, 0.001f);
                }));
            folderExecutor.Execute();
        }

        [Test]
        public void ModifyAnimatorController_SetStateSpeed_MissingSpeed_ReturnsError()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    var layer = controller.layers[0];
                    layer.stateMachine.AddState("Run");
                    var layers = controller.layers;
                    controller.layers = layers;
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();

                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.SetStateSpeed,
                            layerName = BaseLayerName,
                            stateName = "Run"
                            // Missing speed
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.errors);
                    StringAssert.Contains("speed", response.errors![0]);
                }));
            folderExecutor.Execute();
        }

        // ── Response structure ───────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_Response_ContainsModifiedAssetInfo()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification
                        {
                            type = AnimatorModificationType.AddParameter,
                            parameterName = "TestParam",
                            parameterType = "Float"
                        }
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNotNull(response.modifiedAsset);
                    Assert.AreEqual(TestControllerPath, response.modifiedAsset!.path);
                    Assert.AreEqual("TestController", response.modifiedAsset.name);
                    Assert.NotZero(response.modifiedAsset.instanceId);
                }));
            folderExecutor.Execute();
        }

        // ── Complex multi-modification scenario ──────────────────────────────────

        [Test]
        public void ModifyAnimatorController_ComplexSetup_AllModificationsApplied()
        {
            var folderExecutor = new CreateFolderExecutor("Assets", "Tests", "MCP", "Animator", "ModifyTests");
            folderExecutor.Nest(new AnimatorControllerExecutor(TestControllerPath))
                .Nest(new LazyNodeExecutor().SetAction(() =>
                {
                    var animatorRef = new AssetObjectRef(TestControllerPath);
                    var mods = new[]
                    {
                        new AnimatorModification { type = AnimatorModificationType.AddParameter, parameterName = "Speed", parameterType = "Float" },
                        new AnimatorModification { type = AnimatorModificationType.AddParameter, parameterName = "IsJumping", parameterType = "Bool" },
                        new AnimatorModification { type = AnimatorModificationType.AddLayer, layerName = "UpperBody" },
                        new AnimatorModification { type = AnimatorModificationType.AddState, layerName = BaseLayerName, stateName = "Idle" },
                        new AnimatorModification { type = AnimatorModificationType.AddState, layerName = BaseLayerName, stateName = "Walk" },
                        new AnimatorModification { type = AnimatorModificationType.SetDefaultState, layerName = BaseLayerName, stateName = "Idle" },
                    };

                    var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                    Assert.IsNull(response.errors, "All modifications should succeed");

                    var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(TestControllerPath);
                    Assert.AreEqual(2, controller.parameters.Length, "Should have 2 parameters");
                    Assert.IsTrue(controller.layers.Any(l => l.name == "UpperBody"), "UpperBody layer should exist");

                    var baseLayer = controller.layers[0];
                    Assert.IsTrue(baseLayer.stateMachine.states.Any(s => s.state.name == "Idle"));
                    Assert.IsTrue(baseLayer.stateMachine.states.Any(s => s.state.name == "Walk"));
                    Assert.AreEqual("Idle", baseLayer.stateMachine.defaultState?.name);
                }));
            folderExecutor.Execute();
        }
    }
}
