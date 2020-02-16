using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using WpfMath.Atoms;
using WpfMath.Colors;
using WpfMath.Exceptions;
using WpfMath.Parsers;
using WpfMath.Utils;

namespace WpfMath
{
    // TODO: Put all error strings into resources.
    // TODO: Use TextReader for lexing.
    public class TexFormulaParser
    {
        // Special characters for parsing
        private const char escapeChar = '\\';

        internal const char leftGroupChar = '{';
        internal const char rightGroupChar = '}';
        private const char leftBracketChar = '[';
        private const char rightBracketChar = ']';

        private const char subScriptChar = '_';
        private const char superScriptChar = '^';
        private const char primeChar = '\'';

        /// <summary>
        /// A set of names of the commands that are embedded in the parser itself, <see cref="ProcessCommand"/>.
        /// These're not the additional commands that may be supplied via <see cref="_commandRegistry"/>.
        /// </summary>
        private static readonly HashSet<string> embeddedCommands = new HashSet<string>
        {
            "color",
            "colorbox",
            "frac",
            "left",
            "overline",
            "right",
            "sqrt"
        };

        private static IList<string> symbols;
        private static IList<string> delimeters;
        private static HashSet<string> textStyles;
        private static readonly IDictionary<string, Func<SourceSpan, TexFormula>> predefinedFormulas =
            new Dictionary<string, Func<SourceSpan, TexFormula>>();

        private static readonly string[][] delimiterNames =
        {
            new[] { "lbrace", "rbrace" },
            new[] { "(", ")" },
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

            var formulaSettingsParser = new TexPredefinedFormulaSettingsParser();
            symbols = formulaSettingsParser.GetSymbolMappings();
            delimeters = formulaSettingsParser.GetDelimiterMappings();
            textStyles = formulaSettingsParser.GetTextStyles();

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

        /// <summary>A registry for additional commands.</summary>
        private readonly IReadOnlyDictionary<string, ICommandParser> _commandRegistry;

        private readonly IReadOnlyDictionary<string, IColorParser> _colorModelParsers;

        /// <summary>A color parser for cases when the color model isn't specified.</summary>
        private readonly IColorParser _defaultColorParser;

        internal TexFormulaParser(
            IReadOnlyDictionary<string, ICommandParser> commandRegistry,
            IReadOnlyDictionary<string, IColorParser> colorModelParsers,
            IColorParser defaultColorParser)
        {
            _commandRegistry = commandRegistry;
            _colorModelParsers = colorModelParsers;
            _defaultColorParser = defaultColorParser;
        }

        public TexFormulaParser(
            IReadOnlyDictionary<string, IColorParser> colorModelParsers,
            IColorParser defaultColorParser) : this(StandardCommands.Dictionary, colorModelParsers, defaultColorParser)
        {}

        public TexFormulaParser() : this(
            StandardColorParsers.Dictionary,
            PredefinedColorParser.Instance)
        {}

        public TexFormula Parse(string value, string textStyle = null)
        {
            Debug.WriteLine(value);
            var position = 0;
            return Parse(
                new SourceSpan(value, 0, value.Length),
                ref position,
                false,
                textStyle,
                DefaultCommandEnvironment.Instance);
        }

        internal TexFormula Parse(SourceSpan value, string textStyle, ICommandEnvironment environment)
        {
            int localPostion = 0;
            return Parse(value, ref localPostion, false, textStyle, environment);
        }

        private DelimiterInfo ParseUntilDelimiter(
            SourceSpan value,
            ref int position,
            string textStyle,
            ICommandEnvironment environment)
        {
            var embeddedFormula = Parse(value, ref position, true, textStyle, environment);
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

        private TexFormula Parse(
            SourceSpan value,
            ref int position,
            bool allowClosingDelimiter,
            string textStyle,
            ICommandEnvironment environment)
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
                    ProcessEscapeSequence(
                        formula,
                        value,
                        ref position,
                        allowClosingDelimiter,
                        ref closedDelimiter,
                        environment);
                }
                else if (ch == leftGroupChar)
                {
                    var groupValue = ReadElement(value, ref position);
                    var parsedGroup = Parse(groupValue, textStyle, environment.CreateChildEnvironment());
                    var innerGroupAtom = parsedGroup.RootAtom ?? new RowAtom(groupValue);
                    var groupAtom = new TypedAtom(
                        innerGroupAtom.Source,
                        innerGroupAtom,
                        TexAtomType.Ordinary,
                        TexAtomType.Ordinary);
                    var scriptsAtom = this.AttachScripts(formula, value, ref position, groupAtom, true, environment);
                    formula.Add(scriptsAtom, value.Segment(initialPosition, position - initialPosition));
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
                    var character = ConvertCharacter(formula, ref position, source, environment);
                    if (character != null)
                    {
                        var scriptsAtom = AttachScripts(
                            formula,
                            value,
                            ref position,
                            character,
                            skipWhiteSpace,
                            environment);
                        formula.Add(scriptsAtom, value.Segment(initialPosition, position));
                    }
                }
            }

            return formula;
        }

        private static TexFormula ConvertRawText(SourceSpan value, string textStyle)
        {
            var formula = new TexFormula { TextStyle = textStyle };

            var position = 0;
            var initialPosition = position;
            while (position < value.Length)
            {
                var ch = value[position];
                var source = value.Segment(position, 1);
                var atom = IsWhiteSpace(ch)
                    ? (Atom) new SpaceAtom(source)
                    : new CharAtom(source, ch, textStyle);
                position++;
                formula.Add(atom, value.Segment(initialPosition, position - initialPosition));
            }

            return formula;
        }

        internal static SourceSpan ReadElementGroup(SourceSpan value, ref int position, char openChar, char closeChar)
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

        /// <summary>Reads a char-delimited element group if it exists; returns <c>null</c> if it isn't.</summary>
        private static SourceSpan ReadElementGroupOptional(
            SourceSpan value,
            ref int position,
            char openChar,
            char closeChar)
        {
            SkipWhiteSpace(value, ref position);
            if (value[position] != openChar)
                return null;

            return ReadElementGroup(value, ref position, openChar, closeChar);
        }

        private static SourceSpan ReadEscapeSequence(SourceSpan value, ref int position)
        {
            var initialPosition = position;
            if (value[initialPosition] != escapeChar)
                throw new Exception($"Invalid state: {nameof(ReadEscapeSequence)} called for a value without escape character ({value})");

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

            var length = position - initialPosition;
            if (length <= 1)
                throw new TexParseException($"Unfinished escape sequence (value: \"{value}\", index {position})");

            return value.Segment(initialPosition, length);
        }

        private SymbolAtom ParseDelimiter(SourceSpan value, int start, ref int position)
        {
            var delimiter = ReadElement(value, ref position);

            string delimiterName;
            if (delimiter.Length == 1)
                delimiterName = GetDelimeterMapping(delimiter[0]);
            else
            {
                if (delimiter[0] != escapeChar)
                    throw new Exception($"Incorrect parser state: delimiter should start from {escapeChar}: {delimiter}");

                // Here goes the fancy business: for non-alphanumeric commands (e.g. \{, \\ etc.) we need to pass them
                // through GetDelimeterMapping, but for alphanumeric ones, we don't.
                delimiterName = delimiter.Segment(1).ToString(); // skip an escape character
                if (delimiterName.Length == 1 && !char.IsLetterOrDigit(delimiterName[0]))
                    delimiterName = GetDelimeterMapping(delimiterName[0]);
            }

            var delimiterSource = value.Segment(start, position - start); // will map the whole "\left(" to a delimiter atom created
            if (delimiterName == null || !SymbolAtom.TryGetAtom(delimiterName, delimiterSource, out var atom) || !atom.IsDelimeter)
                throw new TexParseException($"Cannot find delimiter {delimiter}");

            return atom;
        }

        /// <summary>
        /// Reads an element: typically, a curly brace-enclosed value group, a singular value or a character sequence
        /// prefixed by a backslash.
        /// </summary>
        /// <exception cref="TexParseException">Will be thrown for ill-formed groups.</exception>
        internal static SourceSpan ReadElement(SourceSpan value, ref int position)
        {
            SkipWhiteSpace(value, ref position);

            if (position == value.Length)
                throw new TexParseException("An element is missing");

            return value[position] switch
            {
                leftGroupChar => ReadElementGroup(value, ref position, leftGroupChar, rightGroupChar),
                escapeChar => ReadEscapeSequence(value, ref position),
                _ => value.Segment(position++, 1)
            };
        }

        private TexFormula ReadScript(
            TexFormula formula,
            SourceSpan value,
            ref int position,
            ICommandEnvironment environment) =>
            Parse(ReadElement(value, ref position), formula.TextStyle, environment.CreateChildEnvironment());

        /// <remarks>May return <c>null</c> for commands that produce no atoms.</remarks>
        private Tuple<AtomAppendMode, Atom> ProcessCommand(
            TexFormula formula,
            SourceSpan value,
            ref int position,
            string command,
            bool allowClosingDelimiter,
            ref bool closedDelimiter,
            ICommandEnvironment environment)
        {
            int start = position - command.Length;

            SourceSpan source;
            switch (command)
            {
                case "frac":
                    {
                        var numeratorFormula = Parse(
                            ReadElement(value, ref position),
                            formula.TextStyle,
                            environment.CreateChildEnvironment());
                        var denominatorFormula = Parse(
                            ReadElement(value, ref position),
                            formula.TextStyle,
                            environment.CreateChildEnvironment());
                        source = value.Segment(start, position - start);
                        return new Tuple<AtomAppendMode, Atom>(
                            AtomAppendMode.Add,
                            new FractionAtom(
                                source,
                                numeratorFormula.RootAtom,
                                denominatorFormula.RootAtom,
                                true));
                    }
                case "left":
                    {
                        SkipWhiteSpace(value, ref position);
                        if (position == value.Length)
                            throw new TexParseException("`left` command should be passed a delimiter");

                        var opening = ParseDelimiter(value, start, ref position);
                        var internals = ParseUntilDelimiter(value, ref position, formula.TextStyle, environment);
                        var closing = internals.ClosingDelimiter;
                        source = value.Segment(start, position - start);
                        return new Tuple<AtomAppendMode, Atom>(
                            AtomAppendMode.Add,
                            new FencedAtom(source, internals.Body, opening, closing));
                    }
                case "overline":
                    {
                        var overlineFormula = Parse(
                            ReadElement(value, ref position),
                            formula.TextStyle,
                            environment.CreateChildEnvironment());
                        source = value.Segment(start, position - start);
                        return new Tuple<AtomAppendMode, Atom>(
                            AtomAppendMode.Add,
                            new OverlinedAtom(source, overlineFormula.RootAtom));
                    }
                case "right":
                    {
                        if (!allowClosingDelimiter)
                            throw new TexParseException("`right` command is not allowed without `left`");

                        SkipWhiteSpace(value, ref position);
                        if (position == value.Length)
                            throw new TexParseException("`right` command should be passed a delimiter");

                        var closing = ParseDelimiter(value, start, ref position);

                        closedDelimiter = true;
                        return new Tuple<AtomAppendMode, Atom>(AtomAppendMode.Add, closing);
                    }
                case "sqrt":
                    {
                        // Command is radical.
                        SkipWhiteSpace(value, ref position);

                        TexFormula degreeFormula = null;
                        if (value.Length > position && value[position] == leftBracketChar)
                        {
                            // Degree of radical is specified.
                            degreeFormula = Parse(
                                ReadElementGroup(value, ref position, leftBracketChar, rightBracketChar),
                                formula.TextStyle,
                                environment.CreateChildEnvironment());
                        }

                        var sqrtFormula = this.Parse(
                            ReadElement(value, ref position),
                            formula.TextStyle,
                            environment.CreateChildEnvironment());

                        source = value.Segment(start, position - start);
                        return new Tuple<AtomAppendMode, Atom>(
                            AtomAppendMode.Add,
                            new Radical(source, sqrtFormula.RootAtom, degreeFormula?.RootAtom));
                    }
                case "color":
                {
                    var color = ReadColorModelData(value, ref position);

                    var bodyValue = ReadElement(value, ref position);
                    var bodyFormula = Parse(bodyValue, formula.TextStyle, environment.CreateChildEnvironment());
                    source = value.Segment(start, position - start);

                    return new Tuple<AtomAppendMode, Atom>(
                        AtomAppendMode.Add,
                        new StyledAtom(source, bodyFormula.RootAtom, null, new SolidColorBrush(color)));
                }
                case "colorbox":
                {
                    var color = ReadColorModelData(value, ref position);

                    var bodyValue = ReadElement(value, ref position);
                    var bodyFormula = Parse(bodyValue, formula.TextStyle, environment.CreateChildEnvironment());
                    source = value.Segment(start, position - start);

                    return new Tuple<AtomAppendMode, Atom>(
                        AtomAppendMode.Add,
                        new StyledAtom(source, bodyFormula.RootAtom, new SolidColorBrush(color), null));
                }
                }

            if (environment.AvailableCommands.TryGetValue(command, out var parser)
                || _commandRegistry.TryGetValue(command, out parser))
            {
                var context = new CommandContext(this, formula, environment, value, start, position);
                var parseResult = parser.ProcessCommand(context);
                if (parseResult.NextPosition < position)
                    throw new TexParseException(
                        $"Incorrect parser behavior for command {command}: NextPosition = {parseResult.NextPosition}, position = {position}. Parser did not made any progress.");

                position = parseResult.NextPosition;
                return Tuple.Create(parseResult.AppendMode, parseResult.Atom);
            }

            throw new TexParseException("Invalid command.");
        }

        /// <summary>Reads an optional square braced color model name, and then a color name.</summary>
        /// <returns>Returns a color parsed.</returns>
        /// <exception cref="TexParseException">Gets thrown in case of nonexistent color model or color.</exception>
        private Color ReadColorModelData(SourceSpan value, ref int position)
        {
            var colorModelName = ReadElementGroupOptional(
                value,
                ref position,
                leftBracketChar,
                rightBracketChar)?.ToString();
            var colorDefinition = ReadElement(value, ref position).ToString();
            var colorComponents = colorDefinition
                .Split(',')
                .Select(c => c.Trim())
                .ToList();

            var colorParser = string.IsNullOrEmpty(colorModelName)
                ? _defaultColorParser
                : _colorModelParsers.TryGetValue(colorModelName, out var currentColorParser)
                    ? currentColorParser
                    : throw new TexParseException($"Unknown color model name: {colorModelName}");

            var color = colorParser.Parse(colorComponents);
            if (color == null)
                throw new TexParseException(
                    $"Color {colorDefinition} could not be parsed by the {colorModelName ?? "default"} color model.");

            return color.Value;
        }

        private void ProcessEscapeSequence(TexFormula formula,
            SourceSpan value,
            ref int position,
            bool allowClosingDelimiter,
            ref bool closedDelimiter,
            ICommandEnvironment environment)
        {
            var initialSrcPosition = position;
            var commandSpan = ReadEscapeSequence(value, ref position).Segment(1);
            var command = commandSpan.ToString();
            var formulaSource = new SourceSpan(value.Source, initialSrcPosition, commandSpan.End);

            SymbolAtom symbolAtom = null;
            if (SymbolAtom.TryGetAtom(commandSpan, out symbolAtom))
            {
                // Symbol was found.

                if (symbolAtom.Type == TexAtomType.Accent)
                {
                    var helper = new TexFormulaHelper(formula, formulaSource);
                    TexFormula accentFormula = ReadScript(formula, value, ref position, environment);
                    helper.AddAccent(accentFormula, symbolAtom.Name);
                }
                else if (symbolAtom.Type == TexAtomType.BigOperator)
                {
                    var opAtom = new BigOperatorAtom(formulaSource, symbolAtom, null, null);
                    formula.Add(AttachScripts(formula, value, ref position, opAtom, true, environment), formulaSource);
                }
                else
                {
                    formula.Add(
                        AttachScripts(formula, value, ref position, symbolAtom, true, environment), formulaSource);
                }
            }
            else if (predefinedFormulas.TryGetValue(command, out var factory))
            {
                // Predefined formula was found.
                var predefinedFormula = factory(formulaSource);
                var atom = AttachScripts(formula, value, ref position, predefinedFormula.RootAtom, true, environment);
                formula.Add(atom, formulaSource);
            }
            else if (command.Equals("nbsp"))
            {
                // Space was found.
                var atom = AttachScripts(formula, value, ref position, new SpaceAtom(formulaSource), true, environment);
                formula.Add(atom, formulaSource);
            }
            else if (textStyles.Contains(command))
            {
                // Text style was found.
                SkipWhiteSpace(value, ref position);

                var styledFormula = command == TexUtilities.TextStyleName
                    ? ConvertRawText(ReadElement(value, ref position), command)
                    : Parse(ReadElement(value, ref position), command, environment.CreateChildEnvironment());

                var source = value.Segment(commandSpan.Start, position - commandSpan.Start);
                var atom = styledFormula.RootAtom ?? new NullAtom(source);
                var commandAtom = AttachScripts(formula, value, ref position, atom, true, environment);
                formula.Add(commandAtom, source);
            }
            else if (embeddedCommands.Contains(command)
                 || environment.AvailableCommands.ContainsKey(command)
                 || _commandRegistry.ContainsKey(command))
            {
                // Command was found.
                var (appendMode, commandAtom) = ProcessCommand(
                    formula,
                    value,
                    ref position,
                    command,
                    allowClosingDelimiter,
                    ref closedDelimiter,
                    environment);

                if (commandAtom != null)
                {
                    commandAtom = allowClosingDelimiter
                        ? commandAtom
                        : AttachScripts(
                            formula,
                            value,
                            ref position,
                            commandAtom,
                            true,
                            environment);

                    var source = new SourceSpan(
                        formulaSource.Source,
                        formulaSource.Start,
                        commandAtom.Source?.End ?? position);
                    switch (appendMode)
                    {
                        case AtomAppendMode.Add:
                            formula.Add(commandAtom, source);
                            break;
                        case AtomAppendMode.Replace:
                            formula.RootAtom = commandAtom;
                            break;
                    }
                }
            }
            else
            {
                // Escape sequence is invalid.
                throw new TexParseException("Unknown symbol or command or predefined TeXFormula: '" + command + "'");
            }
        }

        private Atom AttachScripts(
            TexFormula formula,
            SourceSpan value,
            ref int position,
            Atom atom,
            bool skipWhiteSpace,
            ICommandEnvironment environment)
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
                superscriptFormula = ReadScript(formula, value, ref position, environment);

                SkipWhiteSpace(value, ref position);
                if (position < value.Length && value[position] == subScriptChar)
                {
                    // Attach subscript also.
                    position++;
                    subscriptFormula = ReadScript(formula, value, ref position, environment);
                }
            }
            else if (ch == subScriptChar)
            {
                // Add subscript.
                position++;
                subscriptFormula = ReadScript(formula, value, ref position, environment);

                SkipWhiteSpace(value, ref position);
                if (position < value.Length && value[position] == superScriptChar)
                {
                    // Attach superscript also.
                    position++;
                    superscriptFormula = ReadScript(formula, value, ref position, environment);
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

        /// <remarks>May return <c>null</c>.</remarks>
        private Atom ConvertCharacter(
            TexFormula formula,
            ref int position,
            SourceSpan source,
            ICommandEnvironment environment)
        {
            var character = source[0];
            position++;
            if (IsSymbol(character) && formula.TextStyle != TexUtilities.TextStyleName)
            {
                // Character is symbol.
                var symbolName = symbols.ElementAtOrDefault(character);
                if (string.IsNullOrEmpty(symbolName))
                {
                    if (environment.ProcessUnknownCharacter(formula, character))
                        return null;

                    throw new TexParseException($"Unknown character : '{character}'");
                }

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

        private static void SkipWhiteSpace(SourceSpan value, ref int position)
        {
            while (position < value.Length && IsWhiteSpace(value[position]))
                position++;
        }
    }
}
