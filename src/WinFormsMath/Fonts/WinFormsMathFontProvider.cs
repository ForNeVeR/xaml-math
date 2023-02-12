using System.Drawing.Text;
using XamlMath.Fonts;
using XamlMath.Utils;

namespace WinFormsMath.Fonts;

/// <summary>A font provider implementation specifically for the WinFormsMath assembly.</summary>
internal class WinFormsMathFontProvider : IFontProvider
{
    private WinFormsMathFontProvider() {}

    public static WinFormsMathFontProvider Instance = new();

    private const string FontsDirectory = "WinFormsMath.Fonts";

    public unsafe IFontTypeface ReadFontFile(string fontFileName)
    {
        using var resource = typeof(WinFormsMathFontProvider).Assembly.ReadResource($"{FontsDirectory}.{fontFileName}");
        using var byteStream = new MemoryStream();
        resource.CopyTo(byteStream);
        var bytes = byteStream.ToArray();

        var c = new PrivateFontCollection(); // TODO: Dispose?
        fixed (byte* p = bytes)
            c.AddMemoryFont((IntPtr)p, bytes.Length);

        var ff = c.Families.Single();
        return new WinFormsGlyphTypeface(new Font(ff, 1.0f));
    }
}
