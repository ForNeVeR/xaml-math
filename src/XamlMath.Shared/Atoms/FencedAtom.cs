using System;
using XamlMath.Boxes;

namespace XamlMath.Atoms;

// Atom representing base atom surrounded by delimeters.
internal sealed record FencedAtom : Atom
{
    private const int delimeterFactor = 901;
    private const double delimeterShortfall = 0.5;

    private static void CentreBox(Box box, double axis)
    {
        var totalHeight = box.Height + box.Depth;
        box.Shift = -(totalHeight / 2 - box.Height) - axis;
    }

    public FencedAtom(SourceSpan? source, Atom? baseAtom, SymbolAtom? leftDelimeter, SymbolAtom? rightDelimeter)
        : base(source)
    {
        this.BaseAtom = baseAtom ?? new RowAtom(null);
        this.LeftDelimeter = leftDelimeter;
        this.RightDelimeter = rightDelimeter;
    }

    public Atom BaseAtom { get; }

    private SymbolAtom? LeftDelimeter { get; }

    private SymbolAtom? RightDelimeter { get; }

    protected override Box CreateBoxCore(TexEnvironment environment)
    {
        var texFont = environment.MathFont;
        var style = environment.Style;

        // Create box for base atom.
        var baseBox = this.BaseAtom.CreateBox(environment);

        // Create result box.
        var resultBox = new HorizontalBox();
        var axis = texFont.GetAxisHeight(style);
        var delta = Math.Max(baseBox.Height - axis, baseBox.Depth + axis);
        var minHeight = Math.Max((delta / 500) * delimeterFactor, 2 * delta - delimeterShortfall);

        // Create and add box for left delimeter.
        if (this.LeftDelimeter != null && this.LeftDelimeter.Name != SymbolAtom.EmptyDelimiterName)
        {
            var leftDelimeterBox = DelimiterFactory.CreateBox(this.LeftDelimeter.Name, minHeight, environment);
            leftDelimeterBox.Source = this.LeftDelimeter.Source;
            CentreBox(leftDelimeterBox, axis);
            resultBox.Add(leftDelimeterBox);
        }

        // add glueElement between left delimeter and base Atom, unless base Atom is whitespace.
        if (this.BaseAtom is not SpaceAtom)
            resultBox.Add(Glue.CreateBox(TexAtomType.Opening, this.BaseAtom.GetLeftType(), environment));

        // add box for base Atom.
        resultBox.Add(baseBox);

        // add glueElement between right delimeter and base Atom, unless base Atom is whitespace.
        if (this.BaseAtom is not SpaceAtom)
            resultBox.Add(Glue.CreateBox(this.BaseAtom.GetRightType(), TexAtomType.Closing, environment));

        // Create and add box for right delimeter.
        if (this.RightDelimeter != null && this.RightDelimeter.Name != SymbolAtom.EmptyDelimiterName)
        {
            var rightDelimeterBox = DelimiterFactory.CreateBox(this.RightDelimeter.Name, minHeight, environment);
            rightDelimeterBox.Source = this.RightDelimeter.Source;
            CentreBox(rightDelimeterBox, axis);
            resultBox.Add(rightDelimeterBox);
        }

        return resultBox;
    }

    public override TexAtomType GetLeftType()
    {
        return TexAtomType.Opening;
    }

    public override TexAtomType GetRightType()
    {
        return TexAtomType.Closing;
    }
}
