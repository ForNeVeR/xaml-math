using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;

namespace WpfMath.Colors
{
    internal class RgbColorParser : FixedComponentCountColorParser
    {
        public RgbColorParser() : base(3)
        {
        }

        protected override Color? ParseComponents(List<string> components)
        {
            var rgb = components.Select(x =>
            {
                var success = byte.TryParse(x, NumberStyles.None, CultureInfo.InvariantCulture, out var val);
                return success ? (byte?) val : null;
            }).ToArray();
            var r = rgb[0];
            var g = rgb[1];
            var b = rgb[2];
            return r == null || g == null || b == null ? (Color?) null : Color.FromRgb(r.Value, g.Value, b.Value);
        }
    }
}
