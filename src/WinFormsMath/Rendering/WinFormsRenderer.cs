using System.Runtime.InteropServices;
using WinFormsMath.Fonts;
using XamlMath;
using XamlMath.Boxes;
using XamlMath.Rendering;
using XamlMath.Rendering.Transformations;
using Rectangle = XamlMath.Rendering.Rectangle;

namespace WinFormsMath.Rendering;

public class WinFormsRenderer : IElementRenderer
{
    private readonly Graphics _graphics;
    private float Scale => 20f * _graphics.DpiX / 96f; // TODO: Figure out what to do with sizes properly

    public WinFormsRenderer(Graphics graphics)
    {
        _graphics = graphics;
    }

    public void RenderElement(Box box, double x, double y)
    {
        // TODO: RenderBackground(box, x, y);
        box.RenderTo(this, x, y);
    }

    [DllImport("gdi32.dll")]
    public static extern int GetTextCharacterExtra(
        IntPtr hdc    // DC handle
    );

    public void RenderCharacter(CharInfo info, double x, double y, IBrush? foreground)
    {
        var font = ((WinFormsGlyphTypeface)info.Font).Font;
        var newF = new Font(font.FontFamily, (float)info.Size * Scale, GraphicsUnit.Pixel);
        var brush = foreground.ToWinForms() ?? Brushes.Black; // TODO: Make IBrush disposable?

        int mm;
        var hdc = _graphics.GetHdc();
        try
        {
            mm = GetTextCharacterExtra(hdc);
        }
        finally
        {
            _graphics.ReleaseHdc(hdc);
        }

        var metric = _graphics.MeasureString(info.Character.ToString(), newF);

        // Renderer wants upper left corner from us, while we have baseline here. Let's convert.
        var ff = newF.FontFamily;
        var lineSpace = ff.GetLineSpacing(font.Style);
        var ascent = ff.GetCellAscent(font.Style);
        var fontBaseline = newF.GetHeight(_graphics.DpiX) * ascent / lineSpace;

        var baselineX = x * Scale; // TODO: It looks like we should subtract several pixels here for some reason,
                                   //       I don't understand why. Probably a font metric issue. It's possible
                                   //       that we won't be able to get the right metrics using WinForms without
                                   //       falling back to DirectX?
        var baselineY = y * Scale;
        var topY = baselineY - fontBaseline;


        _graphics.DrawString(info.Character.ToString(), newF, brush, (float)baselineX, (float)topY);
    }

    public void RenderRectangle(Rectangle rectangle, IBrush? foreground)
    {
        var brush = foreground.ToWinForms() ?? Brushes.Black; // TODO: Make IBrush disposable?
        _graphics.FillRectangle(brush, GeometryHelper.ScaleRectangle(Scale, rectangle).ToWinForms());
    }

    public void RenderTransformed(Box box, IEnumerable<Transformation> transforms, double x, double y)
    {
        // TODO: Implement this correctly. For now, Avalonia ignores this, so I assume it's safe for us to ignore it as
        // well.
        RenderElement(box, x, y);
    }

    public void FinishRendering()
    {
    }
}
