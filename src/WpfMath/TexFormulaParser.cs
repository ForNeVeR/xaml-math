using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using WpfMath.Atoms;
using WpfMath.Exceptions;

namespace WpfMath
{
    // TODO: Put all error strings into resources.
    // TODO: Use TextReader for lexing.
    public class TexFormulaParser
    {
        #region Special characters for parsing
        private const char escapeChar = '\\';

        private const char leftGroupChar = '{';
        private const char rightGroupChar = '}';
        private const char leftBracketChar = '[';
        private const char rightBracketChar = ']';

        private const char subScriptChar = '_';
        private const char superScriptChar = '^';
        private const char primeChar = '\'';
        #endregion
        #region Information used for parsing
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
        #endregion
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
                "bgcolor",
                "colorbox",
                "enclose",
                "fgcolor",
                "frac",
                "hide",
                "image",
                "left",
                "matrix",
                "oline",
                "overline",
                "phantom",
                "photo",
                "right",
                "sqrt",
                "table",
                "tbl",
                "uline",
                "underline",
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
                    {
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
                        throw new TexParseException($"The radicand of the square root at column {position-1} can't be empty!");
                    }

                    source = value.Segment(start, sqrtEnd - start);
                    return new Radical(source, sqrtFormula.RootAtom, degreeFormula?.RootAtom);
                    }
                case "fgcolor":
                    {
                        //Command to change the foreground color 
                        var colorName = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);

                        string remainingString = value.Substring(position);
                        var remaining = Parse(remainingString, formula.TextStyle);
                        position = value.Length;

                        if (predefinedColors.TryGetValue(colorName, out Color color))
                        {
                            return new StyledAtom(remaining.RootAtom, null, new SolidColorBrush(color));
                        }
                        else
                        {
                            try
                            {
                                Color color1 = UserDefinedColorParser.ParseUserColor(colorName);
                                return new StyledAtom(remaining.RootAtom, null, new SolidColorBrush(color1));
                            }
                            catch
                            {
                                string helpstr= HelpOutMessage(colorName, predefinedColors.Keys.ToList());
                                int a =position-remainingString.Length-3-colorName.Length;
                                throw new TexParseException($"Color {colorName} at columns {a} and {a+colorName.Length} could either not be found or converted{helpstr}.");
                            }
                            
                        }
                    }
                case "bgcolor":
                    {
                        //Command to change the background color
                        var colorName = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                        string remainingString = value.Substring(position);
                        var remaining = Parse(remainingString, formula.TextStyle);
                        position = value.Length;

                        if (predefinedColors.TryGetValue(colorName, out Color color))
                        {
                            return new StyledAtom(remaining.RootAtom, new SolidColorBrush(color), null);
                        }
                        else
                        {
                            try
                            {
                                Color color1 = UserDefinedColorParser.ParseUserColor(colorName);
                                return new StyledAtom(remaining.RootAtom, new SolidColorBrush(color1), null);
                            }
                            catch (Exception ex)
                            {
                                string helpstr= HelpOutMessage(colorName, predefinedColors.Keys.ToList());
                                  int a =position-remainingString.Length-3-colorName.Length;
                                throw new TexParseException($"Color {colorName} at columns {a} and {a+colorName.Length} could either not be found or converted{helpstr}.");
                            }
                        }
                    }
                case "matrix":
                case "table":
                    {
                        //command requires a tabular arrangement of atoms.
                        SkipWhiteSpace(value, ref position);
                        if (position == value.Length)
                        {
                            throw new TexParseException("illegal end!");
                        }
                        //the def of table
                        string tableTypeDef = "0,0";
                        tableTypeDef= ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
                        string[] ttdstrarr = new string[] { };
                        SkipWhiteSpace(value, ref position);
                        if (tableTypeDef.Contains(",")&&Regex.IsMatch(tableTypeDef,@"[0-9]+,[0-9]+"))
                        {
                            ttdstrarr = tableTypeDef.Split(',') ;
                        }
                        if (tableTypeDef.Contains(",")==false)
                        {
                            throw new TexParseException("Invalid number of columns.");
                        }
                        if (tableTypeDef.Contains(",")==true && Regex.IsMatch(tableTypeDef, @"[0-9]+,[0-9]+")==false)
                        {
                            throw new TexParseException("Invalid number of rows and columns.");
                        }
                        uint rowsdefined = 0;
                        uint colsdefined = 0;
                        if (ttdstrarr.Length==2)
                        {
                            if (uint.TryParse(ttdstrarr[0], out rowsdefined) == true)
                            {
                            }
                            if (uint.TryParse(ttdstrarr[1], out colsdefined) == true)
                            {
                            }
                            if (uint.TryParse(ttdstrarr[0], out rowsdefined) == false)
                            {
                                throw new TexParseException("The number of rows of a table must be >=0.");
                            }
                            if (uint.TryParse(ttdstrarr[1], out colsdefined) == false)
                            {
                                throw new TexParseException("The number of columns of a table must be >=0.");
                            }
                        }
                        if (ttdstrarr.Length>2)
                        {
                            throw new TexParseException("Multiple parameters given for the table.");
                        }

                        //Need To work on the parsing stage from here downwards
                        List<List<TexFormula>> TableData = new List<List<TexFormula>>();
                        for (int i = 0; i < rowsdefined; i++)
                        {
                            List<TexFormula> rowData = new List<TexFormula>();
                            for (int j = 0; j < colsdefined; j++)
                            {
                                TexFormula colFxn = new TexFormula();
                                rowData.Add(colFxn);
                            }
                            TableData.Add(rowData);
                        }

                        List<TexFormula> cellsdata = new List<TexFormula>();
                        uint cellsdefined = rowsdefined * colsdefined;
                        for (uint i = 0; i < cellsdefined; i++)
                        {
                            SkipWhiteSpace(value, ref position);
                            var curcell= Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), formula.TextStyle);
                            cellsdata.Add(curcell);
                            SkipWhiteSpace(value, ref position);
                        }

                        //create a row atom list:Outer list holds an inner list which contains its cells
                        List<List<Atom>> tableDataAtoms = new List<List<Atom>>();
                        for (uint i = 0; i < cellsdefined; i+=colsdefined)
                        {
                            List<Atom> rowData = new List<Atom>();
                            for (uint j = i; j < i+colsdefined; j++)
                            {
                                var item = cellsdata[int.Parse(j.ToString())];
                                if (item==null||item.RootAtom==null)
                                {
                                    throw new TexParseException("The cells of a table cannot be empty.");
                                }
                                else
                                {
                                    rowData.Add(item.RootAtom);
                                }
                                
                            }
                            tableDataAtoms.Add(rowData);
                        }
                                               
                        return new TableAtom(tableDataAtoms);
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
                        else
                        {
                            try
                            {
                                Color color1 = UserDefinedColorParser.ParseUserColor(colorName);
                                return new StyledAtom(remaining.RootAtom, new SolidColorBrush(color1), null);
                            }
                            catch (Exception ex)
                            {
                                string helpstr= HelpOutMessage(colorName, predefinedColors.Keys.ToList());
                                int a =position-remainingString.Length-3-colorName.Length;
                                throw new TexParseException($"Color {colorName} at columns {a} and {a+colorName.Length} could either not be found or converted{helpstr}.");
                            }
                        }

                    }
                case "uline":
                case "underline":
                    {

                        var underlineFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar,
                            rightGroupChar), formula.TextStyle);
                        SkipWhiteSpace(value, ref position);
                        return new UnderlinedAtom(underlineFormula.RootAtom);
                    }
                case "oline":
                case "overline":
                    {
                        var overlineFormula= Parse(ReadGroup(formula, value, ref position, leftGroupChar,
                                                           rightGroupChar), formula.TextStyle);
                        SkipWhiteSpace(value, ref position);
                        return new OverlinedAtom(overlineFormula.RootAtom);
                    }
                case "enclose":
                    {
                        SkipWhiteSpace(value, ref position);
                        if (position == value.Length)
                            throw new TexParseException("illegal end!");
                        var enclosetypes = "circle";
                        if (value[position] == leftBracketChar)
                        {
                            // type of enclosure - is specified.
                            SkipWhiteSpace(value, ref position);
                            enclosetypes = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
                        }
                         

                        var enclosedItemFormula = Parse(
                            ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), formula.TextStyle);

                        if (enclosedItemFormula.RootAtom == null)
                        {
                            throw new TexParseException("The enclosed item can't be empty!");
                        }

                        return new EnclosedAtom(enclosedItemFormula.RootAtom,enclosetypes);
                    }

                case "hide":
                case "phantom":
                    {
                        SkipWhiteSpace(value, ref position);
                        if (position == value.Length)
                            throw new TexParseException("illegal end!");

                        var phantomItemFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), formula.TextStyle);

                        return new PhantomAtom(phantomItemFormula.RootAtom);
                    }

                case "photo":
                case "image":
                    {
                        SkipWhiteSpace(value, ref position);
                        if (position == value.Length)
                            throw new TexParseException("illegal end!");
                        var imagedim = "10:7";
                        if (value[position] == leftBracketChar)
                        {
                            // type of enclosure - is specified.
                            SkipWhiteSpace(value, ref position);
                            imagedim = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
                            SkipWhiteSpace(value, ref position);
                        }
                         
                        var imagePath = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);

                        return new ImageAtom(null,imagePath, imagedim);
                    }
            }

            throw new TexParseException("Invalid command.");
        }

        public static string HelpOutMessage(string input, List<string> database)
            {
            string helpStr=""; bool helpGiven=false;
            foreach(var item in database){
                if(input!=""&& input!=null&& input.Trim().Length>=1&& database!=null&&item!=null&&item!=""&& database.Count>0)
                   {
                    if(item.StartsWith(input)){
                        helpStr=$" Did you mean: {item}";
                        helpGiven=true;}
                    if(item.Contains(input))
                        {
                        if(helpGiven==false){
                            helpStr=$" Did you mean: {item}";
                            helpGiven=true;
                            }
                        else{continue;}
                        }
                    else{continue;}
                    }
                else{continue;}
                }
            return helpStr;
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
                
                List<string> somepossibleparams=new List<string>();
                foreach(var item in commands){somepossibleparams.Add(item);}
                 foreach(var item in delimeters){somepossibleparams.Add(item);}
                 foreach(var item in predefinedFormulas){somepossibleparams.Add(item.Key);}
                 foreach(var item in textStyles){somepossibleparams.Add(item);}
                 foreach(var item in symbols){somepossibleparams.Add(item);}
                
                string helpstr=HelpOutMessage(command,somepossibleparams);
                throw new TexParseException("Unknown symbol or command or predefined TeXFormula: '" + command + "'"+helpstr);
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
