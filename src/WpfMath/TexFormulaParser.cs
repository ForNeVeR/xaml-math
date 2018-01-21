using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;
using System.Windows.Media;
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
        private static IDictionary<string, TexFormula> predefinedFormulas;
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
            predefinedFormulas = new Dictionary<string, TexFormula>();
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
                "frac",
                "left",
                "right",
                "sqrt",
                "color",
                "colorbox"
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

        internal static TexFormula GetFormula(string name)
        {
            try
            {
                return new TexFormula(predefinedFormulas[name]);
            }
            catch (KeyNotFoundException)
            {
                throw new FormulaNotFoundException(name);
            }
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

        internal static SymbolAtom GetDelimiterSymbol(string name)
        {
            if (name == null)
                return null;

            var result = SymbolAtom.GetAtom(name);
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

        private static bool ShouldSkipWhiteSpace(string style) => style != "text";

        public TexFormula Parse(string value, string textStyle = null)
        {
            var position = 0;
            return Parse(value, ref position, false, textStyle);
        }

        private DelimiterInfo ParseUntilDelimiter(string value, ref int position, string textStyle)
        {
            var embeddedFormula = Parse(value, ref position, true, textStyle);
            if (embeddedFormula.RootAtom == null)
                throw new TexParseException("Cannot find closing delimiter");

            var bodyRow = embeddedFormula.RootAtom as RowAtom;
            var lastAtom = bodyRow?.Elements.LastOrDefault() ?? embeddedFormula.RootAtom;
            var lastDelimiter = lastAtom as SymbolAtom;
            if (lastDelimiter == null || !lastDelimiter.IsDelimeter)
                throw new TexParseException($"Cannot find closing delimiter; got {lastDelimiter} instead");

            Atom bodyAtom;
            if (bodyRow == null)
            {
                bodyAtom = new RowAtom();
            }
            else if (bodyRow.Elements.Count > 2)
            {
                var row = new RowAtom();
                row.Elements.AddRange(bodyRow.Elements.Take(bodyRow.Elements.Count - 1));
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

        private TexFormula Parse(string value, ref int position, bool allowClosingDelimiter, string textStyle)
        {
            var formula = new TexFormula() { TextStyle = textStyle };
            var closedDelimiter = false;
            var skipWhiteSpace = ShouldSkipWhiteSpace(textStyle);
            while (position < value.Length && !(allowClosingDelimiter && closedDelimiter))
            {
                char ch = value[position];
                if (IsWhiteSpace(ch))
                {
                    if (!skipWhiteSpace)
                    {
                        formula.Add(new SpaceAtom());
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
                    var innerGroupAtom = parsedGroup.RootAtom ?? new RowAtom();
                    var groupAtom = new TypedAtom(innerGroupAtom, TexAtomType.Ordinary, TexAtomType.Ordinary);
                    formula.Add(AttachScripts(formula, value, ref position, groupAtom));
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
                    var scriptsAtom = AttachScripts(
                        formula,
                        value,
                        ref position,
                        ConvertCharacter(formula, value, ref position, ch),
                        skipWhiteSpace);
                    formula.Add(scriptsAtom);
                }
            }

            return formula;
        }

        private string ReadGroup(TexFormula formula, string value, ref int position, char openChar, char closeChar)
        {
            if (position == value.Length || value[position] != openChar)
                throw new TexParseException("missing '" + openChar + "'!");

            var result = new StringBuilder();
            var group = 0;
            position++;
            while (position < value.Length && !(value[position] == closeChar && group == 0))
            {
                if (value[position] == openChar)
                    group++;
                else if (value[position] == closeChar)
                    group--;
                result.Append(value[position]);
                position++;
            }

            if (position == value.Length)
            {
                // Reached end of formula but group has not been closed.
                throw new TexParseException("Illegal end,  missing '" + closeChar + "'!");
            }

            position++;
            return result.ToString();
        }

        private TexFormula ReadScript(TexFormula formula, string value, ref int position)
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
                return Parse(ch.ToString(), formula.TextStyle);
            }
        }

        private Atom ProcessCommand(
            TexFormula formula,
            string value,
            ref int position,
            string command,
            bool allowClosingDelimiter,
            ref bool closedDelimiter)
        {
            SkipWhiteSpace(value, ref position);

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

                    return new FractionAtom(numeratorFormula.RootAtom, denominatorFormula.RootAtom, true);

                case "left":
                    {
                        SkipWhiteSpace(value, ref position);
                        if (position == value.Length)
                            throw new TexParseException("`left` command should be passed a delimiter");

                        var delimiter = value[position];
                        ++position;

                        var internals = ParseUntilDelimiter(value, ref position, formula.TextStyle);

                        var opening = GetDelimiterSymbol(GetDelimeterMapping(delimiter));
                        if (opening == null)
                            throw new TexParseException($"Cannot find delimiter named {delimiter}");

                        var closing = internals.ClosingDelimiter;
                        return new FencedAtom(internals.Body, opening, closing);
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

                        var closing = GetDelimiterSymbol(GetDelimeterMapping(delimiter));
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

                    TexFormula degreeFormula = null;
                    if (value[position] == leftBracketChar)
                    {
                        // Degree of radical- is specified.
                        degreeFormula = Parse(ReadGroup(formula, value, ref position, leftBracketChar,
                            rightBracketChar), formula.TextStyle);
                        SkipWhiteSpace(value, ref position);
                    }

                    var sqrtFormula = Parse(
                        ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar),
                        formula.TextStyle);

                    if (sqrtFormula.RootAtom == null)
                    {
                        throw new TexParseException("The radicand of a square root can't be empty!");
                    }

                    return new Radical(sqrtFormula.RootAtom, degreeFormula?.RootAtom);
                case "color":
                    {
                        var colorName = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                        string remainingString = value.Substring(position);
                        var remaining = Parse(remainingString, formula.TextStyle);
                        position = value.Length;
                        Color color;
                        if (predefinedColors.TryGetValue(colorName, out color))
                        {
                            return new StyledAtom(remaining.RootAtom, null, new SolidColorBrush(color));
                        }
                        else
                        {
                            throw new TexParseException(String.Format("Color {0} not found", colorName));
                        }
                    }
                case "colorbox":
                    {
                        var colorName = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                        string remainingString = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                        var remaining = Parse(remainingString, formula.TextStyle);
                        Color color;
                        if (predefinedColors.TryGetValue(colorName, out color))
                        {
                            return new StyledAtom(remaining.RootAtom, new SolidColorBrush(color), null);
                        }
                        else
                        {
                            throw new TexParseException(String.Format("Color {0} not found", colorName));
                        }
                    }
            }

            throw new TexParseException("Invalid command.");
        }

        private void ProcessEscapeSequence(
            TexFormula formula,
            string value,
            ref int position,
            bool allowClosingDelimiter,
            ref bool closedDelimiter)
        {
            var result = new StringBuilder();
            position++;
            while (position < value.Length)
            {
                var ch = value[position];
                var isEnd = position == value.Length - 1;
                if (!char.IsLetter(ch) || isEnd)
                {
                    // Escape sequence has ended
                    // Or it's a symbol. Assuming in this case it will only be a single char.
                    if ((isEnd && char.IsLetter(ch)) || result.Length == 0)
                    {
                        result.Append(ch);
                        position++;
                    }
                    break;
                }

                result.Append(ch);
                position++;
            }

            var command = result.ToString();

            SymbolAtom symbolAtom = null;
            TexFormula predefinedFormula = null;
            if (SymbolAtom.TryGetAtom(command, out symbolAtom))
            {
                // Symbol was found.

                if (symbolAtom.Type == TexAtomType.Accent)
                {
                    TexFormulaHelper helper = new TexFormulaHelper(formula);
                    TexFormula accentFormula = ReadScript(formula, value, ref position);
                    helper.AddAccent(accentFormula, symbolAtom.Name);
                }
                else if (symbolAtom.Type == TexAtomType.BigOperator)
                {
                    var opAtom = new BigOperatorAtom(symbolAtom, null, null);
                    formula.Add(AttachScripts(formula, value, ref position, opAtom));
                }
                else
                {
                    formula.Add(AttachScripts(formula, value, ref position, symbolAtom));
                }
            }
            else if (predefinedFormulas.TryGetValue(command, out predefinedFormula))
            {
                // Predefined formula was found.

                formula.Add(AttachScripts(formula, value, ref position, predefinedFormula.RootAtom));
            }
            else if (command.Equals("nbsp"))
            {
                // Space was found.

                formula.Add(AttachScripts(formula, value, ref position, new SpaceAtom()));
            }
            else if (textStyles.Contains(command))
            {
                // Text style was found.

                SkipWhiteSpace(value, ref position);
                var styledFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), command);
                if (styledFormula.RootAtom == null)
                    throw new TexParseException("Styled text can't be empty!");
                formula.Add(AttachScripts(formula, value, ref position, styledFormula.RootAtom));
            }
            else if (commands.Contains(command))
            {
                // Command was found.
                var commandAtom = ProcessCommand(
                            formula,
                            value,
                            ref position,
                            command,
                            allowClosingDelimiter,
                            ref closedDelimiter);

                commandAtom = allowClosingDelimiter ?
                                    commandAtom :
                                    AttachScripts(
                                        formula,
                                        value,
                                        ref position,
                                        commandAtom);

                formula.Add(commandAtom);
            }
            else
            {
                // Escape sequence is invalid.
                throw new TexParseException("Unknown symbol or command or predefined TeXFormula: '" + command + "'");
            }

        }

        private Atom AttachScripts(TexFormula formula, string value, ref int position, Atom atom, bool skipWhiteSpace = true)
        {
            if (skipWhiteSpace)
            {
                SkipWhiteSpace(value, ref position);
            }

            if (position == value.Length)
                return atom;

            // Check for prime marks.
            var primesRowAtom = new RowAtom();
            int i = position;
            while (i < value.Length)
            {
                if (value[i] == primeChar)
                {
                    primesRowAtom.Add(SymbolAtom.GetAtom("prime"));
                    position++;
                }
                else if (!IsWhiteSpace(value[i]))
                    break;
                i++;
            }

            if (primesRowAtom.Elements.Count > 0)
                atom = new ScriptsAtom(atom, null, primesRowAtom);

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
                if (atom is BigOperatorAtom)
                {
                    var typedAtom = (BigOperatorAtom)atom;
                    return new BigOperatorAtom(typedAtom.BaseAtom, subscriptAtom, superscriptAtom,
                        typedAtom.UseVerticalLimits);
                }

                return new BigOperatorAtom(atom, subscriptAtom, superscriptAtom);
            }
            else
            {
                return new ScriptsAtom(atom, subscriptAtom, superscriptAtom);
            }
        }

        private Atom ConvertCharacter(TexFormula formula, string value, ref int position, char character)
        {
            position++;
            if (IsSymbol(character))
            {
                // Character is symbol.
                var symbolName = symbols.ElementAtOrDefault(character);
                if (string.IsNullOrEmpty(symbolName))
                    throw new TexParseException("Unknown character : '" + character.ToString() + "'");

                try
                {
                    return SymbolAtom.GetAtom(symbolName);
                }
                catch (SymbolNotFoundException e)
                {
                    throw new TexParseException("The character '"
                            + character.ToString()
                            + "' was mapped to an unknown symbol with the name '"
                            + (string)symbolName + "'!", e);
                }
            }
            else
            {
                // Character is alpha-numeric.
                return new CharAtom(character, formula.TextStyle);
            }
        }

        private void SkipWhiteSpace(string value, ref int position)
        {
            while (position < value.Length && IsWhiteSpace(value[position]))
                position++;
        }
    }
}
