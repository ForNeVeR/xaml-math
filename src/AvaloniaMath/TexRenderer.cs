using System;
using System.Windows;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.VisualTree;
using WpfMath.Avalonia;

namespace WpfMath
{
    public class TexRenderer
    {
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

        public Size RenderSize
        {
            get
            {
                return new Size(this.Box.TotalWidth * this.Scale, this.Box.TotalHeight * this.Scale);
            }
        }

        public double Baseline
        {
            get
            {
                return this.Box.Height / this.Box.TotalHeight * this.Scale;
            }
        }

        public Geometry RenderToGeometry(double x, double y)
        {
            GeometryGroup geometry = new GeometryGroup();
            Box.RenderGeometry(geometry, this.Scale, x / this.Scale, y / this.Scale + this.Box.Height);
            return geometry;
        }
        
        //recover this later

        //public IBitmap RenderToBitmap(double x, double y)
        //{
            
        //    var visual = new Visual(); // it's not clear what do i need to use as IVisual
        //    visual.Render();
        //    var renderer = new ImmediateRenderer(visual);
        //    var context = new DrawingContext(renderer.);
           
            
        //    using (var drawingContext = visual.
        //        this.Render(drawingContext, 0, 0);

        //    var width = (int)Math.Ceiling(this.RenderSize.Width);
        //    var height = (int)Math.Ceiling(this.RenderSize.Height);
        //    var bitmap = new RenderTargetBitmap(width, height);
        //    bitmap.Render(visual);

        //    return bitmap;
        //}

        public void Render(DrawingContext drawingContext, double x, double y)
        {
            Box.DrawWithGuidelines(drawingContext, Scale, x / Scale, y / Scale + Box.Height);
        }
    }
}
