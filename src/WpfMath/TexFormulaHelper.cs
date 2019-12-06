using WpfMath.Atoms;

namespace WpfMath
{
    internal class TexFormulaHelper
    {
        private readonly TexFormulaParser formulaParser;
        private readonly SourceSpan source;

        public TexFormulaHelper(TexFormula formula, SourceSpan source)
        {
            this.formulaParser = new TexFormulaParser();
            this.Formula = formula;
            this.source = source;
        }

        public TexFormula Formula { get; }

        // TODO[F]: Review the cases where the formula gets constructed from this.Formula.RootAtom wrapped in something
        // (e.g. SetFixedTypes): in these cases, it looks like we should clean the SourceSpan inside of a formula and
        // set it for the new root atom only?
        public void SetFixedTypes(TexAtomType leftType, TexAtomType rightType)
        {
            this.Formula.RootAtom = new TypedAtom(this.source, this.Formula.RootAtom, leftType, rightType);
        }

        public void CenterOnAxis()
        {
            this.Formula.RootAtom = new VerticalCenteredAtom(this.source, this.Formula.RootAtom);
        }

        public void AddAccent(string formula, string accentName)
        {
            AddAccent(formulaParser.Parse(formula), accentName);
        }

        public void AddAccent(TexFormula baseAtom, string accentName)
        {
            this.Add(new AccentedAtom(null, baseAtom?.RootAtom, accentName));
        }

        public void AddAccent(TexFormula baseAtom, TexFormula accent)
        {
            this.Add(new AccentedAtom(null, baseAtom?.RootAtom, accent));
        }

        public void AddEmbraced(string formula, char leftChar, char rightChar)
        {
            AddEmbraced(formulaParser.Parse(formula), leftChar, rightChar);
        }

        public void AddEmbraced(TexFormula formula, char leftChar, char rightChar)
        {
            AddEmbraced(formula, TexFormulaParser.GetDelimeterMapping(leftChar),
                TexFormulaParser.GetDelimeterMapping(rightChar));
        }

        public void AddEmbraced(string formula, string leftSymbol, string rightSymbol)
        {
            AddEmbraced(formulaParser.Parse(formula), leftSymbol, rightSymbol);
        }

        public void AddEmbraced(TexFormula formula, string leftSymbol, string rightSymbol)
        {
            this.Add(
                new FencedAtom(
                    null,
                    formula?.RootAtom,
                    TexFormulaParser.GetDelimiterSymbol(leftSymbol, null),
                    TexFormulaParser.GetDelimiterSymbol(rightSymbol, null)));
        }

        public void AddFraction(string numerator, string denominator, bool drawLine)
        {
            AddFraction(formulaParser.Parse(numerator), formulaParser.Parse(denominator), drawLine);
        }

        public void AddFraction(string numerator, TexFormula denominator, bool drawLine)
        {
            AddFraction(formulaParser.Parse(numerator), denominator, drawLine);
        }

        public void AddFraction(string numerator, string denominator, bool drawLine, TexAlignment numeratorAlignment,
            TexAlignment denominatorAlignment)
        {
            AddFraction(formulaParser.Parse(numerator), formulaParser.Parse(denominator), drawLine, numeratorAlignment,
                denominatorAlignment);
        }

        public void AddFraction(TexFormula numerator, string denominator, bool drawLine)
        {
            AddFraction(numerator, formulaParser.Parse(denominator), drawLine);
        }

        public void AddFraction(TexFormula numerator, TexFormula denominator, bool drawLine)
        {
            this.Add(new FractionAtom(null, numerator?.RootAtom, denominator?.RootAtom, drawLine));
        }

        public void AddFraction(TexFormula numerator, TexFormula denominator, bool drawLine,
            TexAlignment numeratorAlignment, TexAlignment denominatorAlignment)
        {
            this.Add(
                new FractionAtom(
                    null,
                    numerator?.RootAtom,
                    denominator?.RootAtom,
                    drawLine,
                    numeratorAlignment,
                    denominatorAlignment));
        }

        public void AddRadical(string baseFormula, string nthRoot)
        {
            AddRadical(formulaParser.Parse(baseFormula), formulaParser.Parse(nthRoot));
        }

        public void AddRadical(string baseFormula, TexFormula nthRoot)
        {
            AddRadical(formulaParser.Parse(baseFormula), nthRoot);
        }

        public void AddRadical(string baseFormula)
        {
            AddRadical(formulaParser.Parse(baseFormula));
        }

        public void AddRadical(TexFormula baseFormula, string degreeFormula)
        {
            AddRadical(baseFormula, formulaParser.Parse(degreeFormula));
        }

        public void AddRadical(TexFormula baseFormula)
        {
            AddRadical(baseFormula, (TexFormula)null);
        }

        public void AddRadical(TexFormula baseFormula, TexFormula degreeFormula)
        {
            this.Add(new Radical(null, baseFormula?.RootAtom, degreeFormula?.RootAtom));
        }

        public void AddOperator(string operatorFormula, string lowerLimitFormula, string upperLimitFormula)
        {
            AddOperator(formulaParser.Parse(operatorFormula), formulaParser.Parse(lowerLimitFormula),
                formulaParser.Parse(upperLimitFormula));
        }

        public void AddOperator(string operatorFormula, string lowerLimitFormula, string upperLimitFormula,
            bool useVerticalLimits)
        {
            AddOperator(formulaParser.Parse(operatorFormula), formulaParser.Parse(lowerLimitFormula),
                formulaParser.Parse(upperLimitFormula), useVerticalLimits);
        }

        public int MoveOffset(Atom atom, int pos)
        {
            atom.Source = new SourceSpan(atom.Source.Source, pos, atom.Source.Length);
            int currentPos = pos;
            foreach (var child in atom.Children)
            {
                currentPos = MoveOffset(child, currentPos);
            }
            return pos + atom.Source.Length;
        }

        public void AddOperator(string operatorFormula, bool useVerticalLimits)
        {
            var formula = formulaParser.Parse(operatorFormula);
            formula.RootAtom.Source = new SourceSpan(source.Source, source.Start, formula.RootAtom.Source.Length);
            var pos = source.Start+1;
            foreach(var e in formula.RootAtom.Children)
            {
                pos = MoveOffset(e, pos);
            }
            AddOperator(formula, null, null, useVerticalLimits);
        }

        public void AddOperator(TexFormula operatorFormula, TexFormula lowerLimitFormula, TexFormula upperLimitFormula)
        {
            this.Add(
                new BigOperatorAtom(
                    operatorFormula?.RootAtom?.Source,
                    operatorFormula?.RootAtom,
                    lowerLimitFormula?.RootAtom,
                    upperLimitFormula?.RootAtom));
        }

        public void AddOperator(
            TexFormula operatorFormula,
            TexFormula lowerLimitFormula,
            TexFormula upperLimitFormula,
            bool useVerticalLimits)
        {
                this.Add(
                new BigOperatorAtom(
                    operatorFormula?.RootAtom?.Source,
                    operatorFormula?.RootAtom,
                    lowerLimitFormula?.RootAtom,
                    upperLimitFormula?.RootAtom,
                    useVerticalLimits));
        }

        public void AddPhantom(string formula)
        {
            AddPhantom(formulaParser.Parse(formula));
        }

        public void AddPhantom(string formula, bool useWidth, bool useHeight, bool useDepth)
        {
            AddPhantom(formulaParser.Parse(formula), useWidth, useHeight, useDepth);
        }

        public void AddPhantom(TexFormula formula)
        {
            this.Add(new PhantomAtom(null, formula?.RootAtom));
        }

        public void AddPhantom(TexFormula phantom, bool useWidth, bool useHeight, bool useDepth)
        {
            this.Add(new PhantomAtom(null, phantom?.RootAtom, useWidth, useHeight, useDepth));
        }

        public void AddStrut(TexUnit unit, double width, double height, double depth)
        {
            this.Add(new SpaceAtom(null, unit, width, height, depth));
        }

        public void AddStrut(
            TexUnit widthUnit,
            double width,
            TexUnit heightUnit,
            double height,
            TexUnit depthUnit,
            double depth)
        {
            this.Add(new SpaceAtom(null, widthUnit, width, heightUnit, height, depthUnit, depth));
        }

        public void AddSymbol(string name)
        {
            Add(SymbolAtom.GetAtom(name, null));
        }

        public void AddSymbol(string name, TexAtomType type)
        {
            this.Add(new SymbolAtom(null, SymbolAtom.GetAtom(name, null), type));
        }

        public void Add(string formula)
        {
            Add(formulaParser.Parse(formula));
        }

        public void Add(TexFormula formula)
        {
            this.Formula.Add(formula, this.source);
        }

        public void Add(Atom atom)
        {
            this.Formula.Add(atom, this.source);
        }

        public void PutAccentOver(string accentName)
        {
            this.Formula.RootAtom = new AccentedAtom(this.source, this.Formula.RootAtom, accentName);
        }

        public void PutDelimiterOver(TexDelimiter delimiter)
        {
            var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Over];
            this.Formula.RootAtom = new OverUnderDelimiter(
                this.source,
                this.Formula.RootAtom,
                null,
                SymbolAtom.GetAtom(name, null),
                TexUnit.Ex,
                0.0,
                true);
        }

        public void PutDelimiterOver(TexDelimiter delimiter, string superscriptFormula, TexUnit kernUnit, double kern)
        {
            this.PutDelimiterOver(delimiter, this.formulaParser.Parse(superscriptFormula), kernUnit, kern);
        }

        public void PutDelimiterOver(
            TexDelimiter delimiter,
            TexFormula superscriptFormula,
            TexUnit kernUnit,
            double kern)
        {
            var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Over];
            this.Formula.RootAtom = new OverUnderDelimiter(
                this.source,
                this.Formula.RootAtom,
                superscriptFormula?.RootAtom,
                SymbolAtom.GetAtom(name, null),
                kernUnit,
                kern,
                true);
        }

        public void PutDelimiterUnder(TexDelimiter delimiter)
        {
            var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Under];
            this.Formula.RootAtom = new OverUnderDelimiter(
                this.source,
                this.Formula.RootAtom,
                null,
                SymbolAtom.GetAtom(name, null),
                TexUnit.Ex,
                0.0,
                false);
        }

        public void PutDelimiterUnder(TexDelimiter delimiter, string subscriptFormula, TexUnit kernUnit, double kern)
        {
            this.PutDelimiterUnder(delimiter, this.formulaParser.Parse(subscriptFormula), kernUnit, kern);
        }

        public void PutDelimiterUnder(TexDelimiter delimiter, TexFormula subscriptName, TexUnit kernUnit, double kern)
        {
            var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Under];
            this.Formula.RootAtom = new OverUnderDelimiter(
                this.source,
                this.Formula.RootAtom,
                subscriptName?.RootAtom,
                SymbolAtom.GetAtom(name, null),
                kernUnit,
                kern,
                false);
        }

        public void PutOver(TexFormula overFormula, TexUnit overUnit, double overSpace, bool overScriptSize)
        {
            this.Formula.RootAtom = new UnderOverAtom(
                this.source,
                this.Formula.RootAtom,
                overFormula?.RootAtom,
                overUnit,
                overSpace,
                overScriptSize,
                true);
        }

        public void PutOver(string overFormula, TexUnit overUnit, double overSpace, bool overScriptSize)
        {
            PutOver(overFormula == null ? null : formulaParser.Parse(overFormula), overUnit, overSpace, overScriptSize);
        }

        public void PutUnder(string underFormula, TexUnit underUnit, double underSpace, bool underScriptSize)
        {
            PutUnder(underFormula == null ? null : formulaParser.Parse(underFormula), underUnit, underSpace,
                underScriptSize);
        }

        public void PutUnder(TexFormula underFormula, TexUnit underUnit, double underSpace, bool underScriptSize)
        {
            this.Formula.RootAtom = new UnderOverAtom(
                this.source,
                this.Formula.RootAtom,
                underFormula?.RootAtom,
                underUnit,
                underSpace,
                underScriptSize,
                false);
        }

        public void PutUnderAndOver(string underFormula, TexUnit underUnit, double underSpace, bool underScriptSize,
            string over, TexUnit overUnit, double overSpace, bool overScriptSize)
        {
            PutUnderAndOver(underFormula == null ? null : formulaParser.Parse(underFormula), underUnit, underSpace,
                underScriptSize, over == null ? null : formulaParser.Parse(over), overUnit, overSpace, overScriptSize);
        }

        public void PutUnderAndOver(TexFormula underFormula, TexUnit underUnit, double underSpace, bool underScriptSize,
            TexFormula over, TexUnit overUnit, double overSpace, bool overScriptSize)
        {
            this.Formula.RootAtom = new UnderOverAtom(
                this.source,
                this.Formula.RootAtom,
                underFormula?.RootAtom,
                underUnit,
                underSpace,
                underScriptSize,
                over?.RootAtom,
                overUnit,
                overSpace,
                overScriptSize);
        }
    }
}
