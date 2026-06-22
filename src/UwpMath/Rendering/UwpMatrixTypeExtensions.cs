using System;

using Windows.UI.Xaml.Media;

namespace UwpMath.Rendering;

public static class UwpMatrixTypeExtensions
{
    [Flags]
    public enum MatrixTypes
    {
        TRANSFORM_IS_IDENTITY = 0b000,
        TRANSFORM_IS_TRANSLATION = 0b001,
        TRANSFORM_IS_SCALING = 0b010,
        TRANSFORM_IS_UNKNOWN = 0b100
    }

    /// <summary>
    /// Set the type of the matrix based on its current contents
    /// </summary>
    /// <remarks>
    /// <a href="https://github.com/dotnet/wpf/blob/114fbee660df4e981e851cc04a8a557dc7328898/src/Microsoft.DotNet.Wpf/src/WindowsBase/System/Windows/Media/Matrix.cs#L891" />
    /// </remarks>
    public static MatrixTypes DeriveMatrixType(this Matrix matrix)
    {
        MatrixTypes type = 0;
        if (!(matrix.M21 == 0 && matrix.M12 == 0))
        {
            type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            return type;
        }
        if (!(matrix.M11 == 1 && matrix.M22 == 1))
        {
            type = MatrixTypes.TRANSFORM_IS_SCALING;
        }
        if (!(matrix.OffsetX == 0 && matrix.OffsetY == 0))
        {
            type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
        }
        if (0 == (type & (MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING)))
        {
            // We have an identity matrix.
            type = MatrixTypes.TRANSFORM_IS_IDENTITY;
        }
        return type;
    }
}
