namespace AvaloniaMath.Utils
{
    using Avalonia.Media;
    using WpfMath.Colors;

    internal static class ColorExtensions
    {
        public static Color ToAvaloniaColor(this RgbaColor value) =>
            Color.FromArgb(value.A, value.R, value.G, value.B);

        public static RgbaColor ToInternalColor(this Color value) =>
            new RgbaColor(value.R, value.G, value.B, value.A);
    }
}
