using Windows.UI;
using Windows.UI.Xaml.Media;

using XamlMath.Colors;
using XamlMath.Rendering;

namespace UwpMath.Rendering;

public sealed record UwpBrush : GenericBrush<Brush>
{
    private UwpBrush(Brush brush) : base(brush)
    {
    }

    public static UwpBrush FromBrush(Brush value) => new(value);
}

public sealed class UwpBrushFactory : IBrushFactory
{
    public static UwpBrushFactory Instance => field ??= new();

    public IBrush FromColor(RgbaColor color)
    {
        return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B)).ToPlatform();
    }
}
