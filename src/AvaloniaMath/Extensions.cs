/* TODO[#355]
using Avalonia.Media.Imaging;
using System.IO;

namespace WpfMath;

public static class Extensions
{
    public static byte[] RenderToPng(this TexFormula texForm,
        double scale,
        double x,
        double y,
        string systemTextFontName)
    {
        var trnder = texForm.GetRenderer(TexStyle.Display, scale, systemTextFontName);
        BitmapSource image = trnder.RenderToBitmap(x, y);

        PngBitmapEncoder encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(image));

        using var ms = new MemoryStream();

        encoder.Save(ms);
        return ms.ToArray();
    }
}
*/
