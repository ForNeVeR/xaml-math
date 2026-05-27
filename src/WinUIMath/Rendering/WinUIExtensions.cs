using Microsoft.UI.Xaml.Media;

using System.Diagnostics.CodeAnalysis;

using Windows.UI;

using XamlMath.Colors;
using XamlMath.Rendering;

namespace WinUIMath.Rendering;

public static class WinUIExtensions
{
    [return: NotNullIfNotNull(nameof(brush))]
    public static IBrush? ToPlatform(this Brush? brush) => brush == null ? null : WinUIBrush.FromBrush(brush);

    public static RgbaColor ToPlatform(this Color color) => RgbaColor.FromArgb(color.A, color.R, color.G, color.B);

    public static Windows.Foundation.Point ToWinUI(this Point point) => new((float) point.X, (float) point.Y);
}
