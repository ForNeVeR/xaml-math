using System.Linq;
using XamlMath.Rendering;

namespace XamlMath.Boxes
{
    public class ZBox : Box
    {
        public override void Add(Box box)
        {
            base.Add(box);

            Width = Children.Select(c => c.Width).DefaultIfEmpty(0).Max();
            Height = Children.Select(c => c.Height - c.Shift).DefaultIfEmpty(double.NegativeInfinity).Max();
            Depth = Children.Select(c => c.Depth + c.Shift).DefaultIfEmpty(double.NegativeInfinity).Max(); ;
            Italic = Children.Select(c => c.Italic).DefaultIfEmpty(double.NegativeInfinity).Max(); ;
        }

        public override void RenderTo(IElementRenderer renderer, double x, double y)
        {
            foreach (var box in Children)
            {
                renderer.RenderElement(box, x, y + box.Shift);
            }
        }

        public override int GetLastFontId()
        {
            var fontId = TexFontUtilities.NoFontId;
            foreach (var child in Children)
            {
                fontId = child.GetLastFontId();
                if (fontId == TexFontUtilities.NoFontId)
                    break;
            }
            return fontId;
        }
    }
}
