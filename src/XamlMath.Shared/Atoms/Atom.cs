using XamlMath.Boxes;

namespace XamlMath.Atoms;

/// <summary>Atom (smallest unit) of TexFormula.</summary>
/// <param name="Source"></param>
/// <param name="Type"></param>
internal abstract record Atom(SourceSpan? Source, TexAtomType Type = TexAtomType.Ordinary)
{
    public Box CreateBox(TexEnvironment environment)
    {
        var box = this.CreateBoxCore(environment);
        if (box.Source == null)
        {
            box.Source = this.Source;
        }

        return box;
    }

    protected abstract Box CreateBoxCore(TexEnvironment environment);

    // Gets type of leftmost child item.
    public virtual TexAtomType GetLeftType()
    {
        return this.Type;
    }

    // Gets type of leftmost child item.
    public virtual TexAtomType GetRightType()
    {
        return this.Type;
    }
}
