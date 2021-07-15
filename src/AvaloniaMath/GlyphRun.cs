using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfMath.Rendering
{
    public class GlyphRun
    {
        public Typeface Font { get; set; }
        public double Size { get; set; }
        public Point Position { get; set; }
        public char Character { get; set; }

        public GlyphRun(Typeface typeface, double size, Point position, char character)
        {
            Font = typeface;
            Size = size;
            Position = position;
            Character = character;
        }
    }
    public class GeometryGroup : Geometry
    {
        public override Geometry Clone()
        {
            throw new System.NotImplementedException();
        }

        protected override IGeometryImpl CreateDefiningGeometry()
        {
            throw new System.NotImplementedException();
        }

        public IList<Geometry> Children { get; set; } = new List<Geometry>();
    }
}
