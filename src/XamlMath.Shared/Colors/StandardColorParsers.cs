using System.Collections.Generic;

namespace XamlMath.Colors;

public static class StandardColorParsers
{
    public static readonly IReadOnlyDictionary<string, IColorParser> Dictionary = new Dictionary<string, IColorParser>
    {
        ["ARGB"] = new IntegerRgbColorParser(AlphaChannelMode.AlphaFirst),
        ["argb"] = new FloatRgbColorParser(AlphaChannelMode.AlphaFirst),
        ["cmyk"] = new CmykColorParser(),
        ["gray"] = new GrayscaleColorParser(),
        ["HTML"] = new HtmlColorParser(),
        ["RGB"] = new IntegerRgbColorParser(AlphaChannelMode.None),
        ["RGBA"] = new IntegerRgbColorParser(AlphaChannelMode.AlphaLast),
        ["rgb"] = new FloatRgbColorParser(AlphaChannelMode.None),
        ["rgba"] = new FloatRgbColorParser(AlphaChannelMode.AlphaLast)
    };
}
