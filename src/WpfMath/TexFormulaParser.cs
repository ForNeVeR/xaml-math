using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Text;
using WpfMath.Atoms;
using WpfMath.Exceptions;

namespace WpfMath
{
    // TODO: Put all error strings into resources.
    // TODO: Use TextReader for lexing.
    public class TexFormulaParser
    {
        // Special characters for parsing
        private const char escapeChar = '\\';

        private const char leftGroupChar = '{';
        private const char rightGroupChar = '}';
        private const char leftBracketChar = '[';
        private const char rightBracketChar = ']';

        private const char subScriptChar = '_';
        private const char superScriptChar = '^';
        private const char primeChar = '\'';

        // Information used for parsing
        private static HashSet<string> commands;
        private static IList<string> symbols;
        private static IList<string> delimeters;
        private static HashSet<string> textStyles;
        private static readonly IDictionary<string, Func<SourceSpan, TexFormula>> predefinedFormulas =
            new Dictionary<string, Func<SourceSpan, TexFormula>>();
        private static IDictionary<string, Color> predefinedColors;

        private static readonly string[][] delimiterNames =
        {
            new[] { "lbrace", "rbrace" },
            new[] { "lsqbrack", "rsqbrack" },
            new[] { "lbrack", "rbrack" },
            new[] { "downarrow", "downarrow" },
            new[] { "uparrow", "uparrow" },
            new[] { "updownarrow", "updownarrow" },
            new[] { "Downarrow", "Downarrow" },
            new[] { "Uparrow", "Uparrow" },
            new[] { "Updownarrow", "Updownarrow" },
            new[] { "vert", "vert" },
            new[] { "Vert", "Vert" }
        };

        static TexFormulaParser()
        {
            predefinedColors = new Dictionary<string, Color>();

            Initialize();
        }

        internal static string[][] DelimiterNames
        {
            get { return delimiterNames; }
        }

        private static void Initialize()
        {
            //
            // If start application isn't WPF, pack isn't registered by defaultTexFontParser
            //
            if (Application.ResourceAssembly == null)
            {
                Application.ResourceAssembly = Assembly.GetExecutingAssembly();
                if (!UriParser.IsKnownScheme("pack"))
                    UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), "pack", -1);
            }

            commands = new HashSet<string>
            {
                "\\",
                "amatrix",
                "bmatrix",
                "Bmatrix",
                "cases",
                "color",
                "colorbox",
                "cr",
                "frac",
                "left",
                "matrix",
                "overline",
                "pmatrix",
                "right",
                "sqrt",
                "underline",
                "vmatrix",
                "Vmatrix",
            };

            var formulaSettingsParser = new TexPredefinedFormulaSettingsParser();
            symbols = formulaSettingsParser.GetSymbolMappings();
            delimeters = formulaSettingsParser.GetDelimiterMappings();
            textStyles = formulaSettingsParser.GetTextStyles();

            var colorParser = new PredefinedColorParser();
            colorParser.Parse(predefinedColors);

            var predefinedFormulasParser = new TexPredefinedFormulaParser();
            predefinedFormulasParser.Parse(predefinedFormulas);
        }

        internal static string GetDelimeterMapping(char character)
        {
            try
            {
                return delimeters[character];
            }
            catch (KeyNotFoundException)
            {
                throw new DelimiterMappingNotFoundException(character);
            }
        }

        internal static SymbolAtom GetDelimiterSymbol(string name, SourceSpan source)
        {
            if (name == null)
                return null;

            var result = SymbolAtom.GetAtom(name, source);
            if (!result.IsDelimeter)
                return null;
            return result;
        }

        private static bool IsSymbol(char c)
        {
            return !char.IsLetterOrDigit(c);
        }

        private static bool IsWhiteSpace(char ch)
        {
            return ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
        }

        private static bool ShouldSkipWhiteSpace(string style) => style != TexUtilities.TextStyleName;

        public TexFormula Parse(string value, string textStyle = null)
        {
            Debug.WriteLine(value);
            var position = 0;
            return Parse(new SourceSpan(value, 0, value.Length), ref position, false, textStyle);
        }

        private TexFormula Parse(SourceSpan value, string textStyle)
        {
            int localPostion = 0;
            return Parse(value, ref localPostion, false, textStyle);
        }

        private DelimiterInfo ParseUntilDelimiter(SourceSpan value, ref int position, string textStyle)
        {
            var embeddedFormula = Parse(value, ref position, true, textStyle);
            if (embeddedFormula.RootAtom == null)
                throw new TexParseException("Cannot find closing delimiter");

            var source = embeddedFormula.RootAtom.Source;
            var bodyRow = embeddedFormula.RootAtom as RowAtom;
            var lastAtom = bodyRow?.Elements.LastOrDefault() ?? embeddedFormula.RootAtom;
            var lastDelimiter = lastAtom as SymbolAtom;
            if (lastDelimiter == null || !lastDelimiter.IsDelimeter)
                throw new TexParseException($"Cannot find closing delimiter; got {lastDelimiter} instead");

            Atom bodyAtom;
            if (bodyRow == null)
            {
                bodyAtom = new RowAtom(source);
            }
            else if (bodyRow.Elements.Count > 2)
            {
                var row = bodyRow.Elements.Take(bodyRow.Elements.Count - 1)
                    .Aggregate(new RowAtom(source), (r, atom) => r.Add(atom));
                bodyAtom = row;
            }
            else if (bodyRow.Elements.Count == 2)
            {
                bodyAtom = bodyRow.Elements[0];
            }
            else
            {
                throw new NotSupportedException($"Cannot convert {bodyRow} to fenced atom body");
            }

            return new DelimiterInfo(bodyAtom, lastDelimiter);
        }

        private TexFormula Parse(SourceSpan value, ref int position, bool allowClosingDelimiter, string textStyle)
        {
            var formula = new TexFormula() { TextStyle = textStyle };
            var closedDelimiter = false;
            var skipWhiteSpace = ShouldSkipWhiteSpace(textStyle);
            var initialPosition = position;
            while (position < value.Length && !(allowClosingDelimiter && closedDelimiter))
            {
                char ch = value[position];
                var source = value.Segment(position, 1);
                if (IsWhiteSpace(ch))
                {
                    if (!skipWhiteSpace)
                    {
                        formula.Add(new SpaceAtom(source), source);
                    }

                    position++;
                }
                else if (ch == escapeChar)
                {
                    ProcessEscapeSequence(formula, value, ref position, allowClosingDelimiter, ref closedDelimiter);
                }
                else if (ch == leftGroupChar)
                {
                    var groupValue = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                    var parsedGroup = Parse(groupValue, textStyle);
                    var innerGroupAtom = parsedGroup.RootAtom ?? new RowAtom(groupValue);
                    var groupAtom = new TypedAtom(
                        innerGroupAtom.Source,
                        innerGroupAtom,
                        TexAtomType.Ordinary,
                        TexAtomType.Ordinary);
                    var scriptsAtom = this.AttachScripts(formula, value, ref position, groupAtom);
                    formula.Add(scriptsAtom, value.Segment(initialPosition, scriptsAtom.Source.Length));
                }
                else if (ch == rightGroupChar)
                {
                    throw new TexParseException("Found a closing '" + rightGroupChar
                        + "' without an opening '" + leftGroupChar + "'!");
                }
                else if (ch == superScriptChar || ch == subScriptChar || ch == primeChar)
                {
                    if (position == 0)
                        throw new TexParseException("Every script needs a base: \""
                            + superScriptChar + "\", \"" + subScriptChar + "\" and \""
                            + primeChar + "\" can't be the first character!");
                    else
                        throw new TexParseException("Double scripts found! Try using more braces.");
                }
                else
                {
                    var scriptsAtom = this.AttachScripts(
                        formula,
                        value,
                        ref position,
                        this.ConvertCharacter(formula, ref position, source),
                        skipWhiteSpace);
                    formula.Add(scriptsAtom, value.Segment(initialPosition, position));
                }
            }

            return formula;
        }

        private SourceSpan ReadGroup(TexFormula formula, SourceSpan value, ref int position, char openChar, char closeChar)
        {
            if (position == value.Length || value[position] != openChar)
                throw new TexParseException("missing '" + openChar + "'!");

            var group = 0;
            position++;
            var start = position;
            while (position < value.Length && !(value[position] == closeChar && group == 0))
            {
                if (value[position] == openChar)
                    group++;
                else if (value[position] == closeChar)
                    group--;
                position++;
            }

            if (position == value.Length)
            {
                // Reached end of formula but group has not been closed.
                throw new TexParseException("Illegal end,  missing '" + closeChar + "'!");
            }

            position++;
            return value.Segment(start, position - start - 1);
        }
        
        private string ReadGroup(string str, char leftchar, char rightchar, int startPosition)
        {
            StringBuilder sb = new StringBuilder();
            if (startPosition == str.Length)
            {
                throw new TexParseException("illegal end!");
            }
            int deepness = 0; bool groupfound = false;
            var start = startPosition;
            if (str[start] == leftchar)
            {
                start++;
                while (start < str.Length && groupfound == false)
                {
                    if (str[start] == leftchar)
                    {
                        deepness++;
                        sb.Append(leftchar);
                    }
                    else if (str[start] == rightchar)
                    {
                        if (deepness == 0)
                        {
                            groupfound = true;
                        }
                        else
                        {
                            deepness--;
                            sb.Append(rightchar);
                        }
                    }
                    else
                    {
                        sb.Append(str[start]);
                    }
                    start++;
                }
            }

            if (groupfound)
            {
                return sb.ToString();
            }
            else
            {
                throw new TexParseException("missing->>" + rightchar);
            }
        }

        private TexFormula ReadScript(TexFormula formula, SourceSpan value, ref int position)
        {
            SkipWhiteSpace(value, ref position);
            if (position == value.Length)
                throw new TexParseException("illegal end, missing script!");

            var ch = value[position];
            if (ch == leftGroupChar)
            {
                return Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), formula.TextStyle);
            }
            else
            {
                position++;
                return Parse(value.Segment(position - 1, 1), formula.TextStyle);
            }
        }

        private Atom ProcessCommand(
            TexFormula formula,
            SourceSpan value,
            ref int position,
            string command,
            bool allowClosingDelimiter,
            ref bool closedDelimiter)
        {
            int start = position - command.Length;

            SkipWhiteSpace(value, ref position);

            SourceSpan source;
            switch (command)
            {
                case "\\":
                case "cr":
                    {
                        return new NullAtom(new SourceSpan("cr", start, 2));
                    }

                case "amatrix":
                    {
                        if (position == value.Length)
                            throw new TexParseException("illegal end!");
                        SkipWhiteSpace(value, ref position);

                        var leftmatrixsource = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                        var rightmatrixsource = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                        var leftcells = GetMatrixData(formula, leftmatrixsource);
                        var rightcells = GetMatrixData(formula, rightmatrixsource);
                        source = value.Segment(start, position - start);
                        if (leftcells.Count == rightcells.Count)
                        {
                            return new AugmentedMatrixAtom(source, new MatrixAtom(leftmatrixsource, leftcells), new MatrixAtom(rightmatrixsource, rightcells));
                        }
                        else
                        {
                            throw new TexParseException("an augmented matrix cannot have unequal rows");
                        }
                    }
                    
                case "bmatrix":
                    {
                        if (position == value.Length)
                            throw new TexParseException("illegal end!");
                        SkipWhiteSpace(value, ref position);

                        var matrixsource = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);

                        var cells = GetMatrixData(formula, matrixsource);
                        return new BmatrixAtom(matrixsource, new MatrixAtom(matrixsource, cells));
                    }

                case "Bmatrix":
                    {
                        if (position == value.Length)
                            throw new TexParseException("illegal end!");
                        SkipWhiteSpace(value, ref position);

                        var matrixsource = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);

                        var cells = GetMatrixData(formula, matrixsource);
                        return new BBMatrixAtom(matrixsource, new MatrixAtom(matrixsource, cells));
                    }
                    
                case "cases":
                    {
                        if (position == value.Length)
                            throw new TexParseException("illegal end!");
                        SkipWhiteSpace(value, ref position);

                        var matrixsource = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);

                        var cells = GetMatrixData(formula, matrixsource);
                        return new CasesAtom(matrixsource, new MatrixAtom(matrixsource, cells, cellHAlignment: HorizontalAlignment.Left));
                    }
                    
                case "frac":
                    // Command is fraction.

                    var numeratorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar,
                        rightGroupChar), formula.TextStyle);
                    SkipWhiteSpace(value, ref position);
                    var denominatorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar,
                        rightGroupChar), formula.TextStyle);
                    if (numeratorFormula.RootAtom == null || denominatorFormula.RootAtom == null)
                        throw new TexParseException("Both numerator and denominator of a fraction can't be empty!");

                    source = value.Segment(start, position - start);
                    return new FractionAtom(source, numeratorFormula.RootAtom, denominatorFormula.RootAtom, true);

                case "left":
                    {
                        SkipWhiteSpace(value, ref position);
                        if (position == value.Length)
                            throw new TexParseException("`left` command should be passed a delimiter");

                        var delimiter = value[position];
                        ++position;
                        var left = position;

                        var internals = ParseUntilDelimiter(value, ref position, formula.TextStyle);

                        var opening = GetDelimiterSymbol(
                            GetDelimeterMapping(delimiter),
                            value.Segment(start, left - start));
                        if (opening == null)
                            throw new TexParseException($"Cannot find delimiter named {delimiter}");

                        var closing = internals.ClosingDelimiter;
                        source = value.Segment(start, position - start);
                        return new FencedAtom(source, internals.Body, opening, closing);
                    }
                    
                case "matrix":
                    {
                        if (position == value.Length)
                            throw new TexParseException("illegal end!");
                        SkipWhiteSpace(value, ref position);

                        var matrixsource = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);

                        var cells = GetMatrixData(formula, matrixsource);
                        return new MatrixAtom(matrixsource, cells);
                    }
                    
                case "overline":
                    {
                        var overlineFormula = this.Parse(this.ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), formula.TextStyle);
                        this.SkipWhiteSpace(value, ref position);
                        source = value.Segment(start, position - start);
                        return new OverlinedAtom(source, overlineFormula.RootAtom);
                    }
                    
                case "pmatrix":
                    {
                        if (position == value.Length)
                            throw new TexParseException("illegal end!");
                        SkipWhiteSpace(value, ref position);

                        var matrixsource = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);

                        var cells = GetMatrixData(formula, matrixsource);
                        return new PmatrixAtom(matrixsource, new MatrixAtom(matrixsource, cells));
                    }
                    
                case "right":
                    {
                        if (!allowClosingDelimiter)
                            throw new TexParseException("`right` command is not allowed without `left`");

                        SkipWhiteSpace(value, ref position);
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

                case "sqrt":
                    // Command is radical.

                    SkipWhiteSpace(value, ref position);
                    if (position == value.Length)
                        throw new TexParseException("illegal end!");

                    int sqrtEnd = position;

                    TexFormula degreeFormula = null;
                    if (value[position] == leftBracketChar)
                    {
                        // Degree of radical- is specified.
                        degreeFormula = Parse(ReadGroup(formula, value, ref position, leftBracketChar,
                            rightBracketChar), formula.TextStyle);
                        SkipWhiteSpace(value, ref position);
                    }

                    var sqrtFormula = this.Parse(
                        this.ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar),
                        formula.TextStyle);

                    if (sqrtFormula.RootAtom == null)
                    {
                        throw new TexParseException("The radicand of a square root can't be empty!");
                    }

                    source = value.Segment(start, sqrtEnd - start);
                    return new Radical(source, sqrtFormula.RootAtom, degreeFormula?.RootAtom);

                case "underline":
                    {
                        var underlineFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), formula.TextStyle);
                        SkipWhiteSpace(value, ref position);
                        source = value.Segment(start, position - start);
                        return new UnderlinedAtom(source, underlineFormula.RootAtom);
                    }
                case "color":
                    {
                        var colorName = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                        var remainingString = value.Segment(position);
                        var remaining = Parse(remainingString, formula.TextStyle);
                        position = value.Length;
                        if (predefinedColors.TryGetValue(colorName.ToString(), out var color))
                        {
                            source = value.Segment(start, position - start);
                            return new StyledAtom(source, remaining.RootAtom, null, new SolidColorBrush(color));
                        }

                        throw new TexParseException($"Color {colorName} not found");
                    }
                case "colorbox":
                    {
                        var colorName = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                        var remainingString = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                        var remaining = Parse(remainingString, formula.TextStyle);
                        if (predefinedColors.TryGetValue(colorName.ToString(), out var color))
                        {
                            source = value.Segment(start, position - start);
                            return new StyledAtom(source, remaining.RootAtom, new SolidColorBrush(color), null);
                        }

                        throw new TexParseException($"Color {colorName} not found");
                    }
                 
                case "vmatrix":
                    {
                        if (position == value.Length)
                            throw new TexParseException("illegal end!");
                        SkipWhiteSpace(value, ref position);

                        var matrixsource = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);

                        var cells = GetMatrixData(formula, matrixsource);
                        return new VmatrixAtom(matrixsource, new MatrixAtom(matrixsource, cells));
                    }

                case "Vmatrix":
                    {
                        if (position == value.Length)
                            throw new TexParseException("illegal end!");
                        SkipWhiteSpace(value, ref position);

                        var matrixsource = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);

                        var cells = GetMatrixData(formula, matrixsource);
                        return new VVMatrixAtom(matrixsource, new MatrixAtom(matrixsource, cells));
                    }
                    
            }

            throw new TexParseException("Invalid command.");
        }

        /// <summary>
        /// Retrives the cells of a matrix from a given source.
        /// </summary>
        private List<List<Atom>> GetMatrixData(TexFormula formula, SourceSpan matrixsource)
        {
            List<List<StringBuilder>> rowdata = new List<List<StringBuilder>>() { new List<StringBuilder>() { new StringBuilder() } };
            int rows = 0;
            int cols = 0;
            //how many characters the next row should skip for its sourcespan to start
            int rowindent = 0;
            //to ensure multiple row separators aren't used
            string rowseparationstyle = null;
            for (int i = 0; i < matrixsource.ToString().Length; i++)
            {
                var curchar = matrixsource.ToString()[i];
                var nextchar = i < matrixsource.ToString().Length - 1 ? matrixsource.ToString()[i + 1] : matrixsource.ToString()[i];
                var thirdchar = i < matrixsource.ToString().Length - 2 ? matrixsource.ToString()[i + 2] : matrixsource.ToString()[i];

                if (curchar == '\\' && nextchar == '\\')
                {
                    if (rowseparationstyle == null || rowseparationstyle == "slash")
                    {
                        if (i + 2 == matrixsource.ToString().Length || String.IsNullOrWhiteSpace(matrixsource.ToString().Substring(i + 2)))
                        {
                            i += matrixsource.ToString().Length - i;
                        }
                        else
                        {
                            rowdata.Add(new List<StringBuilder>() { new StringBuilder() });
                            rows++;
                            cols = 0;
                            i += 2;
                            rowindent = 2;
                            rowseparationstyle = "slash";

                            //rowdata[rows][cols].Append((matrixsource.ToString()[i +3].ToString()));
                        }
                    }
                    else
                    {
                        throw new TexParseException("Multiple row separator styles cannot be used.");
                    }
                }
                else if (curchar == '\\' && nextchar == 'c' && thirdchar == 'r')
                {
                    if (rowseparationstyle == null || rowseparationstyle == "cr")
                    {
                        if (i + 3 == matrixsource.ToString().Length || String.IsNullOrWhiteSpace(matrixsource.ToString().Substring(i + 3)))
                        {
                            i += matrixsource.ToString().Length - i;
                        }
                        else
                        {
                            rowdata.Add(new List<StringBuilder>() { new StringBuilder() });
                            rows++;
                            cols = 0;
                            i += 3;
                            rowindent = 3;
                            rowseparationstyle = "cr";
                        }

                    }
                    else
                    {
                        throw new TexParseException("Multiple row separator styles cannot be used.");
                    }
                }
                else if (curchar == leftGroupChar)
                {
                    var nestedgroup = ReadGroup(matrixsource.ToString(), leftGroupChar, rightGroupChar, i);

                    rowdata[rows][cols].Append("{" + nestedgroup + "}");
                    i += nestedgroup.Length + 1;
                }
                else if (curchar == '&')
                {
                    rowdata[rows].Add(new StringBuilder());
                    cols++;
                }
                else
                {
                    rowdata[rows][cols].Append(curchar.ToString());
                }
            }

            List<List<Atom>> matrixcells = new List<List<Atom>>();
            int matrixsrcstart = 0;
            int columnscount = 0;
            for (int i = 0; i < rowdata.Count; i++)
            {
                var rowitem = rowdata[i];
                if (rowitem.Count > 0)
                {
                    List<Atom> rowcellatoms = new List<Atom>();
                    for (int j = 0; j < rowitem.Count; j++)
                    {
                        var cellitem = rowdata[i][j];
                        if (cellitem.ToString().Trim().Length > 0)
                        {
                            var cellsource = matrixsource.Segment(matrixsrcstart, cellitem.Length);// new SourceSpan(cellitem, matrixsrcstart, cellitem.Length);
                            var cellformula = Parse(cellsource, formula.TextStyle);
                            rowcellatoms.Add(cellformula.RootAtom);

                            if (j < (rowitem.Count - 1))
                            {
                                matrixsrcstart += (cellitem.Length + 1);
                            }
                            else
                            {
                                matrixsrcstart += (cellitem.Length + rowindent + 1);
                            }
                        }

                    }

                    matrixcells.Add(rowcellatoms);
                    columnscount = rowcellatoms.Count;
                }

            }

            int colsvalid = 0;
            foreach (var item in matrixcells)
            {
                if (item.Count == columnscount)
                {
                    colsvalid++;
                }
            }

            if (colsvalid == matrixcells.Count)
            {
                return matrixcells;
            }
            else
            {
                throw new TexParseException("The column numbers are not equal.");
            }
        }
        
        private void ProcessEscapeSequence(
            TexFormula formula,
            SourceSpan value,
            ref int position,
            bool allowClosingDelimiter,
            ref bool closedDelimiter)
        {
            var initialSrcPosition = position;
            position++;
            var start = position;
            while (position < value.Length)
            {
                var ch = value[position];
                var isEnd = position == value.Length - 1;
                if (!char.IsLetter(ch) || isEnd)
                {
                    // Escape sequence has ended
                    // Or it's a symbol. Assuming in this case it will only be a single char.
                    if ((isEnd && char.IsLetter(ch)) || position - start == 0)
                    {
                        position++;
                    }
                    break;
                }

                position++;
            }

            var commandSpan = value.Segment(start, position - start);
            var command = commandSpan.ToString();
            var formulaSource = new SourceSpan(value.Source, initialSrcPosition, commandSpan.End);

            SymbolAtom symbolAtom = null;
            if (SymbolAtom.TryGetAtom(commandSpan, out symbolAtom))
            {
                // Symbol was found.

                if (symbolAtom.Type == TexAtomType.Accent)
                {
                    var helper = new TexFormulaHelper(formula, formulaSource);
                    TexFormula accentFormula = ReadScript(formula, value, ref position);
                    helper.AddAccent(accentFormula, symbolAtom.Name);
                }
                else if (symbolAtom.Type == TexAtomType.BigOperator)
                {
                    var opAtom = new BigOperatorAtom(formulaSource, symbolAtom, null, null);
                    formula.Add(this.AttachScripts(formula, value, ref position, opAtom), formulaSource);
                }
                else
                {
                    formula.Add(this.AttachScripts(formula, value, ref position, symbolAtom), formulaSource);
                }
            }
            else if (predefinedFormulas.TryGetValue(command, out var factory))
            {
                // Predefined formula was found.
                var predefinedFormula = factory(formulaSource);
                var atom = this.AttachScripts(formula, value, ref position, predefinedFormula.RootAtom);
                formula.Add(atom, formulaSource);
            }
            else if (command.Equals("nbsp"))
            {
                // Space was found.
                var atom = this.AttachScripts(formula, value, ref position, new SpaceAtom(formulaSource));
                formula.Add(atom, formulaSource);
            }
            else if (textStyles.Contains(command))
            {
                // Text style was found.
                this.SkipWhiteSpace(value, ref position);
                var styledFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), command);
                if (styledFormula.RootAtom == null)
                    throw new TexParseException("Styled text can't be empty!");
                var atom = this.AttachScripts(formula, value, ref position, styledFormula.RootAtom);
                var source = new SourceSpan(formulaSource.Source, formulaSource.Start, position);
                formula.Add(atom, source);
            }
            else if (commands.Contains(command))
            {
                // Command was found.
                var commandAtom = this.ProcessCommand(
                    formula,
                    value,
                    ref position,
                    command,
                    allowClosingDelimiter,
                    ref closedDelimiter);

                commandAtom = allowClosingDelimiter
                    ? commandAtom
                    : AttachScripts(
                        formula,
                        value,
                        ref position,
                        commandAtom);

                var source = new SourceSpan(formulaSource.Source, formulaSource.Start, commandAtom.Source.End);
                formula.Add(commandAtom, source);
            }
            else
            {
                // Escape sequence is invalid.
                throw new TexParseException("Unknown symbol or command or predefined TeXFormula: '" + command + "'");
            }
        }

        private Atom AttachScripts(TexFormula formula, SourceSpan value, ref int position, Atom atom, bool skipWhiteSpace = true)
        {
            if (skipWhiteSpace)
            {
                SkipWhiteSpace(value, ref position);
            }

            var initialPosition = position;
            if (position == value.Length)
                return atom;

            // Check for prime marks.
            var primesRowAtom = new RowAtom(new SourceSpan(value.Source, position, 0));
            int i = position;
            while (i < value.Length)
            {
                if (value[i] == primeChar)
                {
                    primesRowAtom = primesRowAtom.Add(SymbolAtom.GetAtom("prime", value.Segment(i, 1)));
                    position++;
                }
                else if (!IsWhiteSpace(value[i]))
                    break;
                i++;
            }

            var primesRowSource = new SourceSpan(
                value.Source,
                primesRowAtom.Source.Start,
                position - primesRowAtom.Source.Start);
            primesRowAtom = primesRowAtom.WithSource(primesRowSource);

            if (primesRowAtom.Elements.Count > 0)
                atom = new ScriptsAtom(primesRowAtom.Source, atom, null, primesRowAtom);

            if (position == value.Length)
                return atom;

            TexFormula superscriptFormula = null;
            TexFormula subscriptFormula = null;

            var ch = value[position];
            if (ch == superScriptChar)
            {
                // Attach superscript.
                position++;
                superscriptFormula = ReadScript(formula, value, ref position);

                SkipWhiteSpace(value, ref position);
                if (position < value.Length && value[position] == subScriptChar)
                {
                    // Attach subscript also.
                    position++;
                    subscriptFormula = ReadScript(formula, value, ref position);
                }
            }
            else if (ch == subScriptChar)
            {
                // Add subscript.
                position++;
                subscriptFormula = ReadScript(formula, value, ref position);

                SkipWhiteSpace(value, ref position);
                if (position < value.Length && value[position] == superScriptChar)
                {
                    // Attach superscript also.
                    position++;
                    superscriptFormula = ReadScript(formula, value, ref position);
                }
            }

            if (superscriptFormula == null && subscriptFormula == null)
                return atom;

            // Check whether to return Big Operator or Scripts.
            var subscriptAtom = subscriptFormula?.RootAtom;
            var superscriptAtom = superscriptFormula?.RootAtom;
            if (atom.GetRightType() == TexAtomType.BigOperator)
            {
                var source = value.Segment(atom.Source.Start, position - atom.Source.Start);
                if (atom is BigOperatorAtom typedAtom)
                {
                    return new BigOperatorAtom(
                        source,
                        typedAtom.BaseAtom,
                        subscriptAtom,
                        superscriptAtom,
                        typedAtom.UseVerticalLimits);
                }

                return new BigOperatorAtom(source, atom, subscriptAtom, superscriptAtom);
            }
            else
            {
                var source = new SourceSpan(value.Source, initialPosition, position - initialPosition);
                return new ScriptsAtom(source, atom, subscriptAtom, superscriptAtom);
            }
        }

        private Atom ConvertCharacter(TexFormula formula, ref int position, SourceSpan source)
        {
            var character = source[0];
            position++;
            if (IsSymbol(character) && formula.TextStyle != TexUtilities.TextStyleName)
            {
                // Character is symbol.
                var symbolName = symbols.ElementAtOrDefault(character);
                if (string.IsNullOrEmpty(symbolName))
                    throw new TexParseException($"Unknown character : '{character}'");

                try
                {
                    return SymbolAtom.GetAtom(symbolName, source);
                }
                catch (SymbolNotFoundException e)
                {
                    throw new TexParseException("The character '"
                            + character.ToString()
                            + "' was mapped to an unknown symbol with the name '"
                            + (string)symbolName + "'!", e);
                }
            }
            else // Character is alpha-numeric or should be rendered as text.
            {
                return new CharAtom(source, character, formula.TextStyle);
            }
        }

        private void SkipWhiteSpace(SourceSpan value, ref int position)
        {
            while (position < value.Length && IsWhiteSpace(value[position]))
                position++;
        }
    }
}
