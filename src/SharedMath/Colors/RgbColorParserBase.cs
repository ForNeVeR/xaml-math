using System.Collections.Generic;
using WpfMath.Utils;

namespace WpfMath.Colors
{
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
            => ColorHelpers.TryParseRgbColor(
                components,
                _alphaChannelMode,
                DefaultAlpha,
                ParseColorComponent,
                GetByteValue,
                out var color)
                ? color
                : (RgbaColor?)null;
    }
}
