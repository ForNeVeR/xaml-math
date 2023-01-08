using System.Windows.Media;
using WpfMath.Colors;

namespace WpfMath.Rendering;

internal record WpfBrush : GenericPlatformBrush<Brush>
{
    private WpfBrush(Brush brush) : base(brush)
    {
    }

    public static WpfBrush FromBrush(Brush value) => new(value);

    public static WpfBrush FromColor(RgbaColor value) =>
        new(
            new SolidColorBrush(
                Color.FromArgb(value.A, value.R, value.G, value.B)));
}
