using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Platform;

namespace WpfMath.Rendering
{
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
