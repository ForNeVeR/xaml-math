using System.Windows.Media;
using WpfMath.Colors;

namespace WpfMath.Rendering;

public record WpfBrush : GenericBrush<Brush>
{
    private WpfBrush(Brush brush) : base(brush)
    {
    }

    public static WpfBrush FromBrush(Brush value) => new(value);
}

public class WpfBrushFactory : IBrushFactory
{
    // TODO[#359]: private constructor
    // TODO[#359]: readonly property instead of a writeable field
    public static WpfBrushFactory Instance = new();
    public IBrush FromColor(RgbaColor color) =>
        new SolidColorBrush(
            Color.FromArgb(color.A, color.R, color.G, color.B)).ToPlatform();
}
