using Microsoft.UI.Xaml.Media;

using System;
using System.Diagnostics;

using static WinUIMath.Rendering.WinUIMatrixTypeExtensions;

namespace WinUIMath.Rendering;

/// <remarks>
/// <a href="https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/WindowsBase/System/Windows/Media/Matrix.cs" />
/// </remarks>
public static class WinUIMatrixCalculationExtensions
{
    public static Matrix Multiply(Matrix trans1, Matrix trans2)
    {
        MultiplyMatrix(ref trans1, ref trans2);
        return trans1;
    }

    /// <summary>
    /// Append - "this" becomes this * matrix, the same as this *= matrix.
    /// </summary>
    /// <param name="matrix"> The Matrix to append to this Matrix </param>
    public static void Append(this Matrix self, Matrix matrix)
    {
        self.SetMatrix(Multiply(self, matrix));
    }

    /// <summary>
    /// Prepend - "this" becomes matrix * this, the same as this = matrix * this.
    /// </summary>
    /// <param name="matrix"> The Matrix to prepend to this Matrix </param>
    public static void Prepend(this Matrix self, Matrix matrix)
    {
        self.SetMatrix(Multiply(matrix, self));
    }

    /// <summary>
    /// Rotates this matrix about the origin
    /// </summary>
    /// <param name='angle'>The angle to rotate specified in degrees</param>
    public static void Rotate(this Matrix self, double angle)
    {
        angle %= 360.0; // Doing the modulo before converting to radians reduces total error
        self.SetMatrix(Multiply(self, CreateRotationRadians(angle * (Math.PI / 180.0))));
    }

    /// <summary>
    /// Prepends a rotation about the origin to "this"
    /// </summary>
    /// <param name='angle'>The angle to rotate specified in degrees</param>
    public static void RotatePrepend(this Matrix self, double angle)
    {
        angle %= 360.0; // Doing the modulo before converting to radians reduces total error
        self.SetMatrix(Multiply(CreateRotationRadians(angle * (Math.PI / 180.0)), self));
    }

    /// <param name='centerX'>The centerX of rotation</param>
    /// <param name='centerY'>The centerY of rotation</param>
    public static void RotateAt(this Matrix self, double angle, double centerX, double centerY)
    {
        angle %= 360.0; // Doing the modulo before converting to radians reduces total error
        self.SetMatrix(Multiply(self, CreateRotationRadians(angle * (Math.PI / 180.0), centerX, centerY)));
    }

    /// <summary>
    /// Prepends a rotation about the given point to "this"
    /// </summary>
    /// <param name='angle'>The angle to rotate specified in degrees</param>
    /// <param name='centerX'>The centerX of rotation</param>
    /// <param name='centerY'>The centerY of rotation</param>
    public static void RotateAtPrepend(this Matrix self, double angle, double centerX, double centerY)
    {
        angle %= 360.0; // Doing the modulo before converting to radians reduces total error
        self.SetMatrix(Multiply(CreateRotationRadians(angle * (Math.PI / 180.0), centerX, centerY), self));
    }

    /// <summary>
    /// Scales this matrix around the origin
    /// </summary>
    /// <param name='scaleX'>The scale factor in the x dimension</param>
    /// <param name='scaleY'>The scale factor in the y dimension</param>
    public static void Scale(this Matrix self, double scaleX, double scaleY)
    {
        self.SetMatrix(Multiply(self, CreateScaling(scaleX, scaleY)));
    }

    /// <summary>
    /// Prepends a scale around the origin to "this"
    /// </summary>
    /// <param name='scaleX'>The scale factor in the x dimension</param>
    /// <param name='scaleY'>The scale factor in the y dimension</param>
    public static void ScalePrepend(this Matrix self, double scaleX, double scaleY)
    {
        self.SetMatrix(Multiply(CreateScaling(scaleX, scaleY), self));
    }

    /// <summary>
    /// Scales this matrix around the center provided
    /// </summary>
    /// <param name='scaleX'>The scale factor in the x dimension</param>
    /// <param name='scaleY'>The scale factor in the y dimension</param>
    /// <param name="centerX">The centerX about which to scale</param>
    /// <param name="centerY">The centerY about which to scale</param>
    public static void ScaleAt(this Matrix self, double scaleX, double scaleY, double centerX, double centerY)
    {
        self.SetMatrix(Multiply(self, CreateScaling(scaleX, scaleY, centerX, centerY)));
    }

    /// <summary>
    /// Prepends a scale around the center provided to "this"
    /// </summary>
    /// <param name='scaleX'>The scale factor in the x dimension</param>
    /// <param name='scaleY'>The scale factor in the y dimension</param>
    /// <param name="centerX">The centerX about which to scale</param>
    /// <param name="centerY">The centerY about which to scale</param>
    public static void ScaleAtPrepend(this Matrix self, double scaleX, double scaleY, double centerX, double centerY)
    {
        self.SetMatrix(Multiply(CreateScaling(scaleX, scaleY, centerX, centerY), self));
    }

    /// <summary>
    /// Skews this matrix
    /// </summary>
    /// <param name='skewX'>The skew angle in the x dimension in degrees</param>
    /// <param name='skewY'>The skew angle in the y dimension in degrees</param>
    public static void Skew(this Matrix self, double skewX, double skewY)
    {
        skewX %= 360;
        skewY %= 360;
        self.SetMatrix(Multiply(self, CreateSkewRadians(skewX * (Math.PI / 180.0), skewY * (Math.PI / 180.0))));
    }

    /// <summary>
    /// Prepends a skew to this matrix
    /// </summary>
    /// <param name='skewX'>The skew angle in the x dimension in degrees</param>
    /// <param name='skewY'>The skew angle in the y dimension in degrees</param>
    public static void SkewPrepend(this Matrix self, double skewX, double skewY)
    {
        skewX %= 360;
        skewY %= 360;
        self.SetMatrix(Multiply(CreateSkewRadians(skewX * (Math.PI / 180.0), skewY * (Math.PI / 180.0)), self));
    }

    /// <summary>
    /// Translates this matrix
    /// </summary>
    /// <param name='offsetX'>The offset in the x dimension</param>
    /// <param name='offsetY'>The offset in the y dimension</param>
    public static void Translate(this Matrix self, double offsetX, double offsetY)
    {
        //
        // / a b 0 \   / 1 0 0 \    / a      b       0 \
        // | c d 0 | * | 0 1 0 | = |  c      d       0 |
        // \ e f 1 /   \ x y 1 /    \ e+x    f+y     1 /
        //
        // (where e = _offsetX and f == _offsetY)
        //
        MatrixTypes type = self.DeriveMatrixType();
        if (type == MatrixTypes.TRANSFORM_IS_IDENTITY)
        {
            // Values would be incorrect if matrix was created using default constructor.
            // or if SetIdentity was called on a matrix which had values.
            //
            self.SetMatrix(
                1, 0,
                0, 1,
                offsetX, offsetY);
        }
        else if (type == MatrixTypes.TRANSFORM_IS_UNKNOWN)
        {
            self.OffsetX += offsetX;
            self.OffsetY += offsetY;
        }
        else
        {
            self.OffsetX += offsetX;
            self.OffsetY += offsetY;
        }
    }

    /// <summary>
    /// Prepends a translation to this matrix
    /// </summary>
    /// <param name='offsetX'>The offset in the x dimension</param>
    /// <param name='offsetY'>The offset in the y dimension</param>
    public static void TranslatePrepend(this Matrix self, double offsetX, double offsetY)
    {
        self.SetMatrix(Multiply(CreateTranslation(offsetX, offsetY), self));
    }

    private static void SetMatrix(
        this Matrix self,
        double m11, double m12,
        double m21, double m22,
        double offsetX, double offsetY)
    {
        self.M11 = m11;
        self.M12 = m12;
        self.M21 = m21;
        self.M22 = m22;
        self.OffsetX = offsetX;
        self.OffsetY = offsetY;
    }

    private static void SetMatrix(this Matrix self, Matrix matrix)
    {
        self.SetMatrix(
            matrix.M11, matrix.M12,
            matrix.M21, matrix.M22,
            matrix.OffsetX, matrix.OffsetY);
    }

    /// <summary>
    /// Creates a scaling transform around the given point
    /// </summary>
    /// <param name='scaleX'>The scale factor in the x dimension</param>
    /// <param name='scaleY'>The scale factor in the y dimension</param>
    /// <param name='centerX'>The centerX of scaling</param>
    /// <param name='centerY'>The centerY of scaling</param>
    private static Matrix CreateScaling(double scaleX, double scaleY, double centerX, double centerY)
    {
        return new Matrix
        {
            M11 = scaleX, M12 = 0,
            M21 = 0, M22 = scaleY,
            OffsetX = centerX - scaleX * centerX, OffsetY = centerY - scaleY * centerY
        };
    }

    /// <summary>
    /// Creates a scaling transform around the origin
    /// </summary>
    /// <param name='scaleX'>The scale factor in the x dimension</param>
    /// <param name='scaleY'>The scale factor in the y dimension</param>
    private static Matrix CreateScaling(double scaleX, double scaleY)
    {
        return new Matrix
        {
            M11 = scaleX, M12 = 0,
            M21 = 0, M22 = scaleY,
            OffsetX = 0, OffsetY = 0
        };
    }

    /// <summary>
    /// Creates a skew transform
    /// </summary>
    /// <param name='skewX'>The skew angle in the x dimension in degrees</param>
    /// <param name='skewY'>The skew angle in the y dimension in degrees</param>
    private static Matrix CreateSkewRadians(double skewX, double skewY)
    {
        return new Matrix
        {
            M11 = 1, M12 = Math.Tan(skewY),
            M21 = Math.Tan(skewX), M22 = 1,
            OffsetX = 0, OffsetY = 0
        };
    }

    /// <summary>
    /// Sets the transformation to the given translation specified by the offset vector.
    /// </summary>
    /// <param name='offsetX'>The offset in X</param>
    /// <param name='offsetY'>The offset in Y</param>
    private static Matrix CreateTranslation(double offsetX, double offsetY)
    {
        return new Matrix
        {
            M11 = 1, M12 = 0,
            M21 = 0, M22 = 1,
            OffsetX = offsetX, OffsetY = offsetY
        };
    }

    private static Matrix CreateRotationRadians(double angle) => CreateRotationRadians(angle, centerX: 0, centerY: 0);

    /// <summary>
    /// Creates a rotation transformation about the given point
    /// </summary>
    /// <param name='angle'>The angle to rotate specified in radians</param>
    /// <param name='centerX'>The centerX of rotation</param>
    /// <param name='centerY'>The centerY of rotation</param>
    /// <remarks>
    /// <a href="https://github.com/dotnet/wpf/blob/114fbee660df4e981e851cc04a8a557dc7328898/src/Microsoft.DotNet.Wpf/src/WindowsBase/System/Windows/Media/Matrix.cs#L766" />
    /// </remarks>
    private static Matrix CreateRotationRadians(double angle, double centerX, double centerY)
    {
        double sin = Math.Sin(angle);
        double cos = Math.Cos(angle);
        double dx = (centerX * (1.0 - cos)) + (centerY * sin);
        double dy = (centerY * (1.0 - cos)) - (centerX * sin);
        return new Matrix
        {
            M11 = cos, M12 = sin,
            M21 = -sin, M22 = cos,
            OffsetX = dx, OffsetY = dy
        };
    }

    /// <summary>
    /// Multiplies two transformations, where the behavior is matrix1 *= matrix2.
    /// This code exists so that we can efficient combine matrices without copying
    /// the data around, since each matrix is 52 bytes.
    /// To reduce duplication and to ensure consistent behavior, this is the
    /// method which is used to implement Matrix * Matrix as well.
    /// </summary>
    /// <remarks>
    /// <a href="https://github.com/dotnet/wpf/blob/114fbee660df4e981e851cc04a8a557dc7328898/src/Microsoft.DotNet.Wpf/src/Shared/MS/Internal/MatrixUtil.cs#L103" />
    /// </remarks>
    private static void MultiplyMatrix(ref Matrix matrix1, ref Matrix matrix2)
    {
        MatrixTypes type1 = matrix1.DeriveMatrixType();
        MatrixTypes type2 = matrix2.DeriveMatrixType();

        // Check for idents

        // If the second is ident, we can just return
        if (type2 == MatrixTypes.TRANSFORM_IS_IDENTITY)
        {
            return;
        }

        // If the first is ident, we can just copy the memory across.
        if (type1 == MatrixTypes.TRANSFORM_IS_IDENTITY)
        {
            matrix1 = matrix2;
            return;
        }

        // Optimize for translate case, where the second is a translate
        if (type2 == MatrixTypes.TRANSFORM_IS_TRANSLATION)
        {
            // 2 additions
            matrix1.OffsetX += matrix2.OffsetX;
            matrix1.OffsetY += matrix2.OffsetY;
            return;
        }

        // Check for the first value being a translate
        if (type1 == MatrixTypes.TRANSFORM_IS_TRANSLATION)
        {
            // Save off the old offsets
            double offsetX = matrix1.OffsetX;
            double offsetY = matrix1.OffsetY;

            // Copy the matrix
            matrix1 = matrix2;

            matrix1.OffsetX = offsetX * matrix2.M11 + offsetY * matrix2.M21 + matrix2.OffsetX;
            matrix1.OffsetY = offsetX * matrix2.M12 + offsetY * matrix2.M22 + matrix2.OffsetY;
            return;
        }

        // The following code combines the type of the transformations so that the high nibble
        // is "this"'s type, and the low nibble is mat's type.  This allows for a switch rather
        // than nested switches.

        // trans1._type |  trans2._type
        //  7  6  5  4   |  3  2  1  0
        int combinedType = ((int) type1 << 4) | (int) type2;

        switch (combinedType)
        {
            case 34:  // S * S
                      // 2 multiplications
                matrix1.M11 *= matrix2.M11;
                matrix1.M22 *= matrix2.M22;
                return;

            case 35:  // S * S|T
                matrix1.M11 *= matrix2.M11;
                matrix1.M22 *= matrix2.M22;
                matrix1.OffsetX = matrix2.OffsetX;
                matrix1.OffsetY = matrix2.OffsetY;
                return;

            case 50: // S|T * S
                matrix1.M11 *= matrix2.M11;
                matrix1.M22 *= matrix2.M22;
                matrix1.OffsetX *= matrix2.M11;
                matrix1.OffsetY *= matrix2.M22;
                return;

            case 51: // S|T * S|T
                matrix1.M11 *= matrix2.M11;
                matrix1.M22 *= matrix2.M22;
                matrix1.OffsetX = matrix2.M11 * matrix1.OffsetX + matrix2.OffsetX;
                matrix1.OffsetY = matrix2.M22 * matrix1.OffsetY + matrix2.OffsetY;
                return;
            case 36: // S * U
            case 52: // S|T * U
            case 66: // U * S
            case 67: // U * S|T
            case 68: // U * U
                matrix1 = new Matrix(
                    matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21,
                    matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22,

                    matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21,
                    matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22,

                    matrix1.OffsetX * matrix2.M11 + matrix1.OffsetY * matrix2.M21 + matrix2.OffsetX,
                    matrix1.OffsetX * matrix2.M12 + matrix1.OffsetY * matrix2.M22 + matrix2.OffsetY);
                return;
#if DEBUG
            default:
                Debug.Fail("Matrix multiply hit an invalid case: " + combinedType);
                break;
#endif
        }
    }
}
