using System.Collections.Generic;

namespace WpfMath.Colors
{
    internal static class StandardColorParsers
    {
        public static IReadOnlyDictionary<string, IColorParser> Dictionary = new Dictionary<string, IColorParser>
        {
            ["HTML"] = new HtmlColorParser()
        };
    }
}
