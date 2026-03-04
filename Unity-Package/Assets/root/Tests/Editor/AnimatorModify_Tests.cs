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

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    [TestFixture]
    public class AnimatorModify_Tests
    {
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
            var clipEx = new CreateAnimatorControllerExecutor("animation.controller", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(clipEx.AssetPath);
                Assert.Throws<ArgumentNullException>(() =>
                    AnimatorTools.ModifyAnimatorController(animatorRef, null!));
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_EmptyModifications_ThrowsArgumentException()
        {
            var clipEx = new CreateAnimatorControllerExecutor("animation.controller", "Assets", "Tests");
            clipEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(clipEx.AssetPath);
                Assert.Throws<ArgumentException>(() =>
                    AnimatorTools.ModifyAnimatorController(animatorRef, Array.Empty<AnimatorModification>()));
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_NonExistentAsset_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                AnimatorTools.ModifyAnimatorController(new AssetObjectRef("Assets/Tests/NonExistent.controller"), new[] { new AnimatorModification { type = AnimatorModificationType.AddLayer, layerName = "X" } }));
        }

        // ── AddParameter ────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_AddParameter_Float_ParameterAdded()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath);
                var param = controller.parameters.FirstOrDefault(p => p.name == "Speed");
                Assert.IsNotNull(param);
                Assert.AreEqual(AnimatorControllerParameterType.Float, param!.type);
                Assert.AreEqual(1.5f, param.defaultFloat, 0.001f);
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddParameter_Int_ParameterAdded()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var param = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath).parameters.FirstOrDefault(p => p.name == "Score");
                Assert.IsNotNull(param);
                Assert.AreEqual(AnimatorControllerParameterType.Int, param!.type);
                Assert.AreEqual(10, param.defaultInt);
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddParameter_Bool_ParameterAdded()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var param = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath).parameters.FirstOrDefault(p => p.name == "IsGrounded");
                Assert.IsNotNull(param);
                Assert.AreEqual(AnimatorControllerParameterType.Bool, param!.type);
                Assert.IsTrue(param.defaultBool);
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddParameter_Trigger_ParameterAdded()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var param = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath).parameters.FirstOrDefault(p => p.name == "Attack");
                Assert.IsNotNull(param);
                Assert.AreEqual(AnimatorControllerParameterType.Trigger, param!.type);
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddParameter_MissingName_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddParameter_MissingType_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddParameter_InvalidType_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
            }).Execute();
        }

        // ── RemoveParameter ─────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_RemoveParameter_Valid_ParameterRemoved()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var controller = controllerEx.Asset ?? throw new InvalidOperationException("Controller should have been created by executor");
                controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();

                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var remaining = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath).parameters.FirstOrDefault(p => p.name == "Speed");
                Assert.IsNull(remaining, "Parameter should have been removed");
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_RemoveParameter_MissingName_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
                var mods = new[]
                {
                    new AnimatorModification { type = AnimatorModificationType.RemoveParameter }
                };

                var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("parameterName", response.errors![0]);
            }).Execute();
        }

        // ── AddLayer ────────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_AddLayer_Valid_LayerAdded()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath);
                var layer = controller.layers.FirstOrDefault(l => l.name == "UpperBody");
                Assert.IsNotNull(layer.stateMachine, "Layer 'UpperBody' should have been added");
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddLayer_MissingName_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
                var mods = new[]
                {
                    new AnimatorModification { type = AnimatorModificationType.AddLayer }
                };

                var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("layerName", response.errors![0]);
            }).Execute();
        }

        // ── RemoveLayer ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_RemoveLayer_Valid_LayerRemoved()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var controller = controllerEx.Asset ?? throw new InvalidOperationException("Controller should have been created by executor");
                int initialLayerCount = controller.layers.Length;
                controller.AddLayer("ExtraLayer");
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();

                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var updatedController = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath);
                Assert.IsNotNull(updatedController, "Controller reference was lost after modification");
                Assert.AreEqual(initialLayerCount, updatedController.layers.Length,
                    "Layer count should return to initial count after removing the extra layer");
                Assert.IsFalse(updatedController.layers.Any(l => l.name == "ExtraLayer"),
                    "Layer 'ExtraLayer' should have been removed");
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_RemoveLayer_MissingName_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
                var mods = new[]
                {
                    new AnimatorModification { type = AnimatorModificationType.RemoveLayer }
                };

                var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("layerName", response.errors![0]);
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_RemoveLayer_NonExistentLayer_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
            }).Execute();
        }

        // ── AddState ────────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_AddState_Valid_StateAdded()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var layer = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath).layers[0];
                var state = layer.stateMachine.states.FirstOrDefault(s => s.state.name == "Idle");
                Assert.IsNotNull(state.state, "State 'Idle' should have been added");
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddState_MissingLayerName_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddState_MissingStateName_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddState_NonExistentLayer_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
            }).Execute();
        }

        // ── RemoveState ─────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_RemoveState_Valid_StateRemoved()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var controller = controllerEx.Asset ?? throw new InvalidOperationException("Controller should have been created by executor");
                var layer = controller.layers[0];
                layer.stateMachine.AddState("Walk");
                var layers = controller.layers;
                controller.layers = layers;
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();

                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath).layers[0];
                var state = updatedLayer.stateMachine.states.FirstOrDefault(s => s.state.name == "Walk");
                Assert.IsNull(state.state, "State 'Walk' should have been removed");
            }).Execute();
        }

        // ── SetDefaultState ─────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_SetDefaultState_Valid_DefaultStateSet()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var controller = controllerEx.Asset ?? throw new InvalidOperationException("Controller should have been created by executor");
                var layer = controller.layers[0];
                layer.stateMachine.AddState("Idle");
                var layers = controller.layers;
                controller.layers = layers;
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();

                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath).layers[0];
                Assert.IsNotNull(updatedLayer.stateMachine.defaultState);
                Assert.AreEqual("Idle", updatedLayer.stateMachine.defaultState!.name);
            }).Execute();
        }

        // ── AddTransition ───────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_AddTransition_Valid_TransitionAdded()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var controller = controllerEx.Asset ?? throw new InvalidOperationException("Controller should have been created by executor");
                var layer = controller.layers[0];
                layer.stateMachine.AddState("Idle");
                layer.stateMachine.AddState("Walk");
                var layers = controller.layers;
                controller.layers = layers;
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();

                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath).layers[0];
                var idleState = updatedLayer.stateMachine.states.First(s => s.state.name == "Idle").state;
                Assert.IsTrue(idleState.transitions.Any(t => t.destinationState?.name == "Walk"),
                    "Transition from Idle to Walk should exist");
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddTransition_WithCondition_TransitionHasCondition()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var controller = controllerEx.Asset ?? throw new InvalidOperationException("Controller should have been created by executor");
                controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
                var layer = controller.layers[0];
                layer.stateMachine.AddState("Idle");
                layer.stateMachine.AddState("Walk");
                var layers = controller.layers;
                controller.layers = layers;
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();

                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath).layers[0];
                var idleState = updatedLayer.stateMachine.states.First(s => s.state.name == "Idle").state;
                var transition = idleState.transitions.First(t => t.destinationState?.name == "Walk");
                Assert.AreEqual(1, transition.conditions.Length);
                Assert.AreEqual("Speed", transition.conditions[0].parameter);
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddTransition_MissingSourceState_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddTransition_MissingDestinationState_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddTransition_MissingLayerName_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
                var mods = new[]
                {
                    new AnimatorModification
                    {
                        type = AnimatorModificationType.AddTransition,
                        sourceStateName = "Idle",
                        destinationStateName = "Walk"
                    }
                };

                var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("layerName", response.errors![0]);
            }).Execute();
        }

        // ── RemoveTransition ────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_RemoveTransition_Valid_TransitionRemoved()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                // Setup: create states and transition
                var controller = controllerEx.Asset ?? throw new InvalidOperationException("Controller should have been created by executor");
                var layer = controller.layers[0];
                var idleState = layer.stateMachine.AddState("Idle");
                var walkState = layer.stateMachine.AddState("Walk");
                idleState.AddTransition(walkState);
                var layers = controller.layers;
                controller.layers = layers;
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();

                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath).layers[0];
                var updatedIdleState = updatedLayer.stateMachine.states.First(s => s.state.name == "Idle").state;
                Assert.IsFalse(updatedIdleState.transitions.Any(t => t.destinationState?.name == "Walk"),
                    "Transition should have been removed");
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_RemoveTransition_MissingSourceState_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
                var mods = new[]
                {
                    new AnimatorModification
                    {
                        type = AnimatorModificationType.RemoveTransition,
                        layerName = BaseLayerName,
                        destinationStateName = "Walk"
                    }
                };

                var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("sourceStateName", response.errors![0]);
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_RemoveTransition_MissingDestinationState_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
                var mods = new[]
                {
                    new AnimatorModification
                    {
                        type = AnimatorModificationType.RemoveTransition,
                        layerName = BaseLayerName,
                        sourceStateName = "Idle"
                    }
                };

                var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("destinationStateName", response.errors![0]);
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_RemoveTransition_MissingLayerName_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
                var mods = new[]
                {
                    new AnimatorModification
                    {
                        type = AnimatorModificationType.RemoveTransition,
                        sourceStateName = "Idle",
                        destinationStateName = "Walk"
                    }
                };

                var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("layerName", response.errors![0]);
            }).Execute();
        }

        // ── AddAnyStateTransition ────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_AddAnyStateTransition_Valid_TransitionAdded()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var controller = controllerEx.Asset ?? throw new InvalidOperationException("Controller should have been created by executor");
                var layer = controller.layers[0];
                layer.stateMachine.AddState("Death");
                var layers = controller.layers;
                controller.layers = layers;
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();

                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath).layers[0];
                Assert.IsTrue(updatedLayer.stateMachine.anyStateTransitions.Any(t => t.destinationState?.name == "Death"),
                    "Any-state transition to Death should exist");
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddAnyStateTransition_MissingLayerName_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
                var mods = new[]
                {
                    new AnimatorModification
                    {
                        type = AnimatorModificationType.AddAnyStateTransition,
                        destinationStateName = "Death"
                    }
                };

                var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("layerName", response.errors![0]);
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_AddAnyStateTransition_MissingDestination_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
            }).Execute();
        }

        // ── SetStateMotion ───────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_SetStateMotion_WithExistingClip_MotionSet()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            var clipEx = new CreateAnimationClipExecutor("TestClip.anim", "Assets", "Tests");
            controllerEx.AddChild(clipEx);
            clipEx.AddChild(() =>
            {
                var controller = controllerEx.Asset ?? throw new InvalidOperationException("Controller should have been created by executor");
                var layer = controller.layers[0];
                layer.stateMachine.AddState("Walk");
                var layers = controller.layers;
                controller.layers = layers;
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();

                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
                var mods = new[]
                {
                    new AnimatorModification
                    {
                        type = AnimatorModificationType.SetStateMotion,
                        layerName = BaseLayerName,
                        stateName = "Walk",
                        motionAssetPath = clipEx.AssetPath
                    }
                };

                var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                Assert.IsNull(response.errors);
                var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath).layers[0];
                var walkState = updatedLayer.stateMachine.states.First(s => s.state.name == "Walk").state;
                Assert.IsNotNull(walkState.motion, "Motion should be set on the Walk state");
                Assert.AreEqual("TestClip", walkState.motion!.name);
            });
            controllerEx.Execute();
        }

        [Test]
        public void ModifyAnimatorController_SetStateMotion_WithNonExistentMotion_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var controller = controllerEx.Asset ?? throw new InvalidOperationException("Controller should have been created by executor");
                var layer = controller.layers[0];
                layer.stateMachine.AddState("Walk");
                var layers = controller.layers;
                controller.layers = layers;
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();

                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
                var mods = new[]
                {
                    new AnimatorModification
                    {
                        type = AnimatorModificationType.SetStateMotion,
                        layerName = BaseLayerName,
                        stateName = "Walk",
                        motionAssetPath = "Assets/Tests/NonExistentClip.anim"
                    }
                };

                var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                Assert.IsNotNull(response.errors);
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_SetStateMotion_MissingMotionPath_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var controller = controllerEx.Asset ?? throw new InvalidOperationException("Controller should have been created by executor");
                var layer = controller.layers[0];
                layer.stateMachine.AddState("Walk");
                var layers = controller.layers;
                controller.layers = layers;
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();

                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
            }).Execute();
        }

        // ── SetStateSpeed ────────────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_SetStateSpeed_Valid_SpeedSet()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var controller = controllerEx.Asset ?? throw new InvalidOperationException("Controller should have been created by executor");
                var layer = controller.layers[0];
                layer.stateMachine.AddState("Run");
                var layers = controller.layers;
                controller.layers = layers;
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();

                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                var updatedLayer = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath).layers[0];
                var runState = updatedLayer.stateMachine.states.First(s => s.state.name == "Run").state;
                Assert.AreEqual(2.0f, runState.speed, 0.001f);
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_SetStateSpeed_MissingSpeed_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var controller = controllerEx.Asset ?? throw new InvalidOperationException("Controller should have been created by executor");
                var layer = controller.layers[0];
                layer.stateMachine.AddState("Run");
                var layers = controller.layers;
                controller.layers = layers;
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();

                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_SetStateSpeed_MissingLayerName_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
                var mods = new[]
                {
                    new AnimatorModification
                    {
                        type = AnimatorModificationType.SetStateSpeed,
                        stateName = "Run",
                        speed = 2.0f
                    }
                };

                var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("layerName", response.errors![0]);
            }).Execute();
        }

        [Test]
        public void ModifyAnimatorController_SetStateSpeed_MissingStateName_ReturnsError()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
                var mods = new[]
                {
                    new AnimatorModification
                    {
                        type = AnimatorModificationType.SetStateSpeed,
                        layerName = BaseLayerName,
                        speed = 2.0f
                    }
                };

                var response = AnimatorTools.ModifyAnimatorController(animatorRef, mods);

                Assert.IsNotNull(response.errors);
                StringAssert.Contains("stateName", response.errors![0]);
            }).Execute();
        }

        // ── Response structure ───────────────────────────────────────────────────

        [Test]
        public void ModifyAnimatorController_Response_ContainsModifiedAssetInfo()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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
                Assert.AreEqual(controllerEx.AssetPath, response.modifiedAsset!.path);
                Assert.AreEqual("TestController", response.modifiedAsset.name);
                Assert.NotZero(response.modifiedAsset.instanceId);
            }).Execute();
        }

        // ── Complex multi-modification scenario ──────────────────────────────────

        [Test]
        public void ModifyAnimatorController_ComplexSetup_AllModificationsApplied()
        {
            var controllerEx = new CreateAnimatorControllerExecutor("TestController.controller", "Assets", "Tests");
            controllerEx.AddChild(() =>
            {
                var animatorRef = new AssetObjectRef(controllerEx.AssetPath);
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

                var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerEx.AssetPath);
                Assert.AreEqual(2, controller.parameters.Length, "Should have 2 parameters");
                Assert.IsTrue(controller.layers.Any(l => l.name == "UpperBody"), "UpperBody layer should exist");

                var baseLayer = controller.layers[0];
                Assert.IsTrue(baseLayer.stateMachine.states.Any(s => s.state.name == "Idle"));
                Assert.IsTrue(baseLayer.stateMachine.states.Any(s => s.state.name == "Walk"));
                Assert.AreEqual("Idle", baseLayer.stateMachine.defaultState?.name);
            }).Execute();
        }
    }
}
