using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Globalization;
using System.Diagnostics;

namespace WpfMath.Converters
{
    public class SVGConverter
    {
        private int m_nestedLevel = 0;

        public string ConvertGeometry(Geometry geometry)
        {
            StringBuilder svgOutput = new StringBuilder();
            if (geometry is GeometryGroup group)
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
                else if (geometry is LineGeometry)
                {
                    LineGeometry lineGeometry = (LineGeometry)geometry;
                    AddGeometry(svgString, lineGeometry);
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

        private void AddGeometry(StringBuilder svgString, LineGeometry line)
        {
            var x1 = line.StartPoint.X.ToString(CultureInfo.InvariantCulture);
            var y1 = line.StartPoint.Y.ToString(CultureInfo.InvariantCulture);

            var x2 = line.EndPoint.X.ToString(CultureInfo.InvariantCulture);
            var y2 = line.EndPoint.Y.ToString(CultureInfo.InvariantCulture);

            svgString.AppendLine(@$"<line x1=""{x1}"" y1=""{y1}"" x2=""{x2}"" y2=""{y2}"" style=""stroke:black;stroke-width:1"" />");
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
                    if (ps is PolyLineSegment plSeg)
                    {
                        for (int i = 0; i < plSeg.Points.Count; ++i)
                        {
                            svgString.Append("L ");
                            svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", plSeg.Points[i].X);
                            svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", plSeg.Points[i].Y);
                        }
                    }
                    else if (ps is PolyBezierSegment pbSeg)
                    {
                        for (int i = 0; i < pbSeg.Points.Count; i += 3)
                        {
                            svgString.Append("C ");
                            svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", pbSeg.Points[i].X);
                            svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", pbSeg.Points[i].Y);

                            svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", pbSeg.Points[i + 1].X);
                            svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", pbSeg.Points[i + 1].Y);

                            svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", pbSeg.Points[i + 2].X);
                            svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", pbSeg.Points[i + 2].Y);
                        }
                    }
                    else if (ps is LineSegment lSeg)
                    {
                        svgString.Append("L ");
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", lSeg.Point.X);
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", lSeg.Point.Y);
                    }
                    else if (ps is BezierSegment bSeg)
                    {
                        svgString.Append("C ");
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", bSeg.Point1.X);
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", bSeg.Point1.Y);

                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", bSeg.Point2.X);
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", bSeg.Point2.Y);

                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", bSeg.Point3.X);
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", bSeg.Point3.Y);
                    }
                    else if (ps is QuadraticBezierSegment qbSeg)
                    {
                        //Untested: BuildGeometry converts quadratic bezier to cubic
                        svgString.Append("Q ");
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", qbSeg.Point1.X);
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", qbSeg.Point1.Y);

                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", qbSeg.Point2.X);
                        svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", qbSeg.Point2.Y);
                    }
                    else if (ps is PolyQuadraticBezierSegment pqbSeg)
                    {
                        //Untested: BuildGeometry converts quadratic bezier to cubic
                        for (int i = 0; i < pqbSeg.Points.Count; i += 2)
                        {
                            svgString.Append("Q ");
                            svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", pqbSeg.Points[i].X);
                            svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", pqbSeg.Points[i].Y);

                            svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", pqbSeg.Points[i + 1].X);
                            svgString.AppendFormat(CultureInfo.InvariantCulture, "{0} ", pqbSeg.Points[i + 1].Y);
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
}
