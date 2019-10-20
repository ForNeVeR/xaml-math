using System.Collections.Generic;

namespace WpfMath.Colors
{
    internal static class StandardColorParsers
    {
        public static IReadOnlyDictionary<string, IColorParser> Dictionary = new Dictionary<string, IColorParser>
        {
            ["ARGB"] = new IntegerRgbColorParser(true),
            ["argb"] = new FloatRgbColorParser(true),
            ["cmyk"] = new CmykColorParser(),
            ["gray"] = new GrayscaleColorParser(),
            ["HTML"] = new HtmlColorParser(),
            ["RGB"] = new IntegerRgbColorParser(false),
            ["rgb"] = new FloatRgbColorParser(false)
        };
    }
}
