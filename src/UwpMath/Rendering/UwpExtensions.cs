using System.Diagnostics.CodeAnalysis;

using Windows.UI;
using Windows.UI.Xaml.Media;

using XamlMath.Colors;
using XamlMath.Rendering;

namespace UwpMath.Rendering;

internal static class UwpExtensions
{
    [return: NotNullIfNotNull(nameof(brush))]
    public static IBrush? ToPlatform(this Brush? brush) => brush == null ? null : UwpBrush.FromBrush(brush);

    public static RgbaColor ToPlatform(this Color color) => RgbaColor.FromArgb(color.A, color.R, color.G, color.B);

    public static Windows.Foundation.Point ToUwp(this Point point) => new((float)point.X, (float)point.Y);
}
