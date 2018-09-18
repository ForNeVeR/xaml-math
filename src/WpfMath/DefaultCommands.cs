using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using WpfMath.Atoms;
using WpfMath.Exceptions;
using static WpfMath.TexFormulaParser;

namespace WpfMath
{
    public static class DefaultCommands
    {
        public static Atom ColorCommand(this TexFormulaParser formulaParser, TexFormula formula, SourceSpan value, ref int position, bool allowClosingDelimiter, ref bool closedDelimiter)
        {
            var start = position - 5;
            var colorName = formulaParser.ReadGroup(formula, value, ref position, formulaParser.leftGroupChar, formulaParser.rightGroupChar);
            var remainingString = value.Segment(position);
            var remaining = formulaParser.Parse(remainingString, formula.TextStyle);
            position = value.Length;
            if (predefinedColors.TryGetValue(colorName.ToString(), out var color))
            {
                var source = value.Segment(start, position - start);
                return new StyledAtom(source, remaining.RootAtom, null, new SolidColorBrush(color));
            }

            throw new TexParseException($"Color {colorName} not found");
        }

        public static Atom ColorboxCommand(this TexFormulaParser formulaParser, TexFormula formula, SourceSpan value, ref int position, bool allowClosingDelimiter, ref bool closedDelimiter)
        {
            var start = position - 8;
            var colorName = formulaParser.ReadGroup(formula, value, ref position, formulaParser.leftGroupChar, formulaParser.rightGroupChar);
            var remainingString = formulaParser.ReadGroup(formula, value, ref position, formulaParser.leftGroupChar, formulaParser.rightGroupChar);
            var remaining = formulaParser.Parse(remainingString, formula.TextStyle);
            if (predefinedColors.TryGetValue(colorName.ToString(), out var color))
            {
                var source = value.Segment(start, position - start);
                return new StyledAtom(source, remaining.RootAtom, new SolidColorBrush(color), null);
            }

            throw new TexParseException($"Color {colorName} not found");
        }

        public static Atom FracCommand(this TexFormulaParser formulaParser, TexFormula formula, SourceSpan value, ref int position, bool allowClosingDelimiter, ref bool closedDelimiter)
        {
            // Command is fraction.
            var start = position - 4;
            var numeratorFormula = formulaParser.Parse(formulaParser.ReadGroup(formula, value, ref position, formulaParser.leftGroupChar,
                formulaParser.rightGroupChar), formula.TextStyle);
            formulaParser.SkipWhiteSpace(value, ref position);
            var denominatorFormula = formulaParser.Parse(formulaParser.ReadGroup(formula, value, ref position, formulaParser.leftGroupChar,
                formulaParser.rightGroupChar), formula.TextStyle);
            if (numeratorFormula.RootAtom == null || denominatorFormula.RootAtom == null)
                throw new TexParseException("Both numerator and denominator of a fraction can't be empty!");

            var source = value.Segment(start, position - start);
            return new FractionAtom(source, numeratorFormula.RootAtom, denominatorFormula.RootAtom, true);

        }

        public static Atom LeftCommand(this TexFormulaParser formulaParser, TexFormula formula, SourceSpan value, ref int position, bool allowClosingDelimiter, ref bool closedDelimiter)
        {
            var start = position - 4;
            formulaParser.SkipWhiteSpace(value, ref position);
            if (position == value.Length)
                throw new TexParseException("`left` command should be passed a delimiter");

            var delimiter = value[position];
            ++position;
            var left = position;

            var internals = formulaParser.ParseUntilDelimiter(value, ref position, formula.TextStyle);

            var opening = GetDelimiterSymbol(
                GetDelimeterMapping(delimiter),
                value.Segment(start, left - start));
            if (opening == null)
                throw new TexParseException($"Cannot find delimiter named {delimiter}");

            var closing = internals.ClosingDelimiter;
            var source = value.Segment(start, position - start);
            return new FencedAtom(source, internals.Body, opening, closing);
        }

        public static Atom OverlineCommand(this TexFormulaParser formulaParser, TexFormula formula, SourceSpan value, ref int position, bool allowClosingDelimiter, ref bool closedDelimiter)
        {
            var start = position - 8;
            var overlineFormula = formulaParser.Parse(formulaParser.ReadGroup(formula, value, ref position, formulaParser.leftGroupChar, formulaParser.rightGroupChar), formula.TextStyle);
            formulaParser.SkipWhiteSpace(value, ref position);
            var source = value.Segment(start, position - start);
            return new OverlinedAtom(source, overlineFormula.RootAtom);
        }

        public static Atom RightCommand(this TexFormulaParser formulaParser, TexFormula formula, SourceSpan value, ref int position, bool allowClosingDelimiter, ref bool closedDelimiter)
        {
            if (!allowClosingDelimiter)
                throw new TexParseException("`right` command is not allowed without `left`");
            var start = position - 5;
            formulaParser.SkipWhiteSpace(value, ref position);
            if (position == value.Length)
                throw new TexParseException("`right` command should be passed a delimiter");

            var delimiter = value[position];
            ++position;

            var closing = GetDelimiterSymbol(
                GetDelimeterMapping(delimiter),
                value.Segment(start, position - start));
            if (closing == null)
                throw new TexParseException($"Cannot find delimiter named {delimiter}");

            closedDelimiter = true;
            return closing;
        }

        public static Atom SqrtCommand(this TexFormulaParser formulaParser, TexFormula formula, SourceSpan value, ref int position, bool allowClosingDelimiter, ref bool closedDelimiter)
        {
            // Command is radical.
            var start = position - 4;
            formulaParser.SkipWhiteSpace(value, ref position);
            if (position == value.Length)
                throw new TexParseException("illegal end!");

            int sqrtEnd = position;

            TexFormula degreeFormula = null;
            if (value[position] == formulaParser.leftBracketChar)
            {
                // Degree of radical- is specified.
                degreeFormula = formulaParser.Parse(formulaParser.ReadGroup(formula, value, ref position, formulaParser.leftBracketChar,
                    formulaParser.rightBracketChar), formula.TextStyle);
                formulaParser.SkipWhiteSpace(value, ref position);
            }

            var sqrtFormula = formulaParser.Parse(
                formulaParser.ReadGroup(formula, value, ref position, formulaParser.leftGroupChar, formulaParser.rightGroupChar),
                formula.TextStyle);

            if (sqrtFormula.RootAtom == null)
            {
                throw new TexParseException("The radicand of a square root can't be empty!");
            }

            var source = value.Segment(start, sqrtEnd - start);
            return new Radical(source, sqrtFormula.RootAtom, degreeFormula?.RootAtom);
        }

        public static Atom UnderlineCommand(this TexFormulaParser formulaParser, TexFormula formula, SourceSpan value, ref int position, bool allowClosingDelimiter, ref bool closedDelimiter)
        {
            var start = position - 9;
            var underlineFormula = formulaParser.Parse(formulaParser.ReadGroup(formula, value, ref position, formulaParser.leftGroupChar, formulaParser.rightGroupChar), formula.TextStyle);
            formulaParser.SkipWhiteSpace(value, ref position);
            var source = value.Segment(start, position - start);
            return new UnderlinedAtom(source, underlineFormula.RootAtom);
        }

    }
}
