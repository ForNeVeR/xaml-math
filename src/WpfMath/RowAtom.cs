using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WpfMath
{
    // Atom representing horizontal row of other atoms, separated by glue.
    internal class RowAtom : Atom, IRow
    {
        // Set of atom types that make previous atom of BinaryOperator type change to Ordinary type.
        private static BitArray binaryOperatorChangeSet;

        // Set of atom types that may need kern, or together with previous atom, be replaced by ligature.
        private static BitArray ligatureKernChangeSet;

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

        public RowAtom(IList<TexFormula> formulaList)
            : this()
        {
            foreach (var formula in formulaList)
            {
                if (formula.RootAtom != null)
                    this.Elements.Add(formula.RootAtom);
            }
        }

        public RowAtom(Atom baseAtom)
            : this()
        {
            if (baseAtom != null)
            {
                if (baseAtom is RowAtom)
                {
                    foreach (var atom in ((RowAtom)baseAtom).Elements)
                        this.Elements.Add(atom);
                }
                else
                {
                    this.Elements.Add(baseAtom);
                }
            }
        }

        public RowAtom()
            : base()
        {
            this.Elements = new List<Atom>();
        }

        public DummyAtom PreviousAtom
        {
            get;
            set;
        }

        public List<Atom> Elements
        {
            get;
            private set;
        }

        public void Add(Atom atom)
        {
            if (atom != null)
                this.Elements.Add(atom);
        }

        private void ChangeAtomToOrdinary(DummyAtom currentAtom, DummyAtom previousAtom, Atom nextAtom)
        {
            var type = currentAtom.GetLeftType();
            if (type == TexAtomType.BinaryOperator && (previousAtom == null ||
                binaryOperatorChangeSet[(int)previousAtom.GetRightType()]))
            {
                currentAtom.Type = TexAtomType.Ordinary;
            }
            else if (nextAtom != null && currentAtom.GetRightType() == TexAtomType.BinaryOperator)
            {
                var nextType = nextAtom.GetLeftType();
                if (nextType == TexAtomType.Relation || nextType == TexAtomType.Closing || nextType == TexAtomType.Punctuation)
                    currentAtom.Type = TexAtomType.Ordinary;
            }
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            var texFont = environment.TexFont;

            // Create result box.
            var resultBox = new HorizontalBox(environment.Foreground, environment.Background);

            // Create and add box for each atom in row.
            for (int i = 0; i < this.Elements.Count; i++)
            {
                var curAtom = new DummyAtom(this.Elements[i]);

                // Change atom type to Ordinary, if required.
                var hasNextAtom = i < this.Elements.Count - 1;
                var nextAtom = hasNextAtom ? (Atom)this.Elements[i + 1] : null;
                ChangeAtomToOrdinary(curAtom, this.PreviousAtom, nextAtom);

                // Check if atom is part of ligature or should be kerned.
                var kern = 0d;
                if (hasNextAtom && curAtom.GetRightType() == TexAtomType.Ordinary && curAtom.IsCharSymbol)
                {
                    if (nextAtom is CharSymbol cs && ligatureKernChangeSet[(int)nextAtom.GetLeftType()])
                    {
                        var font = cs.OverrideFont(texFont);
                        curAtom.IsTextSymbol = true;
                        var leftAtomCharFont = curAtom.GetCharFont(font);
                        var rightAtomCharFont = cs.GetCharFont(font);
                        var ligatureCharFont = font.GetLigature(leftAtomCharFont, rightAtomCharFont);
                        if (ligatureCharFont == null)
                        {
                            // Atom should be kerned.
                            kern = font.GetKern(leftAtomCharFont, rightAtomCharFont, environment.Style);
                        }
                        else
                        {
                            // Atom is part of ligature.
                            curAtom.SetLigature(new FixedCharAtom(ligatureCharFont));
                            i++;
                        }
                    }
                }

                // Create and add glue box, unless atom is first of row or previous/current atom is kern.
                if (i != 0 && this.PreviousAtom != null && !this.PreviousAtom.IsKern && !curAtom.IsKern)
                    resultBox.Add(Glue.CreateBox(this.PreviousAtom.GetRightType(), curAtom.GetLeftType(), environment));

                // Create and add box for atom.
                curAtom.PreviousAtom = this.PreviousAtom;
                var curBox = curAtom.CreateBox(environment);
                resultBox.Add(curBox);
                environment.LastFontId = curBox.GetLastFontId();

                // Insert kern, if required.
                if (kern > TexUtilities.FloatPrecision)
                    resultBox.Add(new StrutBox(0, kern, 0, 0));

                if (!curAtom.IsKern)
                    this.PreviousAtom = curAtom;
            }

            // Reset previous atom.
            this.PreviousAtom = null;

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
}
