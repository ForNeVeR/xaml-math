using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WpfMath.Atoms;
using WpfMath.Exceptions;

namespace WpfMath.Parsers
{
    /// <summary>
    /// Represents the method that will handle the TeX command.
    /// </summary>
    /// <param name="formula"></param>
    /// <param name="value"></param>
    /// <param name="position"></param>
    /// <param name="allowClosingDelimiter"></param>
    /// <param name="closedDelimiter"></param>
    /// <returns></returns>
    public delegate Atom TexCommandHandler(TexFormula formula, SourceSpan value, ref int position, bool allowClosingDelimiter, ref bool closedDelimiter);
    /// <summary>
    /// Represents the method that will handle the TeX environment.
    /// </summary>
    public delegate Atom TexEnvironmentHandler(TexFormula formula, SourceSpan environmentSource);

    // TODO: Put all error strings into resources.
    // TODO: Use TextReader for lexing.
    public class TexFormulaParser
    {
        // Special characters for parsing
        public char escapeChar => '\\';

        public char leftGroupChar => '{';
        public char rightGroupChar => '}';
        public char leftBracketChar => '[';
        public char rightBracketChar => ']';

        public char subScriptChar => '_';
        private char superScriptChar => '^';
        public char primeChar => '\'';

        // Information used for parsing
        private static IList<string> symbols;
        public static IList<string> delimeters;
        private static HashSet<string> textStyles;
        private static readonly IDictionary<string, Func<SourceSpan, TexFormula>> predefinedFormulas =
            new Dictionary<string, Func<SourceSpan, TexFormula>>();
        public static IDictionary<string, Color> predefinedColors;

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

        /// <summary>
        /// Gets or sets the commands recognized by this <see cref="TexFormulaParser"/> and their associated methods.
        /// </summary>
        public Dictionary<string, TexCommandHandler> TexCommands { get; private set; }

        /// <summary>
        /// Gets or sets the environments recognized by this <see cref="TexFormulaParser"/> and their associated methods.
        /// </summary>
        public Dictionary<string, TexEnvironmentHandler> TexEnvironments { get; private set; }

        public TexFormulaParser()
        {
            Initialize();
        }

        internal static string[][] DelimiterNames
        {
            get { return delimiterNames; }
        }

        private void Initialize()
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

            predefinedColors = new Dictionary<string, Color>();

            TexCommands = new Dictionary<string, TexCommandHandler>();
            TexEnvironments = new Dictionary<string, TexEnvironmentHandler>();
            LoadDefaultCommandsandEnvironments();

            var formulaSettingsParser = new TexPredefinedFormulaSettingsParser();
            symbols = formulaSettingsParser.GetSymbolMappings();
            delimeters = formulaSettingsParser.GetDelimiterMappings();
            textStyles = formulaSettingsParser.GetTextStyles();

            var colorParser = new PredefinedColorParser();
            colorParser.Parse(predefinedColors);

            var predefinedFormulasParser = new TexPredefinedFormulaParser();
            predefinedFormulasParser.Parse(predefinedFormulas);
        }

        /// <summary>
        /// Contains commands that cannot be changed.
        /// </summary>
        private string[] UnchangeableCommands = new string[]
        {
            "left","right","begin","end","color","definecolor"
        };

        /// <summary>
        /// Adds a new command for the <see cref="TexFormulaParser"/> to use.
        /// </summary>
        /// <param name="commandname"></param>
        /// <param name="commandType"></param>
        /// <param name="commandFunction"></param>
        public void AddNewCommand(string commandname, TexCommandHandler commandFunction)
        {
            if (UnchangeableCommands.Contains(commandname) || TexCommands.ContainsKey(commandname))
            {
                throw new InvalidOperationException($"The command, \"{commandname}\", appears to be a core command, may negatively affect the proper functioning of this parser. Try a different command name");
            }
            else
            {
                TexCommands.Add(commandname, commandFunction);
            }
        }

        /// <summary>
        /// Removes a command that is no longer required.
        /// </summary>
        /// <param name="commandname"></param>
        public void RemoveCommand(string commandname)
        {
            if (TexCommands.ContainsKey(commandname) && !UnchangeableCommands.Contains(commandname))
            {
                TexCommands.Remove(commandname);
            }
        }

        private void LoadDefaultCommandsandEnvironments()
        {
            //Add commands to the TexCommands dictionary

            TexCommands.Add("color", this.ColorCommand);
            TexCommands.Add("colobox", this.ColorboxCommand);
            TexCommands.Add("frac", this.FracCommand);
            TexCommands.Add("left", this.LeftCommand);
            TexCommands.Add("overline", this.OverlineCommand);
            TexCommands.Add("right", this.RightCommand);
            TexCommands.Add("sqrt", this.SqrtCommand);
            TexCommands.Add("underline", this.UnderlineCommand);

            //Add environment names and their handlers to the TexEnvironments dictionary

        }

        public static string GetDelimeterMapping(char character)
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

        public static SymbolAtom GetDelimiterSymbol(string name, SourceSpan source)
        {
            if (name == null)
                return null;

            var result = SymbolAtom.GetAtom(name, source);
            if (!result.IsDelimeter)
                return null;
            return result;
        }

        public static bool IsSymbol(char c)
        {
            return !char.IsLetterOrDigit(c);
        }

        public static bool IsWhiteSpace(char ch)
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

        public TexFormula Parse(SourceSpan value, string textStyle)
        {
            int localPostion = 0;
            return Parse(value, ref localPostion, false, textStyle);
        }

        public DelimiterInfo ParseUntilDelimiter(SourceSpan value, ref int position, string textStyle)
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

        public SourceSpan ReadGroup(TexFormula formula, SourceSpan value, ref int position, char openChar, char closeChar)
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

        public TexFormula ReadScript(TexFormula formula, SourceSpan value, ref int position)
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

            if (command == "begin")
            {
                //Syntax: \begin{Environment-Name}...\end{Environment-Name}
                if (position == value.Length)
                    throw new TexParseException("illegal end!");
                SkipWhiteSpace(value, ref position);

                var beginEnvironmentName = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar).ToString().Trim();
                if (!TexEnvironments.ContainsKey(beginEnvironmentName))
                {
                    throw new TexParseException($"The environment, \"{beginEnvironmentName}\" is unavailable at the moment");
                }
                bool environmentEndReached = false;
                int environmentDeepness = 0;
                int environmentSourceStart = position;
                int environmentSourceLength = 0;

                while (position < value.Length && environmentEndReached == false)
                {
                    if (value[position] == escapeChar)
                    {
                        position++;
                        if (position < value.Length)
                        {
                            if (Char.IsLetter(value[position]))
                            {
                                StringBuilder commandSB = new StringBuilder(value[position]);

                                while (position < value.Length && Char.IsLetter(value[position]))
                                {
                                    commandSB.Append(value[position].ToString());
                                    position++;
                                }

                                if (commandSB.ToString() == "begin")
                                {
                                    environmentDeepness++;
                                    environmentSourceLength += 6;
                                }
                                else if (commandSB.ToString() == "end")
                                {
                                    if (environmentDeepness == 0)
                                    {
                                        //try to check if the ending environment name is equal to the beginning environment name
                                        var endEnvironmentName = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar).ToString().Trim();
                                        if (endEnvironmentName == beginEnvironmentName)
                                        {
                                            environmentEndReached = true;
                                        }
                                        else
                                        {
                                            throw new TexParseException($"The environment name \"{beginEnvironmentName}\" does not match the ending environment name \"{endEnvironmentName}\"");
                                        }
                                    }
                                    else
                                    {
                                        environmentDeepness--;
                                        environmentSourceLength += 4;
                                    }
                                }
                                else
                                {
                                    environmentSourceLength += commandSB.Length + 1;
                                }
                            }
                            else
                            {
                                position++;
                                environmentSourceLength += 2;
                            }
                        }
                        
                    }
                    else
                    {
                        position++;
                        environmentSourceLength++;
                    }
                }

                if (environmentEndReached)
                {
                    //process it
                    try
                    {
                        return TexEnvironments[beginEnvironmentName].Invoke(formula, value.Segment(environmentSourceStart, environmentSourceLength));
                    }
                    catch (Exception)
                    {
                        throw new TexParseException($"The environment data is invalid");
                    }

                }
                else
                {
                    throw new TexParseException($"The end of the environment, \"{beginEnvironmentName}\", cannot be found");
                }
            }
            else 
            {
                return TexCommands[command].Invoke(formula, value, ref position, allowClosingDelimiter, ref closedDelimiter);
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
            else if (TexCommands.ContainsKey(command))
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

        public void SkipWhiteSpace(SourceSpan value, ref int position)
        {
            while (position < value.Length && IsWhiteSpace(value[position]))
                position++;
        }
    }

}
