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

using System;
using System.Linq;
using System.Reflection;
using com.IvanMurzak.Unity.MCP.Editor.Tests;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    public partial class ToolTests : BaseTest
    {
        private static MethodInfo[] GetToolMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(method => HasAttributeByName(method, "McpPluginToolAttribute"))
                .ToArray();
        }

        private static bool HasAttributeByName(MemberInfo member, string attributeName)
        {
            return member.GetCustomAttributes(false).Any(attribute =>
                string.Equals(attribute.GetType().Name, attributeName, StringComparison.Ordinal));
        }
    }
}
