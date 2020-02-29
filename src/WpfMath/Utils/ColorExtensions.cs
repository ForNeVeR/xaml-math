namespace WpfMath.Utils
{
    using System.Windows.Media;
    using WpfMath.Colors;

    internal static class ColorExtensions
    {
        public static Color ToWpfColor(this RgbaColor value) =>
            Color.FromArgb(value.A, value.R, value.G, value.B);

        public static RgbaColor ToInternalColor(this Color value) =>
            new RgbaColor(value.R, value.G, value.B, value.A);
    }
}
