using System.Drawing.Text;
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
    private readonly TextRenderingHint _textRenderingHintToRestore;
    private float Scale => 30f * _graphics.DpiX / 96f; // TODO: Figure out what to do with sizes properly

    public WinFormsRenderer(Graphics graphics)
    {
        _graphics = graphics;
        _textRenderingHintToRestore = _graphics.TextRenderingHint;
        _graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
    }

    public void RenderElement(Box box, double x, double y)
    {
        // TODO: RenderBackground(box, x, y);
        box.RenderTo(this, x, y);
    }

    private static readonly StringFormat typographicFormat = StringFormat.GenericTypographic; // TODO: Dispose

    public void RenderCharacter(CharInfo info, double x, double y, IBrush? foreground)
    {
        var font = ((WinFormsGlyphTypeface)info.Font).Font;
        var newF = new Font(font.FontFamily, (float)info.Size * Scale, GraphicsUnit.Pixel);
        var brush = foreground.ToWinForms() ?? Brushes.Black; // TODO: Make IBrush disposable?

        // Renderer wants upper left corner from us, while we have baseline here. Let's convert.
        var ff = newF.FontFamily;
        var lineSpace = ff.GetLineSpacing(font.Style);
        var ascent = ff.GetCellAscent(font.Style);
        var fontBaseline = newF.GetHeight(_graphics.DpiX) * ascent / lineSpace;

        var baselineOriginX = x * Scale;
        var baselineOriginY = y * Scale;
        var topY = baselineOriginY - fontBaseline;

        // NOTE: Passing of TypographicFormat here is important. Otherwise, the X alignment of the font may be
        // incorrect. See this for details: https://github.com/ForNeVeR/xaml-math/issues/281#issuecomment-1427149022
        _graphics.DrawString(
            info.Character.ToString(),
            newF,
            brush,
            (float)baselineOriginX,
            (float)topY,
            typographicFormat);
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
        _graphics.TextRenderingHint = _textRenderingHintToRestore;
    }
}
