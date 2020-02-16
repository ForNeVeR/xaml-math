using System.Collections.Generic;
using Avalonia.Media;
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

        protected override Color? ParseComponents(IReadOnlyList<string> components)
            => ColorHelpers.TryParseRgbColor(
                components,
                _alphaChannelMode,
                DefaultAlpha,
                ParseColorComponent,
                GetByteValue,
                out var color)
                ? Color.FromArgb(color.a, color.r, color.g, color.b)
                : (Color?)null;
    }
}
