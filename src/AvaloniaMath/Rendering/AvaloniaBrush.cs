using Avalonia.Media;
using WpfMath.Colors;
using WpfMath.Utils;

namespace AvaloniaMath.Rendering
{
    internal class AvaloniaBrush : GenericPlatformBrush<IBrush>
    {
        private AvaloniaBrush(IBrush brush) : base(brush) {}

        public static AvaloniaBrush FromBrush(Brush value) =>
            new AvaloniaBrush(value);

        public static AvaloniaBrush FromColor(RgbaColor value) =>
            new AvaloniaBrush(
                new SolidColorBrush(
                    Color.FromArgb(value.A, value.R, value.G, value.B)));
    }
}
