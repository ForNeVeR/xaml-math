using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Globalization;

namespace WpfMath
{
    public class SVGConverter
    {
        private int m_nestedLevel = 0;

        public string ConvertGeometry(Geometry geometry)
        {
            StringBuilder svgOutput = new StringBuilder();
            GeometryGroup group = geometry as GeometryGroup;
            if (group != null)
            {
                AddGeometry(svgOutput, group);
            }
            return svgOutput.ToString();
        }

        private void AddGeometry(StringBuilder svgString, GeometryGroup group)
        {
            m_nestedLevel++;
            if (!group.Transform.Value.IsIdentity)
            {
                svgString.AppendFormat(CultureInfo.InvariantCulture, "<g matrix({0} {1} {2} {3} {4} {5} {6})>"
                    , group.Transform.Value.M11, group.Transform.Value.M12, group.Transform.Value.OffsetX
                    , group.Transform.Value.M21, group.Transform.Value.M22, group.Transform.Value.OffsetY);
            }
            foreach (Geometry geometry in group.Children)
            {
                if (geometry is GeometryGroup)
                {
                    GeometryGroup childGroup = (GeometryGroup)geometry;
                    AddGeometry(svgString, childGroup);
                }
                else if (geometry is PathGeometry)
                {
                    PathGeometry path = (PathGeometry)geometry;
                    AddGeometry(svgString, path);
                }
                else if (geometry is RectangleGeometry)
                {
                    RectangleGeometry rectangle = (RectangleGeometry)geometry;
                    AddGeometry(svgString, rectangle);
                }
            }
            if (!group.Transform.Value.IsIdentity)
            {
                svgString.AppendLine("</g>");
                svgString.AppendLine(Environment.NewLine);
            }

            m_nestedLevel--;
        }

        private void AddGeometry(StringBuilder svgString, PathGeometry path)
        {
            svgString.Append("<path d=\"");
            foreach (PathFigure pf in path.Figures)
            {
                foreach (PathSegment ps in pf.Segments)
                {
                    PolyLineSegment seg = ps as PolyLineSegment;
                    svgString.Append("M ");
                    svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[0].X);
                    svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[0].Y);
                    for (int i = 1; i < seg.Points.Count; ++i)
                    {
                        svgString.Append("L ");
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[i].X);
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[i].Y);
                    }
                    svgString.Append("Z ");
                }
            }
            svgString.Append("\" fill = \"black\" />");
            svgString.Append(Environment.NewLine);
        }

        private void AddGeometry(StringBuilder svgString, RectangleGeometry rectangle)
        {
            svgString.AppendFormat(CultureInfo.InvariantCulture, "<rect x=\"{0}\" y=\"{1}\" width=\"{2}\" height=\"{3}\" />"
                , rectangle.Rect.Left, rectangle.Rect.Top, rectangle.Rect.Width, rectangle.Rect.Height);
            svgString.Append(Environment.NewLine);
        }
    }
}