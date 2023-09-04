using XamlMath.Boxes;

namespace XamlMath.Atoms;

internal sealed record CancelAtom : Atom
{
    private readonly Atom? _contentAtom;
    private readonly StrokeBoxMode _strokeBoxMode;

    public CancelAtom(SourceSpan atomSource, Atom? contentAtom, StrokeBoxMode strokeBoxMode) : base(atomSource)
    {
        _contentAtom = contentAtom;
        _strokeBoxMode = strokeBoxMode;
    }

    protected override Box CreateBoxCore(TexEnvironment environment)
    {
        var contentBox = _contentAtom is null ? StrutBox.Empty : _contentAtom.CreateBox(environment);
        var lineBox = new StrokeBox(_strokeBoxMode)
        {
            Height = contentBox.Height,
            Depth = contentBox.Depth,
            Width = contentBox.Width
        };

        var box = new LayeredBox();
        box.Add(contentBox);
        box.Add(lineBox);

        return box;
    }
}
