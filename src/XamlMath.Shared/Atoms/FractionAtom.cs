using XamlMath.Boxes;

namespace XamlMath.Atoms;

// Atom representing fraction, with or without separation line.
internal sealed record FractionAtom : Atom
{
    private static TexAlignment CheckAlignment(TexAlignment alignment)
    {
        if (alignment == TexAlignment.Left || alignment == TexAlignment.Right)
            return alignment;
        else
            return TexAlignment.Center;
    }

    private readonly TexAlignment numeratorAlignment;
    private readonly TexAlignment denominatorAlignment;

    private readonly double lineThickness;
    private readonly TexUnit lineThicknessUnit;

    private readonly bool useDefaultThickness;
    private readonly double? lineRelativeThickness;

    public FractionAtom(
        SourceSpan? source,
        Atom? numerator,
        Atom? denominator,
        double relativeThickness,
        TexAlignment numeratorAlignment,
        TexAlignment denominatorAlignment)
        : this(source, numerator, denominator, true, numeratorAlignment, denominatorAlignment)
    {
        this.lineRelativeThickness = relativeThickness;
    }

    public FractionAtom(
        SourceSpan? source,
        Atom? numerator,
        Atom? denominator,
        bool drawLine,
        TexAlignment numeratorAlignment,
        TexAlignment denominatorAlignment)
        : this(source, numerator, denominator, drawLine)
    {
        this.numeratorAlignment = CheckAlignment(numeratorAlignment);
        this.denominatorAlignment = CheckAlignment(denominatorAlignment);
    }

    public FractionAtom(SourceSpan? source, Atom? numerator, Atom? denominator, bool drawLine)
        : this(source, numerator, denominator, drawLine, TexUnit.Pixel, 0d)
    {
    }

    public FractionAtom(
        SourceSpan? source,
        Atom? numerator,
        Atom? denominator,
        TexUnit unit,
        double thickness,
        TexAlignment numeratorAlignment,
        TexAlignment denominatorAlignment)
        : this(source, numerator, denominator, unit, thickness)
    {
        this.numeratorAlignment = CheckAlignment(numeratorAlignment);
        this.denominatorAlignment = CheckAlignment(denominatorAlignment);
    }

    public FractionAtom(SourceSpan? source, Atom? numerator, Atom? denominator, TexUnit unit, double thickness)
        : this(source, numerator, denominator, false, unit, thickness)
    {
    }

    private FractionAtom(
        SourceSpan? source,
        Atom? numerator,
        Atom? denominator,
        bool useDefaultThickness,
        TexUnit unit,
        double thickness)
        : base(source, TexAtomType.Inner)
    {
        SpaceAtom.CheckUnit(unit);

        this.Numerator = numerator;
        this.Denominator = denominator;
        this.numeratorAlignment = TexAlignment.Center;
        this.denominatorAlignment = TexAlignment.Center;
        this.useDefaultThickness = useDefaultThickness;
        this.lineThicknessUnit = unit;
        this.lineThickness = thickness;
    }

    public Atom? Numerator { get; }

    public Atom? Denominator { get; }

    protected override Box CreateBoxCore(TexEnvironment environment)
    {
        LineThicknessAndHeight lineStyle = GetEffectiveLineHeight(environment);
        NumeratorDenominatorAtoms n_d_atoms = CreateNumeratorAndDenominatorAtoms(environment); // Of equal width
        ShiftUpDown preliminaryShifts = CreatePreliminaryShiftUpDown(environment, lineStyle);
        return CreateResultBox(environment, lineStyle, n_d_atoms, preliminaryShifts);
    }

    private static Box CreateResultBox(TexEnvironment environment, LineThicknessAndHeight lineStyle, NumeratorDenominatorAtoms n_d_atoms, ShiftUpDown preliminaryShifts)
    {
        var texFont = environment.MathFont;
        var style = environment.Style;

        // Create result box.
        var resultBox = new VerticalBox();

        // add box for numerator.
        resultBox.Add(n_d_atoms.NumeratorBox);

        // Calculate clearance and adjust shift amounts.
        var axis = texFont.GetAxisHeight(style);

        if (lineStyle.LineHeight > 0)
        {
            // Draw fraction line.

            // Calculate clearance amount.
            double clearance;
            if (style < TexStyle.Text)
                clearance = 3 * lineStyle.LineHeight;
            else
                clearance = lineStyle.LineHeight;

            // Adjust shift amounts.
            var delta = lineStyle.LineHeight / 2;
            var kern1 = preliminaryShifts.ShiftUp - n_d_atoms.NumeratorBox.Depth - (axis + delta);
            var kern2 = axis - delta - (n_d_atoms.DenominatorBox.Height - preliminaryShifts.ShiftDown);
            var delta1 = clearance - kern1;
            var delta2 = clearance - kern2;
            if (delta1 > 0)
            {
                preliminaryShifts = preliminaryShifts with { ShiftUp = preliminaryShifts.ShiftUp + delta1 };
                kern1 += delta1;
            }
            if (delta2 > 0)
            {
                preliminaryShifts = preliminaryShifts with { ShiftDown = preliminaryShifts.ShiftDown + delta2 };
                kern2 += delta2;
            }

            resultBox.Add(new StrutBox(0, kern1, 0, 0));
            resultBox.Add(new HorizontalRule(environment, lineStyle.LineHeight, n_d_atoms.NumeratorBox.Width, 0));
            resultBox.Add(new StrutBox(0, kern2, 0, 0));
        }
        else
        {
            // Do not draw fraction line.

            // Calculate clearance amount.
            double clearance;
            if (style < TexStyle.Text)
                clearance = 7 * lineStyle.DefaultLineThickness;
            else
                clearance = 3 * lineStyle.DefaultLineThickness;

            // Adjust shift amounts.
            var kern = preliminaryShifts.ShiftUp - n_d_atoms.NumeratorBox.Depth - (n_d_atoms.DenominatorBox.Height - preliminaryShifts.ShiftDown);
            var delta = (clearance - kern) / 2;
            if (delta > 0)
            {
                preliminaryShifts = preliminaryShifts with { ShiftUp = preliminaryShifts.ShiftUp + delta };
                preliminaryShifts = preliminaryShifts with { ShiftDown = preliminaryShifts.ShiftDown + delta };
                kern += 2 * delta;
            }

            resultBox.Add(new StrutBox(0, kern, 0, 0));
        }

        // add box for denominator.
        resultBox.Add(n_d_atoms.DenominatorBox);

        // Adjust height and depth of result box.
        resultBox.Height = preliminaryShifts.ShiftUp + n_d_atoms.NumeratorBox.Height;
        resultBox.Depth = preliminaryShifts.ShiftDown + n_d_atoms.DenominatorBox.Depth;

        return resultBox;
    }

    private readonly record struct ShiftUpDown(double ShiftUp, double ShiftDown);
    private static ShiftUpDown CreatePreliminaryShiftUpDown(TexEnvironment environment, LineThicknessAndHeight lineStyle)
    {
        var texFont = environment.MathFont;
        var style = environment.Style;
        double shiftUp, shiftDown;
        if (style < TexStyle.Text)
        {
            shiftUp = texFont.GetNum1(style);
            shiftDown = texFont.GetDenom1(style);
        }
        else
        {
            shiftDown = texFont.GetDenom2(style);
            if (lineStyle.LineHeight > 0)
                shiftUp = texFont.GetNum2(style);
            else
                shiftUp = texFont.GetNum3(style);
        }
        return new(shiftUp, shiftDown);
    }

    private readonly record struct NumeratorDenominatorAtoms(Box NumeratorBox, Box DenominatorBox);
    private NumeratorDenominatorAtoms CreateNumeratorAndDenominatorAtoms(TexEnvironment environment)
    {
        // Create boxes for numerator and demoninator atoms, and make them of equal width.
        var numeratorBox = this.Numerator == null
            ? StrutBox.Empty
            : Numerator.CreateBox(environment.GetNumeratorStyle());
        var denominatorBox = this.Denominator == null
            ? StrutBox.Empty
            : Denominator.CreateBox(environment.GetDenominatorStyle());

        if (numeratorBox.Width < denominatorBox.Width)
        {
            numeratorBox = new HorizontalBox(numeratorBox, denominatorBox.Width, this.numeratorAlignment);
        }
        else
        {
            denominatorBox = new HorizontalBox(denominatorBox, numeratorBox.Width, this.denominatorAlignment);
        }

        return new(numeratorBox, denominatorBox);
    }

    private readonly record struct LineThicknessAndHeight(double DefaultLineThickness, double LineHeight);
    private LineThicknessAndHeight GetEffectiveLineHeight(TexEnvironment environment)
    {
        // set thickness to default if default value should be used
        var defaultLineThickness = environment.MathFont.GetDefaultLineThickness(environment.Style);
        if (this.useDefaultThickness)
        {
            double lineHeight = lineRelativeThickness.HasValue ? lineRelativeThickness.Value * defaultLineThickness : defaultLineThickness;
            return new(defaultLineThickness, lineHeight);
        }
        else
        {
            double lineHeight = (new SpaceAtom(null, this.lineThicknessUnit, 0, this.lineThickness, 0)).CreateBox(environment).Height;
            return new(defaultLineThickness, lineHeight);
        }
    }
}
