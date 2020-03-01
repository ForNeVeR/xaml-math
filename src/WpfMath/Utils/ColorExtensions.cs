namespace WpfMath.Utils
{
    using System.Windows.Media;
    using WpfMath.Colors;

    internal static class ColorExtensions
    {
        public static Color ToWpfColor(this ArgbColor value) =>
            Color.FromArgb(value.A, value.R, value.G, value.B);

        public static ArgbColor ToInternalColor(this Color value) =>
            new ArgbColor(value.A, value.R, value.G, value.B);
    }
}
