/*
┌──────────────────────────────────────────────────────────────────┐
│  Author: Tristyn Mackay (https://github.com/InMetaTech-Tristyn)  │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-MCP)    │
│  Copyright (c) 2025 Ivan Murzak                                  │
│  Licensed under the Apache License, Version 2.0.                 │
│  See the LICENSE file in the project root for more information.  │
└──────────────────────────────────────────────────────────────────┘
*/

#nullable enable

using com.IvanMurzak.Unity.MCP.Animation;
using com.IvanMurzak.Unity.MCP.Editor.Tests;
using com.IvanMurzak.Unity.MCP.Editor.Tests.Utils;
using com.IvanMurzak.Unity.MCP.Runtime.Utils;
using NUnit.Framework;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    public partial class ToolTests
    {
        [Test]
        public void AnimatorToolTypeHasMcpAttribute()
        {
            Assert.IsTrue(HasAttributeByName(typeof(AnimatorTools), "McpPluginToolTypeAttribute"));
        }

        [Test]
        public void AnimatorToolMethodsAreRegistered()
        {
            var toolMethods = GetToolMethods(typeof(AnimatorTools));
            Assert.IsNotEmpty(toolMethods);
        }
    }
}
