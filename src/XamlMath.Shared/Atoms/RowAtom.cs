using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using XamlMath.Boxes;

namespace XamlMath.Atoms;

// Atom representing horizontal row of other atoms, separated by glue.
internal sealed record RowAtom : Atom, IRow
{
    // Set of atom types that make previous atom of BinaryOperator type change to Ordinary type.
    private static readonly BitArray binaryOperatorChangeSet;

    // Set of atom types that may need kern, or together with previous atom, be replaced by ligature.
    private static readonly BitArray ligatureKernChangeSet;

    static RowAtom()
    {
        binaryOperatorChangeSet = new BitArray(16);
        binaryOperatorChangeSet.Set((int)TexAtomType.BinaryOperator, true);
        binaryOperatorChangeSet.Set((int)TexAtomType.BigOperator, true);
        binaryOperatorChangeSet.Set((int)TexAtomType.Relation, true);
        binaryOperatorChangeSet.Set((int)TexAtomType.Opening, true);
        binaryOperatorChangeSet.Set((int)TexAtomType.Punctuation, true);

        ligatureKernChangeSet = new BitArray(16);
        ligatureKernChangeSet.Set((int)TexAtomType.Ordinary, true);
        ligatureKernChangeSet.Set((int)TexAtomType.BigOperator, true);
        ligatureKernChangeSet.Set((int)TexAtomType.BinaryOperator, true);
        ligatureKernChangeSet.Set((int)TexAtomType.Relation, true);
        ligatureKernChangeSet.Set((int)TexAtomType.Opening, true);
        ligatureKernChangeSet.Set((int)TexAtomType.Closing, true);
        ligatureKernChangeSet.Set((int)TexAtomType.Punctuation, true);
    }

    public RowAtom(SourceSpan? source, Atom? baseAtom)
        : this(
            source,
            baseAtom is RowAtom rowAtom
                ? (IEnumerable<Atom>)rowAtom.Elements
                : new[] { baseAtom! }) // Nullable: Seems to require some sort of non-null assertion to make the analyzer happy
    {
    }

    public RowAtom(SourceSpan? source)
        : base(source)
    {
        this.Elements = new List<Atom>().AsReadOnly();
    }

    private RowAtom(SourceSpan? source, DummyAtom? previousAtom, ReadOnlyCollection<Atom> elements)
        : base(source)
    {
        this.PreviousAtom = previousAtom;
        this.Elements = elements;
    }

    internal RowAtom(SourceSpan? source, IEnumerable<Atom?> elements)
        : base(source) =>
        this.Elements = elements.Where(x => x != null).ToList().AsReadOnly()!;
    // TODO[F]: Fix this with C# 8 migration: there shouldn't be nullable atoms in this collection

    public DummyAtom? PreviousAtom { get; init; }

    public ReadOnlyCollection<Atom> Elements { get; }

    public Atom WithPreviousAtom(DummyAtom? previousAtom) =>
        this with { PreviousAtom = previousAtom };

    public RowAtom Add(Atom atom)
    {
        var newElements = this.Elements.ToList();
        newElements.Add(atom);
        return new RowAtom(this.Source, this.PreviousAtom, newElements.AsReadOnly());
    }

    private static DummyAtom ChangeAtomToOrdinary(DummyAtom currentAtom, DummyAtom? previousAtom, Atom? nextAtom)
    {
        var type = currentAtom.GetLeftType();
        if (type == TexAtomType.BinaryOperator && (previousAtom == null ||
            binaryOperatorChangeSet[(int)previousAtom.GetRightType()]))
        {
            currentAtom = currentAtom with { Type = TexAtomType.Ordinary };
        }
        else if (nextAtom != null && currentAtom.GetRightType() == TexAtomType.BinaryOperator)
        {
            var nextType = nextAtom.GetLeftType();
            if (nextType == TexAtomType.Relation || nextType == TexAtomType.Closing || nextType == TexAtomType.Punctuation)
                currentAtom = currentAtom with { Type = TexAtomType.Ordinary };
        }

        return currentAtom;
    }

    protected override Box CreateBoxCore(TexEnvironment environment)
    {
        // Create result box.
        var resultBox = new HorizontalBox(environment.Foreground, environment.Background);

        var previousAtom = this.PreviousAtom;

        // Create and add box for each atom in row.
        for (int i = 0; i < this.Elements.Count; i++)
        {
            var curAtom = new DummyAtom(this.Elements[i]);

            // Change atom type to Ordinary, if required.
            var hasNextAtom = i < this.Elements.Count - 1;
            var nextAtom = hasNextAtom ? (Atom)this.Elements[i + 1] : null;
            curAtom = ChangeAtomToOrdinary(curAtom, previousAtom, nextAtom);

            // Check if atom is part of ligature or should be kerned.
            var kern = 0d;
            if (hasNextAtom && curAtom.GetRightType() == TexAtomType.Ordinary && curAtom.Atom is CharSymbol cs)
            {
                if (nextAtom is CharSymbol ns && ligatureKernChangeSet[(int)nextAtom.GetLeftType()])
                {
                    var font = ns.GetStyledFont(environment);
                    var style = environment.Style;
                    curAtom = curAtom with { IsTextSymbol = true };
                    if (font.SupportsMetrics && cs.IsSupportedByFont(font, style))
                    {
                        var leftAtomCharFont = curAtom.GetCharFont(font).Value;
                        var rightAtomCharFont = ns.GetCharFont(font).Value;
                        var ligatureCharFont = font.GetLigature(leftAtomCharFont, rightAtomCharFont);
                        if (ligatureCharFont == null)
                        {
                            // Atom should be kerned.
                            kern = font.GetKern(leftAtomCharFont, rightAtomCharFont, style);
                        }
                        else
                        {
                            // Atom is part of ligature.
                            curAtom = DummyAtom.CreateLigature(new FixedCharAtom(null, ligatureCharFont));
                            i++;
                        }
                    }
                }
            }

            // Create and add glue box, unless atom is first of row or previous/current atom is kern.
            if (i != 0 && previousAtom != null && !previousAtom.IsKern && !curAtom.IsKern)
                resultBox.Add(Glue.CreateBox(previousAtom.GetRightType(), curAtom.GetLeftType(), environment));

            // Create and add box for atom.
            var curBox = curAtom.WithPreviousAtom(previousAtom).CreateBox(environment);

            resultBox.Add(curBox);
            environment.LastFontId = curBox.GetLastFontId();

            // Insert kern, if required.
            if (kern > TexUtilities.FloatPrecision)
                resultBox.Add(new StrutBox(0, kern, 0, 0));

            if (!curAtom.IsKern)
                previousAtom = curAtom;
        }

        return resultBox;
    }

    public override TexAtomType GetLeftType()
    {
        if (this.Elements.Count == 0)
            return this.Type;
        return this.Elements.First().GetLeftType();
    }

    public override TexAtomType GetRightType()
    {
        if (this.Elements.Count == 0)
            return this.Type;
        return this.Elements.Last().GetRightType();
    }
}
