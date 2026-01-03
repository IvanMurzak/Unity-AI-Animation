#nullable enable

using System;
using System.Linq;
using System.Reflection;
using com.IvanMurzak.Unity.MCP.Animation;
using com.IvanMurzak.Unity.MCP.Editor.Tests;
using NUnit.Framework;

namespace com.IvanMurzak.Unity.MCP.Animation.Editor.Tests
{
    public class AnimationToolsTests : BaseTest
    {
        [Test]
        public void AnimationToolTypeHasMcpAttribute()
        {
            Assert.IsTrue(HasAttributeByName(typeof(AnimationTools), "McpPluginToolTypeAttribute"));
        }

        [Test]
        public void AnimatorToolTypeHasMcpAttribute()
        {
            Assert.IsTrue(HasAttributeByName(typeof(AnimatorTools), "McpPluginToolTypeAttribute"));
        }

        [Test]
        public void AnimationToolMethodsAreRegistered()
        {
            var toolMethods = GetToolMethods(typeof(AnimationTools));
            Assert.IsNotEmpty(toolMethods);
        }

        [Test]
        public void AnimatorToolMethodsAreRegistered()
        {
            var toolMethods = GetToolMethods(typeof(AnimatorTools));
            Assert.IsNotEmpty(toolMethods);
        }

        private static MethodInfo[] GetToolMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
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
