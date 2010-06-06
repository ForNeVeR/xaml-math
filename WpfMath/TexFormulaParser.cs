using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;

namespace WpfMath
{
    // TODO: Put all error strings into resources.
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
        private static ISet<string> commands;
        private static IList<string> symbols;
        private static IList<string> delimeters;
        private static ISet<string> textStyles;
        private static IDictionary<string, TexFormula> predefinedFormulas;

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

        // True if parser has been initialized.
        private static bool isInitialized;

        static TexFormulaParser()
        {
            isInitialized = false;

            predefinedFormulas = new Dictionary<string, TexFormula>();
        }

        internal static string[][] DelimiterNames
        {
            get { return delimiterNames; }
        }

        public static void Initialize()
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


            commands = new HashSet<string>();
            commands.Add("frac");
            commands.Add("sqrt");

            var formulaSettingsParser = new TexPredefinedFormulaSettingsParser();
            symbols = formulaSettingsParser.GetSymbolMappings();
            delimeters = formulaSettingsParser.GetDelimiterMappings();
            textStyles = formulaSettingsParser.GetTextStyles();

            isInitialized = true;

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
            var result = SymbolAtom.GetAtom(name);
            if (!result.IsDelimeter)
                return null;
            return result;
        }

        private static bool IsSymbol(char c)
        {
            return !((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'));
        }

        private static bool IsWhiteSpace(char ch)
        {
            return ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
        }

        public TexFormulaParser()
        {
            if (!isInitialized)
                throw new InvalidOperationException("Parser has not yet been initialized.");
        }

        public TexFormula Parse(string value)
        {
            var formula = new TexFormula();
            var position = 0;
            while (position < value.Length)
            {
                char ch = value[position];
                if (IsWhiteSpace(ch))
                {
                    position++;
                }
                else if (ch == escapeChar)
                {
                    ProcessEscapeSequence(formula, value, ref position);
                }
                else if (ch == leftGroupChar)
                {
                    formula.Add(AttachScripts(formula, value, ref position, Parse(ReadGroup(formula, value, ref position,
                        leftGroupChar, rightGroupChar)).RootAtom));
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
                    var scriptsAtom = AttachScripts(formula, value, ref position,
                        ConvertCharacter(formula, value, ref position, ch));
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
            if (position == value.Length)
                throw new TexParseException("illegal end, missing script!");

            SkipWhiteSpace(value, ref position);
            var ch = value[position];
            if (ch == leftGroupChar)
            {
                return Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar));
            }
            else
            {
                position++;
                return Parse(ch.ToString());
            }
        }

        private Atom ProcessCommand(TexFormula formula, string value, ref int position, string command)
        {
            SkipWhiteSpace(value, ref position);

            switch (command)
            {
                case "frac":
                    // Command is fraction.

                    var numeratorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar));
                    SkipWhiteSpace(value, ref position);
                    var denominatorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar));
                    if (numeratorFormula.RootAtom == null || denominatorFormula.RootAtom == null)
                        throw new TexParseException("Both numerator and denominator of a fraction can't be empty!");

                    return new FractionAtom(numeratorFormula.RootAtom, denominatorFormula.RootAtom, true);
                case "sqrt":
                    // Command is radical.

                    SkipWhiteSpace(value, ref position);
                    if (position == value.Length)
                        throw new TexParseException("illegal end!");

                    TexFormula degreeFormula = null;
                    if (value[position] == leftBracketChar)
                    {
                        // Degree of radical- is specified.
                        degreeFormula = Parse(ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar));
                        SkipWhiteSpace(value, ref position);
                    }

                    return new Radical(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar))
                        .RootAtom, degreeFormula == null ? null : degreeFormula.RootAtom);
            }

            throw new TexParseException("Invalid command.");
        }

        private void ProcessEscapeSequence(TexFormula formula, string value, ref int position)
        {
            var result = new StringBuilder();
            position++;
            while (position < value.Length)
            {
                var ch = value[position];
                var isEnd = position == value.Length - 1;
                if (!char.IsLetter(ch) || isEnd)
                {
                    // Escape sequence has ended.
                    if (isEnd)
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

            try
            {
                symbolAtom = SymbolAtom.GetAtom(command);
            }
            catch (SymbolNotFoundException)
            {
            }

            try
            {
                predefinedFormula = GetFormula(command);
            }
            catch (FormulaNotFoundException)
            {
            }

            if (symbolAtom != null)
            {
                // Symbol was found.

                formula.Add(AttachScripts(formula, value, ref position, symbolAtom));
            }
            else if (predefinedFormula != null)
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
                var styledFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar));
                styledFormula.TextStyle = command;
                formula.Add(AttachScripts(formula, value, ref position, styledFormula.RootAtom));
            }
            else if (commands.Contains(command))
            {
                // Command was found.

                formula.Add(AttachScripts(formula, value, ref position, ProcessCommand(formula, value, ref position,
                    command)));
            }
            else
            {
                // Escape sequence is invalid.
                throw new TexParseException("Unknown symbol or command or predefined TeXFormula: '" + command + "'");
            }

        }

        private Atom AttachScripts(TexFormula formula, string value, ref int position, Atom atom)
        {
            SkipWhiteSpace(value, ref position);

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
            if (atom.GetRightType() == TexAtomType.BigOperator)
                return new BigOperatorAtom(atom, subscriptFormula == null ? null : subscriptFormula.RootAtom,
                    superscriptFormula == null ? null : superscriptFormula.RootAtom);
            else
                return new ScriptsAtom(atom, subscriptFormula == null ? null : subscriptFormula.RootAtom,
                    superscriptFormula == null ? null : superscriptFormula.RootAtom);
        }

        private Atom ConvertCharacter(TexFormula formula, string value, ref int position, char character)
        {
            position++;
            if (IsSymbol(character))
            {
                // Character is symbol.
                var symbolName = symbols[character];
                if (symbolName == null)
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
