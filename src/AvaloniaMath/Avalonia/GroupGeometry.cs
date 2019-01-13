using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Platform;

namespace WpfMath.Avalonia
{
    public class GeometryGroup : Geometry
    {
        public override Geometry Clone()
        {
            throw new System.NotImplementedException();
        }

        // TODO[F]: Fix the method when we'll be ready to use it
        protected override IGeometryImpl CreateDefiningGeometry() => throw new System.NotImplementedException();

        public IList<Geometry> Children { get; set; }
    }
}
