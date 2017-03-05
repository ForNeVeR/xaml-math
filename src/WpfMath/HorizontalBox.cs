using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfMath
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
                Add(strutBox);
                Add(box);
                Add(strutBox);
            }
            else if (alignment == TexAlignment.Left)
            {
                Add(box);
                Add(new StrutBox(extraWidth, 0, 0, 0));
            }
            else if (alignment == TexAlignment.Right)
            {
                Add(new StrutBox(extraWidth, 0, 0, 0));
                Add(box);
            }
        }

        public HorizontalBox(Box box)
            : this()
        {
            Add(box);
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

            childBoxesTotalWidth += box.Width;
            this.Width = Math.Max(this.Width, childBoxesTotalWidth);
            this.Height = Math.Max((this.Children.Count == 0 ? double.NegativeInfinity : Height), box.Height - box.Shift);
            this.Depth = Math.Max((this.Children.Count == 0 ? double.NegativeInfinity : Depth), box.Depth + box.Shift);
            this.Italic = Math.Max((this.Children.Count == 0 ? double.NegativeInfinity : Italic), box.Italic);
        }

        public override void RenderGeometry(GeometryGroup geometry, double scale, double x, double y)
        {
            base.RenderGeometry(geometry, scale, x, y);

            var curX = x;
            foreach (var box in this.Children)
            {
                box.RenderGeometry(geometry, scale, curX, y + box.Shift);
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
