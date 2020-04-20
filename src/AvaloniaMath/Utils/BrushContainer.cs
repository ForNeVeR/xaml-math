using Avalonia.Media;
using WpfMath.Colors;

namespace WpfMath.Utils
{
    internal class BrushContainer : IBrushContainer
    {
        public object Value { get; }

        private BrushContainer(Brush value) => Value = value;

        public static IBrushContainer FromBrush(Brush value) =>
            new BrushContainer(value);

        public static IBrushContainer FromColor(RgbaColor value) =>
            new BrushContainer(
                new SolidColorBrush(
                    Color.FromArgb(value.A, value.R, value.G, value.B)));
    }
}
