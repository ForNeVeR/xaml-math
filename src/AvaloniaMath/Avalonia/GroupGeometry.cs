using System.Collections.Generic;
using Avalonia.Media;

namespace WpfMath.Avalonia
{
    public class GeometryGroup : Geometry
    {
        public override Geometry Clone()
        {
            throw new System.NotImplementedException();
        }

        public IList<Geometry> Children { get; set; }
    }
}