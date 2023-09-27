using System;
using System.Collections.Generic;
using System.Diagnostics;
using XamlMath.Atoms;
using XamlMath.Rendering;

namespace XamlMath;

internal sealed class TexFormulaHelper
{
    private readonly TexFormulaParser _formulaParser;
    private readonly SourceSpan _source;

    public TexFormulaHelper(
        TexFormula formula,
        SourceSpan source,
        IBrushFactory brushFactory,
        IReadOnlyDictionary<string, Func<SourceSpan, TexFormula?>> predefinedFormulae)
    {
        this._formulaParser = new TexFormulaParser(brushFactory, predefinedFormulae);
        this.Formula = formula;
        this._source = source;
    }

    public TexFormula Formula { get; }

    private TexFormula ParseFormula(string source) =>
        _formulaParser.Parse(new SourceSpan("Predefined formula fragment", source, 0, source.Length));

    public void SetFixedTypes(TexAtomType leftType, TexAtomType rightType)
    {
        this.Formula.RootAtom = new TypedAtom(this._source, this.Formula.RootAtom, leftType, rightType);
    }

    public void CenterOnAxis()
    {
        this.Formula.RootAtom = new VerticalCenteredAtom(this._source, this.Formula.RootAtom);
    }

    public void AddAccent(string formula, string accentName)
    {
        AddAccent(ParseFormula(formula), accentName);
    }

    public void AddAccent(TexFormula baseAtom, string accentName)
    {
        this.Add(new AccentedAtom(_source, baseAtom?.RootAtom, accentName));
    }

    public void AddAccent(TexFormula baseAtom, TexFormula accent)
    {
        this.Add(new AccentedAtom(null, baseAtom?.RootAtom, accent));
    }

    public void AddEmbraced(string formula, char leftChar, char rightChar)
    {
        AddEmbraced(ParseFormula(formula), leftChar, rightChar);
    }

    public void AddEmbraced(TexFormula formula, char leftChar, char rightChar)
    {
        AddEmbraced(formula, TexFormulaParser.GetDelimeterMapping(leftChar),
            TexFormulaParser.GetDelimeterMapping(rightChar));
    }

    public void AddEmbraced(string formula, string leftSymbol, string rightSymbol)
    {
        AddEmbraced(ParseFormula(formula), leftSymbol, rightSymbol);
    }

    public void AddEmbraced(TexFormula formula, string leftSymbol, string rightSymbol)
    {
        this.Add(
            new FencedAtom(
                _source,
                formula?.RootAtom,
                TexFormulaParser.GetDelimiterSymbol(leftSymbol, null),
                TexFormulaParser.GetDelimiterSymbol(rightSymbol, null)));
    }

    public void AddFraction(string numerator, string denominator, bool drawLine)
    {
        AddFraction(ParseFormula(numerator), ParseFormula(denominator), drawLine);
    }

    public void AddFraction(string numerator, TexFormula denominator, bool drawLine)
    {
        AddFraction(ParseFormula(numerator), denominator, drawLine);
    }

    public void AddFraction(string numerator, string denominator, bool drawLine, TexAlignment numeratorAlignment,
        TexAlignment denominatorAlignment)
    {
        AddFraction(ParseFormula(numerator), ParseFormula(denominator), drawLine, numeratorAlignment,
            denominatorAlignment);
    }

    public void AddFraction(TexFormula numerator, string denominator, bool drawLine)
    {
        AddFraction(numerator, ParseFormula(denominator), drawLine);
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
        AddRadical(ParseFormula(baseFormula), ParseFormula(nthRoot));
    }

    public void AddRadical(string baseFormula, TexFormula nthRoot)
    {
        AddRadical(ParseFormula(baseFormula), nthRoot);
    }

    public void AddRadical(string baseFormula)
    {
        AddRadical(ParseFormula(baseFormula));
    }

    public void AddRadical(TexFormula baseFormula, string degreeFormula)
    {
        AddRadical(baseFormula, ParseFormula(degreeFormula));
    }

    public void AddRadical(TexFormula baseFormula)
    {
        AddRadical(baseFormula, (TexFormula?)null);
    }

    public void AddRadical(TexFormula baseFormula, TexFormula? degreeFormula)
    {
        Debug.Assert(baseFormula.RootAtom != null);
        this.Add(new Radical(null, baseFormula.RootAtom, degreeFormula?.RootAtom));
    }

    public void AddOperator(string operatorFormula, string lowerLimitFormula, string upperLimitFormula)
    {
        AddOperator(ParseFormula(operatorFormula), ParseFormula(lowerLimitFormula),
            ParseFormula(upperLimitFormula));
    }

    public void AddOperator(string operatorFormula, string lowerLimitFormula, string upperLimitFormula,
        bool useVerticalLimits)
    {
        AddOperator(ParseFormula(operatorFormula), ParseFormula(lowerLimitFormula),
            ParseFormula(upperLimitFormula), useVerticalLimits);
    }

    public void AddOperator(string operatorFormula, bool useVerticalLimits)
    {
        AddOperator(ParseFormula(operatorFormula), null, null, useVerticalLimits);
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
        TexFormula? lowerLimitFormula,
        TexFormula? upperLimitFormula,
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
        AddPhantom(ParseFormula(formula));
    }

    public void AddPhantom(string formula, bool useWidth, bool useHeight, bool useDepth)
    {
        AddPhantom(ParseFormula(formula), useWidth, useHeight, useDepth);
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
        Add(ParseFormula(formula));
    }

    public void Add(TexFormula formula)
    {
        this.Formula.Add(formula, this._source);
    }

    public void Add(Atom atom)
    {
        this.Formula.Add(atom, this._source);
    }

    public void PutAccentOver(string accentName)
    {
        this.Formula.RootAtom = new AccentedAtom(this._source, this.Formula.RootAtom, accentName);
    }

    public void PutDelimiterOver(TexDelimiter delimiter)
    {
        var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Over];
        this.Formula.RootAtom = new OverUnderDelimiter(
            this._source,
            this.Formula.RootAtom,
            null,
            SymbolAtom.GetAtom(name, null),
            TexUnit.Ex,
            0.0,
            true);
    }

    public void PutDelimiterOver(TexDelimiter delimiter, string superscriptFormula, TexUnit kernUnit, double kern)
    {
        this.PutDelimiterOver(delimiter, this.ParseFormula(superscriptFormula), kernUnit, kern);
    }

    public void PutDelimiterOver(
        TexDelimiter delimiter,
        TexFormula superscriptFormula,
        TexUnit kernUnit,
        double kern)
    {
        var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Over];
        this.Formula.RootAtom = new OverUnderDelimiter(
            this._source,
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
            this._source,
            this.Formula.RootAtom,
            null,
            SymbolAtom.GetAtom(name, null),
            TexUnit.Ex,
            0.0,
            false);
    }

    public void PutDelimiterUnder(TexDelimiter delimiter, string subscriptFormula, TexUnit kernUnit, double kern)
    {
        this.PutDelimiterUnder(delimiter, this.ParseFormula(subscriptFormula), kernUnit, kern);
    }

    public void PutDelimiterUnder(TexDelimiter delimiter, TexFormula subscriptName, TexUnit kernUnit, double kern)
    {
        var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Under];
        this.Formula.RootAtom = new OverUnderDelimiter(
            this._source,
            this.Formula.RootAtom,
            subscriptName?.RootAtom,
            SymbolAtom.GetAtom(name, null),
            kernUnit,
            kern,
            false);
    }

    public void PutOver(TexFormula? overFormula, TexUnit overUnit, double overSpace, bool overScriptSize)
    {
        this.Formula.RootAtom = new UnderOverAtom(
            this._source,
            this.Formula.RootAtom,
            overFormula?.RootAtom,
            overUnit,
            overSpace,
            overScriptSize,
            true);
    }

    public void PutOver(string? overFormula, TexUnit overUnit, double overSpace, bool overScriptSize)
    {
        PutOver(overFormula == null ? null : ParseFormula(overFormula), overUnit, overSpace, overScriptSize);
    }

    public void PutUnder(string? underFormula, TexUnit underUnit, double underSpace, bool underScriptSize)
    {
        PutUnder(underFormula == null ? null : ParseFormula(underFormula), underUnit, underSpace,
            underScriptSize);
    }

    public void PutUnder(TexFormula? underFormula, TexUnit underUnit, double underSpace, bool underScriptSize)
    {
        this.Formula.RootAtom = new UnderOverAtom(
            this._source,
            this.Formula.RootAtom,
            underFormula?.RootAtom,
            underUnit,
            underSpace,
            underScriptSize,
            false);
    }

    public void PutUnderAndOver(string? underFormula, TexUnit underUnit, double underSpace, bool underScriptSize,
        string? over, TexUnit overUnit, double overSpace, bool overScriptSize)
    {
        PutUnderAndOver(underFormula == null ? null : ParseFormula(underFormula), underUnit, underSpace,
            underScriptSize, over == null ? null : ParseFormula(over), overUnit, overSpace, overScriptSize);
    }

    public void PutUnderAndOver(TexFormula? underFormula, TexUnit underUnit, double underSpace, bool underScriptSize,
        TexFormula? over, TexUnit overUnit, double overSpace, bool overScriptSize)
    {
        this.Formula.RootAtom = new UnderOverAtom(
            this._source,
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
