using System;
using XamlMath.Boxes;
using XamlMath.Fonts;

namespace XamlMath.Atoms;

/// <summary>Atom representing big operator with optional limits.</summary>
/// <param name="UseVerticalLimits">True if limits should be drawn over and under the base atom; false if they should be drawn as scripts.</param>
internal sealed record BigOperatorAtom(
    SourceSpan? Source,
    Atom? BaseAtom,
    Atom? LowerLimitAtom,
    Atom? UpperLimitAtom,
    bool? UseVerticalLimits = null) : Atom(Source, TexAtomType.BigOperator)
{
    private static Box ChangeWidth(Box box, double maxWidth)
    {
        // Centre specified box in new box of specified width, if necessary.
        if (Math.Abs(maxWidth - box.Width) > TexUtilities.FloatPrecision)
            return new HorizontalBox(box, maxWidth, TexAlignment.Center);
        else
            return box;
    }

    protected override Box CreateBoxCore(TexEnvironment environment)
    {
        if ((this.UseVerticalLimits.HasValue && !this.UseVerticalLimits.Value) ||
            (!this.UseVerticalLimits.HasValue && environment.Style >= TexStyle.Text))
            // Attach atoms for limits as scripts.
            return new ScriptsAtom(this.Source, this.BaseAtom, this.LowerLimitAtom, this.UpperLimitAtom)
                .CreateBox(environment);

        BoxForBaseAtom boxForBaseAtom = CreateBoxForBaseAtom(environment);
        BoxesForUpperAndLowerLimits limits = CreateBoxesForUpperAndLowerLimits(environment);
        FontStyleBundle fontStyleBundle = CreateFontStyleBundle(environment);

        // Make all component boxes equally wide.
        var maxWidth = GetMaxWidth(boxForBaseAtom, limits);
        var adjustedBoxForBaseAtom = boxForBaseAtom.AdjustedToMaxWidth(maxWidth);
        var adjustedLimits = limits.AdjustedToMaxWidth(maxWidth);

        return CreateResultBox(fontStyleBundle, adjustedBoxForBaseAtom, adjustedLimits);
    }

    private Box CreateResultBox(FontStyleBundle fontStyleBundle, BoxForBaseAtom boxForBaseAtom, BoxesForUpperAndLowerLimits limits)
    {
        var texFont = fontStyleBundle.TexFont;
        var style = fontStyleBundle.Syle;

        var resultBox = new VerticalBox();
        var opSpacing5 = texFont.GetBigOpSpacing5(style);
        var kern = 0d;

        // Create and add box for upper limit.
        if (UpperLimitAtom != null)
        {
            resultBox.Add(new StrutBox(0, opSpacing5, 0, 0));
            limits.UpperLimitBox!.Shift = boxForBaseAtom.Delta / 2;
            resultBox.Add(limits.UpperLimitBox);
            kern = Math.Max(texFont.GetBigOpSpacing1(style), texFont.GetBigOpSpacing3(style) -
                limits.UpperLimitBox.Depth);
            resultBox.Add(new StrutBox(0, kern, 0, 0));
        }

        // Add box for base atom.
        resultBox.Add(boxForBaseAtom.BaseBox);

        // Create and add box for lower limit.
        if (LowerLimitAtom != null)
        {
            resultBox.Add(new StrutBox(0, Math.Max(texFont.GetBigOpSpacing2(style), texFont.GetBigOpSpacing4(style) -
                limits.LowerLimitBox!.Height), 0, 0));
            limits.LowerLimitBox.Shift = -boxForBaseAtom.Delta / 2;
            resultBox.Add(limits.LowerLimitBox);
            resultBox.Add(new StrutBox(0, opSpacing5, 0, 0));
        }

        // Adjust height and depth of result box.
        var baseBoxHeight = boxForBaseAtom.BaseBox.Height;
        var totalHeight = resultBox.Height + resultBox.Depth;
        if (limits.UpperLimitBox != null)
            baseBoxHeight += opSpacing5 + kern + limits.UpperLimitBox.Height + limits.UpperLimitBox.Depth;
        resultBox.Height = baseBoxHeight;
        resultBox.Depth = totalHeight - baseBoxHeight;

        return resultBox;
    }

    private readonly record struct FontStyleBundle(ITeXFont TexFont, TexStyle Syle);
    private static FontStyleBundle CreateFontStyleBundle(TexEnvironment environment) => new(environment.MathFont, environment.Style);

    private static double GetMaxWidth(BoxForBaseAtom boxForBaseAtom, BoxesForUpperAndLowerLimits limits)
    {
        var val1 = Math.Max(boxForBaseAtom.BaseBox.Width, limits.UpperLimitBox == null ? 0 : limits.UpperLimitBox.Width);
        var val2 = limits.LowerLimitBox == null ? 0 : limits.LowerLimitBox.Width;
        return Math.Max(val1, val2);
    }

    private readonly record struct BoxesForUpperAndLowerLimits(Box? UpperLimitBox, Box? LowerLimitBox)
    {
        internal BoxesForUpperAndLowerLimits AdjustedToMaxWidth(double maxWidth)
        {
            Box? newUpperLimitBox = UpperLimitBox == null ? null : ChangeWidth(UpperLimitBox, maxWidth);
            Box? newLowerLimitBox = LowerLimitBox == null ? null : ChangeWidth(LowerLimitBox, maxWidth);
            return new(newUpperLimitBox, newLowerLimitBox);
        }
    }
    private BoxesForUpperAndLowerLimits CreateBoxesForUpperAndLowerLimits(TexEnvironment environment)
    {
        Box? upperLimitBox = (this.UpperLimitAtom == null)
            ? null
            : this.UpperLimitAtom.CreateBox(environment.GetSuperscriptStyle());
        Box? lowerLimitBox = (this.LowerLimitAtom == null)
            ? null
            : this.LowerLimitAtom.CreateBox(environment.GetSubscriptStyle());
        return new(upperLimitBox, lowerLimitBox);
    }

    private readonly record struct BoxForBaseAtom(Box BaseBox, double Delta)
    {
        internal BoxForBaseAtom AdjustedToMaxWidth(double maxWidth) =>
            this with { BaseBox = ChangeWidth(BaseBox, maxWidth) };
    }
    private BoxForBaseAtom CreateBoxForBaseAtom(TexEnvironment environment)
    {
        if (BaseAtom is SymbolAtom symbolAtom && BaseAtom.Type == TexAtomType.BigOperator)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;

            // Find character of best scale for operator symbol.
            var opChar = texFont.GetCharInfo(symbolAtom.Name, style).Value;
            if (style < TexStyle.Text && texFont.HasNextLarger(opChar))
                opChar = texFont.GetNextLargerCharInfo(opChar, style);
            var charBox = new CharBox(environment, opChar) { Source = BaseAtom.Source };
            charBox.Shift = -(charBox.Height + charBox.Depth) / 2 -
                environment.MathFont.GetAxisHeight(environment.Style);
            Box baseBox = new HorizontalBox(charBox);

            double delta = opChar.Metrics.Italic;
            if (delta > TexUtilities.FloatPrecision)
                baseBox.Add(new StrutBox(delta, 0, 0, 0));
            return new(baseBox, delta);
        }
        else
        {
            Box baseBox = new HorizontalBox(BaseAtom == null ? StrutBox.Empty : BaseAtom.CreateBox(environment));
            double delta = 0;
            return new(baseBox, delta);
        }

    }
}
