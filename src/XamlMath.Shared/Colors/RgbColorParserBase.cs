using System.Collections.Generic;
using System.Linq;

namespace XamlMath.Colors;

/// <summary>A generic parser class for RGB color.</summary>
/// <typeparam name="T">Type of component value (e.g. integer or double).</typeparam>
internal abstract class RgbColorParserBase<T> : FixedComponentCountColorParser where T : struct
{
    private readonly AlphaChannelMode _alphaChannelMode;

    protected RgbColorParserBase(AlphaChannelMode alphaChannelMode)
        : base(alphaChannelMode == AlphaChannelMode.None ? 3 : 4)
    {
        _alphaChannelMode = alphaChannelMode;
    }

    protected abstract T DefaultAlpha { get; }

    protected abstract T? ParseColorComponent(string component);
    protected abstract byte GetByteValue(T val);

    protected override RgbaColor? ParseComponents(IReadOnlyList<string> components)
    {
        var values = components
            .Select(ParseColorComponent)
            .ToArray();

        var index = 0;
        var alpha = _alphaChannelMode == AlphaChannelMode.AlphaFirst
            ? values[index++]
            : DefaultAlpha;

        var r = values[index++];
        var g = values[index++];
        var b = values[index++];

        if (_alphaChannelMode == AlphaChannelMode.AlphaLast)
            alpha = values[index];

        if (!(alpha.HasValue && r.HasValue && g.HasValue && b.HasValue))
            return null;

        var color = new RgbaColor
        {
            R = GetByteValue(r.Value),
            G = GetByteValue(g.Value),
            B = GetByteValue(b.Value),
            A = GetByteValue(alpha.Value),
        };
        return color;
    }
}
