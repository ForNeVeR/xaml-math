using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class TexFormulaHelper
{
    public TexFormulaParser formulaParser;

    public TexFormulaHelper(TexFormula formula)
    {
        this.formulaParser = new TexFormulaParser();
        this.Formula = formula;
    }

    public TexFormula Formula
    {
        get;
        private set;
    }

    public void SetFixedTypes(TexAtomType leftType, TexAtomType rightType)
    {
        this.Formula.RootAtom = new TypedAtom(this.Formula.RootAtom, leftType, rightType);
    }

    public void CenterOnAxis()
    {
        this.Formula.RootAtom = new VerticalCenteredAtom(this.Formula.RootAtom);
    }

    public void AddAccent(string formula, string accentName)
    {
        AddAccent(formulaParser.Parse(formula), accentName);
    }

    public void AddAccent(TexFormula baseAtom, string accentName)
    {
        Add(new AccentedAtom((baseAtom == null ? null : baseAtom.RootAtom), accentName));
    }

    public void AddAccent(TexFormula baseAtom, TexFormula accent)
    {
        Add(new AccentedAtom((baseAtom == null ? null : baseAtom.RootAtom), accent));
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
        Add(new FencedAtom(formula == null ? null : formula.RootAtom, TexFormulaParser.GetDelimiterSymbol(leftSymbol),
            TexFormulaParser.GetDelimiterSymbol(rightSymbol)));
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
        Add(new FractionAtom(numerator == null ? null : numerator.RootAtom,
            denominator == null ? null : denominator.RootAtom, drawLine));
    }

    public void AddFraction(TexFormula numerator, TexFormula denominator, bool drawLine,
        TexAlignment numeratorAlignment, TexAlignment denominatorAlignment)
    {
        Add(new FractionAtom(numerator == null ? null : numerator.RootAtom, 
            denominator == null ? null : denominator.RootAtom, drawLine, numeratorAlignment, denominatorAlignment));
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
        Add(new Radical(baseFormula == null ? null : baseFormula.RootAtom,
            degreeFormula == null ? null : degreeFormula.RootAtom));
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

    public void AddOperator(TexFormula operatorFormula, TexFormula lowerLimitFormula, TexFormula upperLimitFormula)
    {
        Add(new BigOperatorAtom(operatorFormula == null ? null : operatorFormula.RootAtom,
            lowerLimitFormula == null ? null : lowerLimitFormula.RootAtom,
            upperLimitFormula == null ? null : upperLimitFormula.RootAtom));
    }

    public void AddOperator(TexFormula operatorFormula, TexFormula lowerLimitFormula, TexFormula upperLimitFormula,
        bool useVerticalLimits)
    {
        Add(new BigOperatorAtom(operatorFormula == null ? null : operatorFormula.RootAtom,
            lowerLimitFormula == null ? null : lowerLimitFormula.RootAtom,
            upperLimitFormula == null ? null : upperLimitFormula.RootAtom, useVerticalLimits));
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
        Add(new PhantomAtom(formula == null ? null : formula.RootAtom));
    }

    public void AddPhantom(TexFormula phantom, bool useWidth, bool useHeight, bool useDepth)
    {
        Add(new PhantomAtom(phantom == null ? null : phantom.RootAtom, useWidth, useHeight, useDepth));
    }

    public void AddStrut(TexUnit unit, double width, double height, double depth)
    {
        Add(new SpaceAtom(unit, width, height, depth));
    }

    public void AddStrut(TexUnit widthUnit, double width, TexUnit heightUnit, double height, TexUnit depthUnit,
        double depth)
    {
        Add(new SpaceAtom(widthUnit, width, heightUnit, height, depthUnit, depth));
    }

    public void AddSymbol(string name)
    {
        Add(SymbolAtom.GetAtom(name));
    }

    public void AddSymbol(string name, TexAtomType type)
    {
        Add(new SymbolAtom(SymbolAtom.GetAtom(name), type));
    }

    public void Add(string formula)
    {
        Add(formulaParser.Parse(formula));
    }

    public void Add(TexFormula formula)
    {
        this.Formula.Add(formula);
    }

    public void Add(Atom atom)
    {
        this.Formula.Add(atom);
    }

    public void PutAccentOver(string accentName)
    {
        this.Formula.RootAtom = new AccentedAtom(this.Formula.RootAtom, accentName);
    }

    public void PutDelimiterOver(TexDelimeter delimiter)
    {
        var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Over];
        this.Formula.RootAtom = new OverUnderDelimiter(this.Formula.RootAtom, null, SymbolAtom.GetAtom(name),
            TexUnit.Ex, 0, true);
    }

    public void PutDelimiterOver(TexDelimeter delimiter, string superscriptFormula, TexUnit kernUnit, double kern)
    {
        PutDelimiterOver(delimiter, formulaParser.Parse(superscriptFormula), kernUnit, kern);
    }

    public void PutDelimiterOver(TexDelimeter delimiter, TexFormula superscriptFormula, TexUnit kernUnit, double kern)
    {
        var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Over];
        this.Formula.RootAtom = new OverUnderDelimiter(this.Formula.RootAtom,
            superscriptFormula == null ? null : superscriptFormula.RootAtom, SymbolAtom.GetAtom(name), kernUnit, kern,
            true);
    }

    public void PutDelimiterUnder(TexDelimeter delimiter)
    {
        var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Under];
        this.Formula.RootAtom = new OverUnderDelimiter(this.Formula.RootAtom, null, SymbolAtom.GetAtom(name),
            TexUnit.Ex, 0, false);
    }

    public void PutDelimiterUnder(TexDelimeter delimiter, string subscriptFormula, TexUnit kernUnit, double kern)
    {
        PutDelimiterUnder(delimiter, formulaParser.Parse(subscriptFormula), kernUnit, kern);
    }

    public void PutDelimiterUnder(TexDelimeter delimiter, TexFormula subscriptName, TexUnit kernUnit, double kern)
    {
        var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Under];
        this.Formula.RootAtom = new OverUnderDelimiter(this.Formula.RootAtom,
            subscriptName == null ? null : subscriptName.RootAtom, SymbolAtom.GetAtom(name), kernUnit, kern, false);
    }

    public void PutOver(TexFormula overFormula, TexUnit overUnit, double overSpace, bool overScriptSize)
    {
        this.Formula.RootAtom = new UnderOverAtom(this.Formula.RootAtom,
            overFormula == null ? null : overFormula.RootAtom,overUnit, overSpace, overScriptSize, true);
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
        this.Formula.RootAtom = new UnderOverAtom(this.Formula.RootAtom,
            underFormula == null ? null : underFormula.RootAtom, underUnit, underSpace, underScriptSize, false);
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
        this.Formula.RootAtom = new UnderOverAtom(this.Formula.RootAtom, underFormula == null ?
            null : underFormula.RootAtom, underUnit, underSpace, underScriptSize, over == null ? null : over.RootAtom,
            overUnit, overSpace, overScriptSize);
    }
}
