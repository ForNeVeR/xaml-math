using System;
using XamlMath.Boxes;
#if !NET462 && !NETSTANDARD2_0
using System.Diagnostics.CodeAnalysis;
#endif

namespace XamlMath.Atoms;

// Atom representing other atom with atoms optionally over and under it.
internal sealed record UnderOverAtom : Atom
{
#if !NET462 && !NETSTANDARD2_0
    [return: NotNullIfNotNull("box")]
#endif
    private static Box? ChangeWidth(Box? box, double maxWidth)
    {
        if (box != null && Math.Abs(maxWidth - box.Width) > TexUtilities.FloatPrecision)
            return new HorizontalBox(box, maxWidth, TexAlignment.Center);
        else
            return box;
    }

    public UnderOverAtom(
        SourceSpan? source,
        Atom? baseAtom,
        Atom? underOver,
        TexUnit underOverUnit,
        double underOverSpace,
        bool underOverScriptSize,
        bool over)
        : base(source)
    {
        SpaceAtom.CheckUnit(underOverUnit);

        this.BaseAtom = baseAtom;

        if (over)
        {
            this.UnderAtom = null;
            this.UnderSpace = 0;
            this.UnderSpaceUnit = 0;
            this.UnderScriptSmaller = false;
            this.OverAtom = underOver;
            this.OverSpaceUnit = underOverUnit;
            this.OverSpace = underOverSpace;
            this.OverScriptSmaller = underOverScriptSize;
        }
        else
        {
            this.UnderAtom = underOver;
            this.UnderSpaceUnit = underOverUnit;
            this.UnderSpace = underOverSpace;
            this.UnderScriptSmaller = underOverScriptSize;
            this.OverSpace = 0;
            this.OverAtom = null;
            this.OverSpaceUnit = 0;
            this.OverScriptSmaller = false;
        }
    }

    public UnderOverAtom(
        SourceSpan? source,
        Atom? baseAtom,
        Atom? under,
        TexUnit underUnit,
        double underSpace,
        bool underScriptSize,
        Atom? over,
        TexUnit overUnit,
        double overSpace,
        bool overScriptSize)
        : base(source)
    {
        SpaceAtom.CheckUnit(underUnit);
        SpaceAtom.CheckUnit(overUnit);

        this.BaseAtom = baseAtom;
        this.UnderAtom = under;
        this.UnderSpaceUnit = underUnit;
        this.UnderSpace = underSpace;
        this.UnderScriptSmaller = underScriptSize;
        this.OverAtom = over;
        this.OverSpaceUnit = overUnit;
        this.OverSpace = overSpace;
        this.OverScriptSmaller = overScriptSize;
    }

    public Atom? BaseAtom { get; }

    public Atom? UnderAtom { get; }

    public Atom? OverAtom { get; }

    // Kern between base and under atom.
    public double UnderSpace { get; }

    // Kern between base and over atom.
    public double OverSpace { get; }

    public TexUnit UnderSpaceUnit { get; }

    public TexUnit OverSpaceUnit { get; }

    public bool UnderScriptSmaller { get; }

    public bool OverScriptSmaller { get; }

    protected override Box CreateBoxCore(TexEnvironment environment)
    {
        // Create box for base atom.
        var baseBox = this.BaseAtom == null ? StrutBox.Empty : this.BaseAtom.CreateBox(environment);

        // Create boxes for over and under atoms.
        Box? overBox = null, underBox = null;
        var maxWidth = baseBox.Width;

        if (this.OverAtom != null)
        {
            overBox = this.OverAtom.CreateBox(this.OverScriptSmaller ? environment.GetSubscriptStyle() : environment);
            maxWidth = Math.Max(maxWidth, overBox.Width);
        }

        if (this.UnderAtom != null)
        {
            underBox = this.UnderAtom.CreateBox(this.UnderScriptSmaller ? environment.GetSubscriptStyle() : environment);
            maxWidth = Math.Max(maxWidth, underBox.Width);
        }

        // Create result box.
        var resultBox = new VerticalBox();

        environment.LastFontId = baseBox.GetLastFontId();

        // Create and add box for over atom.
        if (this.OverAtom != null)
        {
            resultBox.Add(ChangeWidth(overBox!, maxWidth));
            resultBox.Add(new SpaceAtom(null, this.OverSpaceUnit, 0, this.OverSpace, 0).CreateBox(environment));
        }

        // Add box for base atom.
        resultBox.Add(ChangeWidth(baseBox, maxWidth));

        double totalHeight = resultBox.Height + resultBox.Depth - baseBox.Depth;

        // Create and add box for under atom.
        if (this.UnderAtom != null)
        {
            resultBox.Add(new SpaceAtom(null, this.OverSpaceUnit, 0, this.UnderSpace, 0).CreateBox(environment));
            resultBox.Add(ChangeWidth(underBox!, maxWidth));
        }

        resultBox.Depth = resultBox.Height + resultBox.Depth - totalHeight;
        resultBox.Height = totalHeight;

        return resultBox;
    }
}
