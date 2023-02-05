using XamlMath.Colors;

namespace XamlMath.Rendering;

public interface IBrushFactory
{
    IBrush FromColor(RgbaColor color);
}
