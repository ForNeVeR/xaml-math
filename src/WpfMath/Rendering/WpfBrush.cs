using System.Windows.Media;
using XamlMath.Colors;
using XamlMath.Rendering;

namespace WpfMath.Rendering;

public sealed record WpfBrush : GenericBrush<Brush>
{
    private WpfBrush(Brush brush) : base(brush)
    {
    }

    public static WpfBrush FromBrush(Brush value) => new(value);
}

public sealed class WpfBrushFactory : IBrushFactory
{
    public static readonly WpfBrushFactory Instance = new();

    private WpfBrushFactory() {}

    public IBrush FromColor(RgbaColor color) =>
        new SolidColorBrush(
            Color.FromArgb(color.A, color.R, color.G, color.B)).ToPlatform();
}
