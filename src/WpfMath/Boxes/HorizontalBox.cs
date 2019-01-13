using System;
using System.Windows.Media;
using WpfMath.Rendering;

namespace WpfMath.Boxes
{
    // Box containing horizontal stack of child boxes.
    internal class HorizontalBox : Box
    {
        private double childBoxesTotalWidth = 0.0;

        public HorizontalBox(Box box, double width, TexAlignment alignment)
            : this()
        {
            var extraWidth = width - box.Width;
            if (alignment == TexAlignment.Center)
            {
                var strutBox = new StrutBox(extraWidth / 2, 0, 0, 0);
                this.Add(strutBox);
                this.Add(box);
                this.Add(strutBox);
            }
            else if (alignment == TexAlignment.Left)
            {
                this.Add(box);
                this.Add(new StrutBox(extraWidth, 0, 0, 0));
            }
            else if (alignment == TexAlignment.Right)
            {
                this.Add(new StrutBox(extraWidth, 0, 0, 0));
                this.Add(box);
            }
        }

        public HorizontalBox(Box box)
            : this()
        {
            this.Add(box);
        }

        public HorizontalBox(Brush foreground, Brush background)
            : base(foreground, background)
        {
        }

        public HorizontalBox()
            : base()
        {
        }

        public override void Add(Box box)
        {
            base.Add(box);

            this.childBoxesTotalWidth += box.Width;
            this.Width = Math.Max(this.Width, this.childBoxesTotalWidth);
            this.Height = Math.Max((this.Children.Count == 0 ? double.NegativeInfinity : this.Height), box.Height - box.Shift);
            this.Depth = Math.Max((this.Children.Count == 0 ? double.NegativeInfinity : this.Depth), box.Depth + box.Shift);
            this.Italic = Math.Max((this.Children.Count == 0 ? double.NegativeInfinity : this.Italic), box.Italic);
        }

        public override void RenderTo(IElementRenderer renderer, double x, double y)
        {
            var curX = x;
            foreach (var box in this.Children)
            {
                renderer.RenderElement(box, curX, y + box.Shift);
                curX += box.Width;
            }
        }

        public override int GetLastFontId()
        {
            var fontId = TexFontUtilities.NoFontId;
            foreach (var child in this.Children)
            {
                fontId = child.GetLastFontId();
                if (fontId == TexFontUtilities.NoFontId)
                    break;
            }
            return fontId;
        }
    }
}
