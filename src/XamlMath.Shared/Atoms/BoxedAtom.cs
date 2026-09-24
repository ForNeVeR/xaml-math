using XamlMath.Boxes;

namespace XamlMath.Atoms;

/// <summary>An atom representing a formula framed in a box, created by \boxed{...}.</summary>
internal sealed record BoxedAtom : Atom
{
    private const double DefaultRuleThickness = 0.04; // relative to x-height
    private const double DefaultPadding = 0.15; // relative to x-height

    public BoxedAtom(SourceSpan? source, Atom? baseAtom)
        : base(source)
    {
        BaseAtom = baseAtom ?? new NullAtom();
    }

    public Atom BaseAtom { get; }

    protected override Box CreateBoxCore(TexEnvironment environment)
    {
        var baseBox = BaseAtom.CreateBox(environment);
        var xHeight = environment.MathFont.GetXHeight(environment.Style, environment.LastFontId);
        var ruleThickness = DefaultRuleThickness * xHeight;
        var padding = DefaultPadding * xHeight;

        // Total dimensions including padding and rules
        var innerWidth = baseBox.Width + 2 * padding;
        var innerHeight = baseBox.Height + padding + ruleThickness;
        var innerDepth = baseBox.Depth + padding;

        var totalWidth = innerWidth + 2 * ruleThickness;
        var totalHeight = innerHeight + ruleThickness;
        var totalDepth = innerDepth;

        // Build the framed box
        var resultBox = new VerticalBox();

        // Top horizontal rule
        resultBox.Add(new HorizontalRule(environment, ruleThickness, totalWidth, 0));

        // Middle row: left rule + content + right rule
        var middleRow = new HorizontalBox();
        // left vertical rule spans the content height + depth
        middleRow.Add(new HorizontalRule(environment, ruleThickness, ruleThickness, 0));
        middleRow.Add(new StrutBox(ruleThickness, 0, 0, 0)); // gap between left rule and content

        // Content with padding
        var contentBox = new HorizontalBox();
        contentBox.Add(new StrutBox(padding, 0, 0, 0));
        contentBox.Add(baseBox);
        contentBox.Add(new StrutBox(padding, 0, 0, 0));

        // We need the content box to have the correct height + depth
        // so the vertical rules extend the full height
        contentBox.Height = innerHeight;
        contentBox.Depth = innerDepth;
        contentBox.Width = innerWidth;
        middleRow.Add(contentBox);

        middleRow.Add(new StrutBox(ruleThickness, 0, 0, 0)); // gap
        middleRow.Add(new HorizontalRule(environment, ruleThickness, ruleThickness, 0));

        // Set middle row height and depth to match content
        middleRow.Height = innerHeight;
        middleRow.Depth = innerDepth;
        resultBox.Add(middleRow);

        // Bottom horizontal rule
        resultBox.Add(new HorizontalRule(environment, ruleThickness, totalWidth, 0));

        // Make the top and bottom rules span the full width
        // Adjust the result box dimensions
        resultBox.Height = totalHeight;
        resultBox.Depth = totalDepth;
        resultBox.Width = totalWidth;

        return resultBox;
    }

    public override TexAtomType GetLeftType() => TexAtomType.Ordinary;

    public override TexAtomType GetRightType() => TexAtomType.Ordinary;
}
