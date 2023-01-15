using System.Windows.Media;
using WpfMath.Colors;

namespace WpfMath.Rendering;

internal record WpfBrush : GenericBrush<Brush>
{
    private WpfBrush(Brush brush) : base(brush)
    {
    }

    public static WpfBrush FromBrush(Brush value) => new(value);
}

internal class WpfBrushFactory : IBrushFactory
{
    public static WpfBrushFactory Instance = new();
    public IBrush FromColor(RgbaColor color) =>
        new SolidColorBrush(
            Color.FromArgb(color.A, color.R, color.G, color.B)).ToPlatform();
}
