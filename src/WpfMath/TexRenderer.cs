using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfMath.Boxes;
using WpfMath.Rendering;

namespace WpfMath
{
    [Obsolete("Use extension methods on WpfMath.TexFormula instead.")]
    public class TexRenderer // TODO[#340]: Drop in the next release
    {
        /// <summary>Default DPI for WPF.</summary>
        private const int DefaultDpi = 96;

        internal TexRenderer(Box box, double scale)
        {
            this.Box = box;
            this.Scale = scale;
        }

        public Box Box
        {
            get;
            set;
        }

        public double Scale
        {
            get;
            private set;
        }

        public System.Windows.Size RenderSize => new(Box.TotalWidth * Scale, Box.TotalHeight * Scale);

        public double Baseline
        {
            get
            {
                return this.Box.Height / this.Box.TotalHeight * this.Scale;
            }
        }

        [Obsolete("Use WpfMath.Rendering.TeXFormulaExtensions::RenderTo instead.")]
        public void RenderFormulaTo(IElementRenderer renderer, double x, double y)
        {
            TeXFormulaExtensions.Render(Box, renderer, x, y);
        }

        [Obsolete("Use WpfMath.Rendering.WpfTeXFormulaExtensions::RenderToGeometry instead.")]
        public Geometry RenderToGeometry(double x, double y)
        {
            var geometry = new GeometryGroup();
            var renderer = new GeometryElementRenderer(geometry, Scale);
            RenderFormulaTo(renderer, x, y);
            return geometry;
        }

        private void RenderWithPositiveCoordinates(DrawingVisual visual, double x, double y)
        {
            using (var drawingContext = visual.RenderOpen())
                this.Render(drawingContext, x, y);

            var bounds = visual.ContentBounds;
            if (bounds.X >= 0 && bounds.Y >= 0) return;

            using (var drawingContext = visual.RenderOpen())
            {
                drawingContext.PushTransform(
                    new TranslateTransform(Math.Max(0.0, -bounds.X), Math.Max(0.0, -bounds.Y)));
                this.Render(drawingContext, x, y);
            }
        }

        [Obsolete("Use WpfMath.Rendering.WpfTeXFormulaExtensions::RenderToBitmap instead.")]
        public BitmapSource RenderToBitmap(double x, double y, double dpi)
        {
            var visual = new DrawingVisual();
            this.RenderWithPositiveCoordinates(visual, x, y);

            var bounds = visual.ContentBounds;
            var width = (int)Math.Ceiling((bounds.Right + x) * dpi / DefaultDpi);
            var height = (int)Math.Ceiling((bounds.Bottom + y) * dpi / DefaultDpi);
            var bitmap = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Default);
            bitmap.Render(visual);

            return bitmap;
        }

        [Obsolete("Use WpfMath.Rendering.WpfTeXFormulaExtensions::RenderToBitmap instead.")]
        public BitmapSource RenderToBitmap(double x, double y) => this.RenderToBitmap(x, y, DefaultDpi);

        [Obsolete("Use WpfMath.Rendering.WpfTeXFormulaExtensions::RenderTo instead.")]
        public void Render(DrawingContext drawingContext, double x, double y) =>
            RenderFormulaTo(new WpfElementRenderer(drawingContext, Scale), x, y);
    }
}
