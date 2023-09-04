namespace XamlMath.Rendering.Transformations;

/// <summary>Geometrical transformation.</summary>
public abstract class Transformation
{
    private Transformation() {}

    /// <summary>Kind of a transformation.</summary>
    public abstract TransformationKind Kind { get; }

    /// <summary>Scale of a transformation.</summary>
    /// <param name="scale">Scale coefficient.</param>
    /// <returns></returns>
    public abstract Transformation Scale(double scale);

    public class Rotate : Transformation
    {
        public override TransformationKind Kind => TransformationKind.Rotate;

        public double RotationDegrees { get; }

        public Rotate(double rotationDegrees) =>
            RotationDegrees = rotationDegrees;

        public override Transformation Scale(double scale) => this;
    }

    public class Translate : Transformation
    {
        public override TransformationKind Kind => TransformationKind.Translate;

        public double X { get; }
        public double Y { get; }

        public Translate(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override Transformation Scale(double scale) => new Translate(X * scale, Y * scale);
    }
}
