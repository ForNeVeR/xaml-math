using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Text;
using System.Text.RegularExpressions;
using WpfMath.Atoms;
using WpfMath.Exceptions;
using WpfMath.Utils;

namespace WpfMath.Parsers
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
        private const char commentChar='%';
        #endregion
        #region Information used for parsing
        private static HashSet<string> commands;
        private static IList<string> symbols;
        private static IList<string> delimeters;
        private static HashSet<string> textStyles;
        private static readonly IDictionary<string, Func<SourceSpan, TexFormula>> predefinedFormulas =
            new Dictionary<string, Func<SourceSpan, TexFormula>>();
        private static IDictionary<string, Color> predefinedColors;
        private static Dictionary<string, Color> userdefinedColors;

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
                "color",
                "colorbox",
                "definecolor",
                "enclose",
                "frac",
                "hide",
                "it",
                "left",
                "overline",
                "phantom",
                "right",
                "rm",
                "sqrt",
                "table",
                "underline",
            };

            userdefinedColors = new Dictionary<string, Color>();
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
            else if (ch == escapeChar)
            {
                StringBuilder sb = new StringBuilder("\\");
                position++;
                while (position < value.Length && IsWhiteSpace(value[position]) == false && value[position] != escapeChar)
                {
                    sb.Append(value[position].ToString());
                    position++;
                }
                var scriptlength = sb.Length;
                var scriptsrc = value.Segment(position - scriptlength, scriptlength);
                return Parse(scriptsrc, formula.TextStyle);
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
                case "color":
                    {
                        //Command to change the foreground color 
                        var colorName = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);

                        var remainingString = value.Segment(position);
                        var remaining = Parse(remainingString, formula.TextStyle);
                        position = value.Length;
                        source = value.Segment(start, position - start);
                        if (predefinedColors.TryGetValue(colorName.ToString(), out Color color))
                        {
                            return new StyledAtom(source,remaining.RootAtom, null, new SolidColorBrush(color));
                        }
                        else if(userdefinedColors.ContainsKey(colorName.ToString()))
                        {
                            source = value.Segment(start, position - start);
                            return new StyledAtom(source, remaining.RootAtom, null, new SolidColorBrush(userdefinedColors[colorName.ToString()]));
                        }
                        else
                        {
                            try
                            {
                                Color color1 = UserDefinedColorParser.Parse(colorName.ToString());
                                return new StyledAtom(source,remaining.RootAtom, null, new SolidColorBrush(color1));
                            }
                            catch
                            {
                                string helpstr= HelpOutMessage(colorName.ToString(), predefinedColors.Keys.ToList());
                                int a =position-remainingString.Length-3-colorName.Length;
                                throw new TexParseException($"Color {colorName.ToString()} at columns {a} and {a+colorName.Length} could either not be found or converted{helpstr}.");
                            }
                            
                        }
                    }
                case "colorbox":
                    {
                        //Command to change the background color
                        var colorName = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                        var remainingString = value.Segment(position);
                        var remaining = Parse(remainingString, formula.TextStyle);
                        position = value.Length;
                        source = value.Segment(start, position - start);
                        if (predefinedColors.TryGetValue(colorName.ToString(), out Color color))
                        {
                            return new StyledAtom(source,remaining.RootAtom, new SolidColorBrush(color), null);
                        }
                        else if(userdefinedColors.ContainsKey(colorName.ToString()))
                        {
                            source = value.Segment(start, position - start);
                            return new StyledAtom(source, remaining.RootAtom, null, new SolidColorBrush(userdefinedColors[colorName.ToString()]));
                        }
                        else
                        {
                            try
                            {
                                Color color1 = UserDefinedColorParser.Parse(colorName.ToString());
                                return new StyledAtom(source,remaining.RootAtom, new SolidColorBrush(color1), null);
                            }
                            catch (Exception)
                            {
                                string helpstr= HelpOutMessage(colorName.ToString(), predefinedColors.Keys.ToList());
                                int a =position-remainingString.Length-3-colorName.Length;
                                throw new TexParseException($"Color {colorName.ToString()} at columns {a} and {a+colorName.Length} could either not be found or converted{helpstr}.");
                            }
                        }
                    }
                case "definecolor":
                    {
                        //Syntax:\definecolor{colorname}{colormodel}{colordefinition}
                        var paramgroups = new string[] { "", "", "" };
                        for (int i = 0; i < 3;i++)
                        {
                            var paramgroup = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                            paramgroups[i] = paramgroup.ToString();
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            if (paramgroups[i].Trim().Length==0)
                            {
                                if (i==1){throw new TexParseException("The name of the color cannot be empty");}
                                else if (i == 2){throw new TexParseException("The name of the color model cannot be empty");}
                                else if (i == 3){throw new TexParseException("The color definition cannot be empty");}
                            }
                        }
                        if (userdefinedColors.ContainsKey(paramgroups[0]))
                        {
                            userdefinedColors[paramgroups[0]] = ColorUtilities.Parse(paramgroups[1], paramgroups[2]);
                        }
                        else
                        {
                            userdefinedColors.Add( paramgroups[0],ColorUtilities.Parse(paramgroups[1], paramgroups[2]));
                        }
                        int a = paramgroups[0].Length+ paramgroups[1].Length+ paramgroups[2].Length+6;
                        source = value.Segment(start, position - start-a); 
                        return new SpaceAtom(source);
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
                            enclosetypes = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar).ToString();
                        }


                        var enclosedItemFormula = Parse(
                            ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), formula.TextStyle);

                        if (enclosedItemFormula.RootAtom == null)
                        {
                            throw new TexParseException("The enclosed item can't be empty!");
                        }
                        source = value.Segment(start, position - start);
                        return new EnclosedAtom(source,enclosedItemFormula.RootAtom, enclosetypes);
                    }
                    
                case "frac":{
                        // Command is fraction.
                        SkipWhiteSpace(value, ref position);
                        if(position==value.Length)
                            throw new TexParseException("illegal end!");
                        
                        if (value[position]==leftGroupChar)
                        {
                            var numeratorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar,
                                                   rightGroupChar), formula.TextStyle);
                            SkipWhiteSpace(value, ref position);
                            var denominatorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar,
                                                     rightGroupChar), formula.TextStyle);
                            //if (numeratorFormula.RootAtom == null || denominatorFormula.RootAtom == null)
                            //    throw new TexParseException("Both numerator and denominator of a fraction can't be empty!");

                            source = value.Segment(start, position - start);
                            return new FractionAtom(source, numeratorFormula.RootAtom, denominatorFormula.RootAtom, true);

                        }
                        else
                        {
                            return GetSimpleFractionAtom(formula, value, ref position);
                        }
                    }
                 case "it":
                    {
                        formula.TextStyle="mathit";
                        return new NullAtom(new SourceSpan ("", position,0));
                    }
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
                    
                case "overline":
                    {
                        SkipWhiteSpace(value,ref position);
                        if(position==value.Length)
                            throw new TexParseException("illegal end!");
                        if (value[position]==leftGroupChar)
                        {
                            var overlineFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), formula.TextStyle);
                            SkipWhiteSpace(value, ref position);
                            source = value.Segment(start, position - start);
                            return new OverlinedAtom(source, overlineFormula.RootAtom);
                        }
                        else
                        {
                            source = GetSimpleUngroupedSource(value,ref position);
                            var overlineFormula = Parse(source, formula.TextStyle);
                            return new OverlinedAtom(source, overlineFormula.RootAtom);
                        }
                    }

                case "phantom":
                case "hide":
                    {
                        SkipWhiteSpace(value, ref position);
                        if (position == value.Length)
                            throw new TexParseException("illegal end!");

                        var phantomItemFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), formula.TextStyle);
                        source = value.Segment(start, position - start);
                        return new PhantomAtom(source,phantomItemFormula.RootAtom);
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
                        if(position==value.Length)
                            throw new TexParseException("illegal end!");
                        if (value[position]==leftBracketChar||value[position]==leftGroupChar)
                        {
                            if (position == value.Length)
                                throw new TexParseException("illegal end!");

                            int sqrtEnd = position;

                            TexFormula degreeFormula = null;
                            bool degreerequested=false;
                            if (value[position] == leftBracketChar)
                            {
                                // Degree of radical- is specified.
                                degreerequested= true;
                                degreeFormula = Parse(ReadGroup(formula, value, ref position, leftBracketChar,
                                    rightBracketChar), formula.TextStyle);
                                SkipWhiteSpace(value, ref position);
                            }

                            var sqrtFormula = this.Parse(
                                this.ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar),
                                formula.TextStyle);

                            if (sqrtFormula.RootAtom == null)
                            {
                                throw new TexParseException($"The radicand of the square root at column {position - 1} can't be empty!");
                            }

                            source = value.Segment(start, sqrtEnd - start);
                            return new Radical(source, sqrtFormula.RootAtom, degreeFormula?.RootAtom, degreerequested );
                        }
                        else
                        {
                            source = GetSimpleUngroupedSource(value,ref position);
                            var sqrtFormula = Parse(source, formula.TextStyle);
                            return new Radical(source, sqrtFormula.RootAtom, null, false);
                        }
                    }
                    
                case "underline":
                    {
                        if(position==value.Length)
                            throw new TexParseException("illegal end!");
                        if (value[position]==leftGroupChar)
                        {
                            var underlineFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), formula.TextStyle);
                            SkipWhiteSpace(value, ref position);
                            source = value.Segment(start, position - start);
                            return new UnderlinedAtom(source, underlineFormula.RootAtom);
                        }
                        else
                        {
                            source = GetSimpleUngroupedSource(value,ref position);
                            var underlineFormula = Parse(source, formula.TextStyle);
                            return new UnderlinedAtom(source, underlineFormula.RootAtom);
                        }
                    }
            }
            throw new TexParseException("Invalid command.");
        }
        
        private Atom GetSimpleFractionAtom(TexFormula formula,SourceSpan value,ref int position)
        {
            SourceSpan source;
            bool fracparamsfound = false;
            StringBuilder sb = new StringBuilder();
            int srcstart = position;
            while (position < value.Length && fracparamsfound == false && value[position] != escapeChar)
            {
                string curChar = value[position].ToString();
                if (curChar == "{")
                {
                    var groupsource = value.Segment(position, value.Length - position);
                    var denomgroup = ReadGroup(groupsource.ToString(), leftGroupChar, rightGroupChar, 0);
                    sb.Append("{" + denomgroup + "}");
                    position += denomgroup.Length + 2;
                    fracparamsfound = true;
                }
                else if (curChar == " ")
                {
                    fracparamsfound = true;
                }
                else
                {
                    sb.Append(value[position].ToString());
                    position++;
                }
            }

            var fracParamsLength = sb.ToString().Length;
            source = fracParamsLength == 0 ? new SourceSpan("  ", position, 2) : value.Segment(srcstart, fracParamsLength);

            int midLength = ((int)Math.Floor((double)(source.Length / 2)));
            TexFormula numeratorFormula = null;
            TexFormula denominatorFormula = null;

            if(fracparamsfound=false)
                throw new TexParseException("The current fraction style is invalid");
            
            if (Regex.IsMatch(sb.ToString(), @".+/.+"))
            {
                midLength = sb.ToString().Split('/')[0].Length;
                numeratorFormula = Parse(source.Segment(0, midLength), formula.TextStyle);
                denominatorFormula = Parse(source.Segment(midLength + 1, source.ToString().Substring(midLength).Length), formula.TextStyle);
            }
            if (Regex.IsMatch(sb.ToString(), @"[.]+[{][.]+[}]") || sb.ToString().Contains("{") == true)
            {
                midLength = sb.ToString().Split('{')[0].Length;
                numeratorFormula = Parse(source.Segment(0, midLength), formula.TextStyle);
                denominatorFormula = Parse(source.Segment(midLength), formula.TextStyle);
            }

            if (sb.ToString().Contains("/") == false && sb.ToString().Contains("{") == false && sb.ToString().Contains("}") == false)
            {
                midLength = ((int)Math.Floor((double)(source.Length / 2)));
                numeratorFormula = Parse(source.Segment(0, midLength), formula.TextStyle);
                denominatorFormula = Parse(source.Segment(midLength), formula.TextStyle);
            }
            if(denominatorFormula==null)
            {
                throw new TexParseException("The denominator cannot be null");
            }
            return new FractionAtom(source, numeratorFormula.RootAtom, denominatorFormula.RootAtom, true);
        }
        
        private SourceSpan GetSimpleUngroupedSource(SourceSpan value,ref int position)
        {
            SourceSpan result;
            if (value[position] == escapeChar)
            {
                StringBuilder sb = new StringBuilder("\\");
                position++;
                while (position < value.Length && IsWhiteSpace(value[position]) == false && value[position] != escapeChar && Char.IsLetter(value[position]))
                {
                    sb.Append(value[position].ToString());
                    position++;
                }
                var grouplength = sb.Length;
                result = value.Segment(position - grouplength, grouplength);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                while (position < value.Length && IsWhiteSpace(value[position]) == false && value[position] != escapeChar)
                {
                    sb.Append(value[position].ToString());
                    position++;
                }
                var grouplength = sb.Length;
                result = value.Segment(position - grouplength, grouplength);
            }
            return result;
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
