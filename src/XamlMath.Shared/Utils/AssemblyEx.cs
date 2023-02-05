using System;
using System.IO;
using System.Reflection;

namespace XamlMath.Utils;

internal static class AssemblyEx
{
    public static Stream ReadResource(this Assembly assembly, string resourceName) =>
        assembly.GetManifestResourceStream(resourceName)
        ?? throw new Exception($"Cannot find resource {resourceName} in assembly {assembly}.");
}
