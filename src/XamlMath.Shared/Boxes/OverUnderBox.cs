using XamlMath.Rendering;
using XamlMath.Rendering.Transformations;

namespace XamlMath.Boxes;

// Box representing other box with delimiter and script box over or under it.
internal sealed class OverUnderBox : Box
{
    public OverUnderBox(Box baseBox, Box delimiterBox, Box? scriptBox, double kern, bool over)
        : base()
    {
        this.BaseBox = baseBox;
        this.DelimiterBox = delimiterBox;
        this.ScriptBox = scriptBox;
        this.Kern = kern;
        this.Over = over;

        // Calculate dimensions of box.
        this.Width = baseBox.Width;
        this.Height = baseBox.Height + (over ? delimiterBox.Width : 0.0) +
            (over && scriptBox != null ? scriptBox.Height + scriptBox.Depth + kern : 0.0);
        this.Depth = baseBox.Depth + (over ? 0.0 : delimiterBox.Width) +
            (!over && scriptBox != null ? scriptBox.Height + scriptBox.Depth + kern : 0.0);
    }

    public Box BaseBox { get; }

    public Box DelimiterBox { get; }

    public Box? ScriptBox { get; }

    // Kern between delimeter and Script.
    public double Kern { get; }

    // True to draw delimeter and script over base; false to draw under base.
    public bool Over { get; }

    private record struct RenderChoices(
        double TranslationX,
        double TranslationY,
        double YPosition);
    private RenderChoices CreateRenderChoices(double x, double y)
    {
        if (this.Over)
        {
            // Draw delimeter and script boxes over base box.
            var centerY = y - this.BaseBox.Height - this.DelimiterBox.Width;
            var translationX = x + this.DelimiterBox.Width / 2;
            var translationY = centerY + this.DelimiterBox.Width / 2;

            return new(

                TranslationX: translationX,
                TranslationY: translationY,

                // Draw script box as superscript.
                YPosition: centerY - this.Kern - this.ScriptBox!.Depth // Nullable TODO: This probably needs null checking
            );
        }
        else
        {
            // Draw delimeter and script boxes under base box.
            var centerY = y + this.BaseBox.Depth + this.DelimiterBox.Width;
            var translationX = x + this.DelimiterBox.Width / 2;
            var translationY = centerY - this.DelimiterBox.Width / 2;

            return new(

                TranslationX: translationX,
                TranslationY: translationY,

                // Draw script box as subscript.
                YPosition: centerY + this.Kern + this.ScriptBox!.Height // Nullable TODO: This probably needs null checking
            );
        }
    }

    public override void RenderTo(IElementRenderer renderer, double x, double y)
    {
        var renderChoices = CreateRenderChoices(x, y);

        renderer.RenderElement(this.BaseBox, x, y);

        RenderDelimiter(renderChoices.TranslationX, renderChoices.TranslationY);
        RenderScriptBox(renderChoices.YPosition);

        void RenderDelimiter(double translationX, double translationY)
        {
            var transformations = new Transformation[]
            {
                new Transformation.Translate(translationX, translationY),
                new Transformation.Rotate(90)
            };

            renderer.RenderTransformed(
                this.DelimiterBox,
                transformations,
                -this.DelimiterBox.Width / 2,
                -this.DelimiterBox.Depth + this.DelimiterBox.Width / 2);
        }

        void RenderScriptBox(double yPosition)
        {
            if (this.ScriptBox != null)
            {
                renderer.RenderElement(this.ScriptBox, x, yPosition);
            }
        }
    }

    public override int GetLastFontId()
    {
        return TexFontUtilities.NoFontId;
    }
}
