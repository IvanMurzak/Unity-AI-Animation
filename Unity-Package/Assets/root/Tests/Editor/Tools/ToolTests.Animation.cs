/*
┌──────────────────────────────────────────────────────────────────┐
│  Author: Tristyn Mackay (https://github.com/Tristyn-InMetaTech)  │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-MCP)    │
│  Copyright (c) 2025 Ivan Murzak                                  │
│  Licensed under the Apache License, Version 2.0.                 │
│  See the LICENSE file in the project root for more information.  │
└──────────────────────────────────────────────────────────────────┘
*/

#nullable enable

using com.IvanMurzak.Unity.MCP.Animation;
using com.IvanMurzak.Unity.MCP.Editor.Tests;
using NUnit.Framework;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    public partial class ToolTests
    {
        [Test]
        public void AnimationToolTypeHasMcpAttribute()
        {
            Assert.IsTrue(HasAttributeByName(typeof(AnimationTools), "McpPluginToolTypeAttribute"));
        }

        [Test]
        public void AnimationToolMethodsAreRegistered()
        {
            var toolMethods = GetToolMethods(typeof(AnimationTools));
            Assert.IsNotEmpty(toolMethods);
        }
    }
}
