using System;
using XamlMath.Boxes;

namespace XamlMath.Atoms;

/// <summary>
/// Atom representing radical (nth-root) construction.
/// </summary>
internal sealed record Radical : Atom
{
    private const string sqrtSymbol = "sqrt";

    private const double scale = 0.55;

    public Radical(SourceSpan? source, Atom baseAtom, Atom? degreeAtom = null)
        : base(source)
    {
        this.BaseAtom = baseAtom;
        this.DegreeAtom = degreeAtom;
    }

    public Atom BaseAtom { get; }

    public Atom? DegreeAtom { get; }

    protected override Box CreateBoxCore(TexEnvironment environment)
    {
        var texFont = environment.MathFont;
        var style = environment.Style;

        // Calculate minimum clearance amount.
        double clearance;
        var defaultRuleThickness = texFont.GetDefaultLineThickness(style);
        if (style < TexStyle.Text)
            clearance = texFont.GetXHeight(style, texFont.GetCharInfo(sqrtSymbol, style).Value.FontId);
        else
            clearance = defaultRuleThickness;
        clearance = defaultRuleThickness + Math.Abs(clearance) / 4;

        // Create box for base atom, in cramped style.
        var baseBox = this.BaseAtom.CreateBox(environment.GetCrampedStyle());

        // Create box for radical sign.
        var totalHeight = baseBox.Height + baseBox.Depth;
        var radicalSignBox = DelimiterFactory.CreateBox(sqrtSymbol, totalHeight + clearance + defaultRuleThickness,
            environment, Source);
        radicalSignBox.Source = Source;

        // Add half of excess height to clearance.
        var delta = radicalSignBox.Depth - (totalHeight + clearance);
        clearance += delta / 2;

        // Create box for square-root containing base box.
        radicalSignBox.Shift = -(baseBox.Height + clearance);
        var overBar = new OverBar(environment, baseBox, clearance, radicalSignBox.Height);
        overBar.Shift = -(baseBox.Height + clearance + defaultRuleThickness);
        var radicalContainerBox = new HorizontalBox(radicalSignBox);
        radicalContainerBox.Add(overBar);

        // If atom is simple radical, just return square-root box.
        if (this.DegreeAtom == null)
            return radicalContainerBox;

        // Atom is complex radical (nth-root).

        // Create box for root atom.
        var rootBox = this.DegreeAtom.CreateBox(environment.GetRootStyle());
        var bottomShift = scale * (radicalContainerBox.Height + radicalContainerBox.Depth);
        rootBox.Shift = radicalContainerBox.Depth - rootBox.Depth - bottomShift;

        // Create result box.
        var resultBox = new HorizontalBox();

        // Add box for negative kern.
        var negativeKern = new SpaceAtom(null, TexUnit.Mu, -10, 0, 0).CreateBox(environment);
        var xPos = rootBox.Width + negativeKern.Width;
        if (xPos < 0)
            resultBox.Add(new StrutBox(-xPos, 0, 0, 0));

        resultBox.Add(rootBox);
        resultBox.Add(negativeKern);
        resultBox.Add(radicalContainerBox);

        return resultBox;
    }
}
