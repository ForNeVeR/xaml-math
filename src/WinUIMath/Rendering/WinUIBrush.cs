using Microsoft.UI.Xaml.Media;

using Windows.UI;

using XamlMath.Colors;
using XamlMath.Rendering;

namespace WinUIMath.Rendering;

public sealed record WinUIBrush : GenericBrush<Brush>
{
    private WinUIBrush(Brush brush) : base(brush)
    {
    }

    public static WinUIBrush FromBrush(Brush value) => new(value);
}

public sealed class WinUIBrushFactory : IBrushFactory
{
    public static WinUIBrushFactory Instance => field ??= new();

    public IBrush FromColor(RgbaColor color)
    {
        return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B)).ToPlatform();
    }
}
