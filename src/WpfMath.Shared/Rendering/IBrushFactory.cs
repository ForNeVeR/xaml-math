using WpfMath.Colors;

namespace WpfMath.Rendering;

public interface IBrushFactory
{
    public IBrush FromColor(RgbaColor color);
}
