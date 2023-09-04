/* TODO[#354]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Media;
using System.Globalization;
using System.Diagnostics;

namespace WpfMath.Converters;

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
            svgString.AppendFormat(CultureInfo.InvariantCulture, "<g transform=\"matrix({0} {1} {2} {3} {4} {5})\">"
                , group.Transform.Value.M11, group.Transform.Value.M12
                , group.Transform.Value.M21, group.Transform.Value.M22, group.Transform.Value.OffsetX, group.Transform.Value.OffsetY);
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
            else
            {
                Debug.Assert(false);
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

            svgString.Append("M ");
            svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", pf.StartPoint.X);
            svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", pf.StartPoint.Y);

            foreach (PathSegment ps in pf.Segments)
            {
                if (ps is PolyLineSegment)
                {
                    PolyLineSegment seg = ps as PolyLineSegment;
                    for (int i = 0; i < seg.Points.Count; ++i)
                    {
                        svgString.Append("L ");
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[i].X);
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[i].Y);
                    }
                }
                else if (ps is PolyBezierSegment)
                {
                    PolyBezierSegment seg = ps as PolyBezierSegment;
                    for (int i = 0; i < seg.Points.Count; i += 3)
                    {
                        svgString.Append("C ");
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[i].X);
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[i].Y);

                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[i + 1].X);
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[i + 1].Y);

                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[i + 2].X);
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[i + 2].Y);
                    }
                }
                else if (ps is LineSegment)
                {
                    LineSegment seg = ps as LineSegment;
                    svgString.Append("L ");
                    svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Point.X);
                    svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Point.Y);
                }
                else if (ps is BezierSegment)
                {
                    BezierSegment seg = ps as BezierSegment;
                    svgString.Append("C ");
                    svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Point1.X);
                    svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Point1.Y);

                    svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Point2.X);
                    svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Point2.Y);

                    svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Point3.X);
                    svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Point3.Y);
                }
                else if (ps is QuadraticBezierSegment)
                {
                    //Untested: BuildGeometry converts quadratic bezier to cubic
                    QuadraticBezierSegment seg = ps as QuadraticBezierSegment;
                    svgString.Append("Q ");
                    svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Point1.X);
                    svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Point1.Y);

                    svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Point2.X);
                    svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Point2.Y);
                }
                else if (ps is PolyQuadraticBezierSegment)
                {
                    //Untested: BuildGeometry converts quadratic bezier to cubic
                    PolyQuadraticBezierSegment seg = ps as PolyQuadraticBezierSegment;
                    for (int i = 0; i < seg.Points.Count; i += 2)
                    {
                        svgString.Append("Q ");
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[i].X);
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[i].Y);

                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[i + 1].X);
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", seg.Points[i + 1].Y);
                    }
                }
                else
                {
                    Debug.Assert(false);        //If asserted, implement segment type
                }
            }

            if (pf.IsClosed)
                svgString.Append("Z ");
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
*/
