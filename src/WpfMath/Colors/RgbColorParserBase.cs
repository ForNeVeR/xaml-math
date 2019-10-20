using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace WpfMath.Colors
{
    /// <summary>A generic parser class for RGB color.</summary>
    /// <typeparam name="T">Type of component value (e.g. integer or double).</typeparam>
    internal abstract class RgbColorParserBase<T> : FixedComponentCountColorParser where T : struct
    {
        protected RgbColorParserBase() : base(3)
        {
        }

        protected abstract (bool, T) TryParseComponent(string component);
        protected abstract byte GetByteValue(T val);

        protected override Color? ParseComponents(List<string> components)
        {
            var rgb = components.Select(x =>
            {
                var (success, val) = TryParseComponent(x);
                return success ? (T?) val : null;
            }).ToArray();
            var r = rgb[0];
            var g = rgb[1];
            var b = rgb[2];
            return r == null || g == null || b == null
                ? (Color?) null
                : Color.FromRgb(
                    GetByteValue(r.Value),
                    GetByteValue(g.Value),
                    GetByteValue(b.Value));
        }
    }
}
