using System.Windows.Media;
using WpfMath.Colors;
using WpfMath.Utils;

namespace WpfMath.Rendering
{
    internal class WpfBrush : GenericPlatformBrush<Brush>
    {
        private WpfBrush(Brush brush) : base(brush) {}

        public static WpfBrush FromBrush(Brush value) =>
            new WpfBrush(value);

        public static WpfBrush FromColor(RgbaColor value) =>
            new WpfBrush(
                new SolidColorBrush(
                    Color.FromArgb(value.A, value.R, value.G, value.B)));
    }
}
