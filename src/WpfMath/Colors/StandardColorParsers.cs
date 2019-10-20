using System.Collections.Generic;

namespace WpfMath.Colors
{
    internal static class StandardColorParsers
    {
        public static IReadOnlyDictionary<string, IColorParser> Dictionary = new Dictionary<string, IColorParser>
        {
            ["gray"] = new GrayscaleColorParser(),
            ["HTML"] = new HtmlColorParser(),
            ["RGB"] = new RgbColorParser()
        };
    }
}
