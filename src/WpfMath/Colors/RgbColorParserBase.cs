using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace WpfMath.Colors
{
    /// <summary>A generic parser class for RGB color.</summary>
    /// <typeparam name="T">Type of component value (e.g. integer or double).</typeparam>
    internal abstract class RgbColorParserBase<T> : FixedComponentCountColorParser where T : struct
    {
        private readonly bool _supportsAlphaChannel;

        protected RgbColorParserBase(bool supportsAlphaChannel) : base(supportsAlphaChannel ? 4 : 3)
        {
            _supportsAlphaChannel = supportsAlphaChannel;
        }

        protected abstract T DefaultAlpha { get; }

        protected abstract (bool, T) TryParseComponent(string component);
        protected abstract byte GetByteValue(T val);

        protected override Color? ParseComponents(List<string> components)
        {
            var values = components.Select(x =>
            {
                var (success, val) = TryParseComponent(x);
                return success ? (T?) val : null;
            }).ToArray();
            var index = 0;
            T? alpha = DefaultAlpha;
            if (_supportsAlphaChannel)
                alpha = values[index++];

            var r = values[index++];
            var g = values[index++];
            var b = values[index];

            return alpha == null || r == null || g == null || b == null
                ? (Color?) null
                : Color.FromArgb(
                    GetByteValue(alpha.Value),
                    GetByteValue(r.Value),
                    GetByteValue(g.Value),
                    GetByteValue(b.Value));
        }
    }
}
