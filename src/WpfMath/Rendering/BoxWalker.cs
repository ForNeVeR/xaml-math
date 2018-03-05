using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using WpfMath.Rendering.Transformations;

namespace WpfMath.Rendering
{
    internal class BoxWalker : IElementRenderer
    {
        private readonly IDictionary<Box, Rect> _boundsDic;
        private readonly double _scale;
        private readonly Func<Box, bool> _filter;

        public BoxWalker(IDictionary<Box, Rect> boundsDic, double scale, Func<Box, bool> filter)
        {
            _boundsDic = boundsDic;
            _scale = scale;
            _filter = filter;
        }

        public void RenderElement(Box box, double x, double y)
        {
            if (_filter(box))
            {
                if (_boundsDic.TryGetValue(box, out Rect rect))
                {
                    rect.Union(new Rect(x * _scale, y * _scale, box.Width * _scale, box.Height * _scale));
                    _boundsDic[box] = rect;
                }
                else
                {
                    _boundsDic.Add(box, new Rect(x * _scale, y * _scale, box.Width * _scale, box.Height * _scale));
                }
            }

            box.RenderTo(this, x, y);
        }

        public void RenderGlyphRun(Func<double, GlyphRun> scaledGlyphFactory, double x, double y, Brush foreground)
        {
        }

        public void RenderRectangle(Rect rectangle, Brush foreground)
        {
        }

        public void RenderTransformed(Box box, Transformation[] transforms, double x, double y)
        {
        }
    }
}
