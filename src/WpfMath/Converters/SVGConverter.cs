using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Media;

namespace WpfMath.Converters;

public class SVGConverter
{
    public string ConvertGeometry(Geometry geometry)
    {
        if (geometry is not GeometryGroup group) return string.Empty;
        return string.Join(string.Empty, AddGeometry(group));
    }

    private static IEnumerable<string> AddGeometry(GeometryGroup group)
    {
        if (!group.Transform.Value.IsIdentity)
        {
            yield return string.Format(
                CultureInfo.InvariantCulture,
                "<g transform=\"matrix({0} {1} {2} {3} {4} {5})\">",
                group.Transform.Value.M11, group.Transform.Value.M12,
                group.Transform.Value.M21, group.Transform.Value.M22,
                group.Transform.Value.OffsetX,
                group.Transform.Value.OffsetY
            );
        }
        foreach (Geometry geometry in group.Children)
        {
            switch (geometry)
            {
                case GeometryGroup childGroup:
                    foreach (string str in AddGeometry(childGroup)) yield return str;
                    break;
                case LineGeometry lineGeometry:
                    foreach (string str in AddGeometry(lineGeometry)) yield return str;
                    break;
                case PathGeometry path:
                    foreach (string str in AddGeometry(path)) yield return str;
                    break;
                case RectangleGeometry rectangle:
                    foreach (string str in AddGeometry(rectangle)) yield return str;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }
        if (!group.Transform.Value.IsIdentity)
        {
            yield return "</g>";
            yield return Environment.NewLine;
            yield return Environment.NewLine;
            yield return Environment.NewLine;
        }
    }

    private static IEnumerable<string> AddGeometry(LineGeometry line)
    {
        var x1 = line.StartPoint.X.ToString(CultureInfo.InvariantCulture);
        var y1 = line.StartPoint.Y.ToString(CultureInfo.InvariantCulture);

        var x2 = line.EndPoint.X.ToString(CultureInfo.InvariantCulture);
        var y2 = line.EndPoint.Y.ToString(CultureInfo.InvariantCulture);

        yield return @$"<line x1=""{x1}"" y1=""{y1}"" x2=""{x2}"" y2=""{y2}"" style=""stroke:black;stroke-width:1"" />";
        yield return Environment.NewLine;
    }

    private static IEnumerable<string> AddGeometry(PathGeometry path)
    {
        yield return "<path d=\"";

        foreach (PathFigure pf in path.Figures)
        {
            yield return "M ";
            yield return string.Format(CultureInfo.InvariantCulture, "{0} ", pf.StartPoint.X);
            yield return string.Format(CultureInfo.InvariantCulture, "{0} ", pf.StartPoint.Y);

            foreach (PathSegment ps in pf.Segments)
            {
                switch (ps)
                {
                    case PolyLineSegment plSeg:
                        {
                            for (int i = 0; i < plSeg.Points.Count; ++i)
                            {
                                yield return "L ";
                                yield return string.Format(CultureInfo.InvariantCulture, "{0} ", plSeg.Points[i].X);
                                yield return string.Format(CultureInfo.InvariantCulture, "{0} ", plSeg.Points[i].Y);
                            }
                        }
                        break;
                    case PolyBezierSegment pbSeg:
                        {
                            for (int i = 0; i < pbSeg.Points.Count; i += 3)
                            {
                                yield return "C ";
                                yield return string.Format(CultureInfo.InvariantCulture, "{0} ", pbSeg.Points[i].X);
                                yield return string.Format(CultureInfo.InvariantCulture, "{0} ", pbSeg.Points[i].Y);

                                yield return string.Format(CultureInfo.InvariantCulture, "{0} ", pbSeg.Points[i + 1].X);
                                yield return string.Format(CultureInfo.InvariantCulture, "{0} ", pbSeg.Points[i + 1].Y);

                                yield return string.Format(CultureInfo.InvariantCulture, "{0} ", pbSeg.Points[i + 2].X);
                                yield return string.Format(CultureInfo.InvariantCulture, "{0} ", pbSeg.Points[i + 2].Y);
                            }
                        }
                        break;
                    case LineSegment lSeg:
                        {
                            yield return "L ";
                            yield return string.Format(CultureInfo.InvariantCulture, "{0} ", lSeg.Point.X);
                            yield return string.Format(CultureInfo.InvariantCulture, "{0} ", lSeg.Point.Y);
                        }
                        break;
                    case BezierSegment bSeg:
                        {
                            yield return "C ";
                            yield return string.Format(CultureInfo.InvariantCulture, "{0} ", bSeg.Point1.X);
                            yield return string.Format(CultureInfo.InvariantCulture, "{0} ", bSeg.Point1.Y);

                            yield return string.Format(CultureInfo.InvariantCulture, "{0} ", bSeg.Point2.X);
                            yield return string.Format(CultureInfo.InvariantCulture, "{0} ", bSeg.Point2.Y);

                            yield return string.Format(CultureInfo.InvariantCulture, "{0} ", bSeg.Point3.X);
                            yield return string.Format(CultureInfo.InvariantCulture, "{0} ", bSeg.Point3.Y);
                        }
                        break;
                    case QuadraticBezierSegment qbSeg:
                        {
                            //Untested: BuildGeometry converts quadratic bezier to cubic

                            yield return "Q ";
                            yield return string.Format(CultureInfo.InvariantCulture, "{0} ", qbSeg.Point1.X);
                            yield return string.Format(CultureInfo.InvariantCulture, "{0} ", qbSeg.Point1.Y);

                            yield return string.Format(CultureInfo.InvariantCulture, "{0} ", qbSeg.Point2.X);
                            yield return string.Format(CultureInfo.InvariantCulture, "{0} ", qbSeg.Point2.Y);
                        }
                        break;
                    case PolyQuadraticBezierSegment pqbSeg:
                        {
                            //Untested: BuildGeometry converts quadratic bezier to cubic
                            for (int i = 0; i < pqbSeg.Points.Count; i += 2)
                            {
                                yield return "Q ";
                                yield return string.Format(CultureInfo.InvariantCulture, "{0} ", pqbSeg.Points[i].X);
                                yield return string.Format(CultureInfo.InvariantCulture, "{0} ", pqbSeg.Points[i].Y);

                                yield return string.Format(CultureInfo.InvariantCulture, "{0} ", pqbSeg.Points[i + 1].X);
                                yield return string.Format(CultureInfo.InvariantCulture, "{0} ", pqbSeg.Points[i + 1].Y);
                            }
                        }
                        break;
                    default:
                        {
                            Debug.Assert(false);        //If asserted, implement segment type
                        }
                        break;
                }
            }

            if (pf.IsClosed)
                yield return "Z ";
        }
        yield return "\" fill = \"black\" />";
        yield return Environment.NewLine;
    }

    private static IEnumerable<string> AddGeometry(RectangleGeometry rectangle)
    {
        yield return string.Format(
            CultureInfo.InvariantCulture,
            "<rect x=\"{0}\" y=\"{1}\" width=\"{2}\" height=\"{3}\" />",
            rectangle.Rect.Left,
            rectangle.Rect.Top,
            rectangle.Rect.Width,
            rectangle.Rect.Height
        );
        yield return Environment.NewLine;
    }
}
