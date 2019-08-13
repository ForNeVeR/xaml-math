using WpfMath.Atoms;
using WpfMath.Exceptions;

namespace WpfMath.Parsers
{
    /// <summary>A context that will be passed to the command parser.</summary>
    internal class CommandContext
    {
        /// <summary>TeX formula parser that calls the command.</summary>
        public TexFormulaParser Parser { get; }

        /// <summary>Current formula.</summary>
        public TexFormula Formula { get; }

        /// <summary>Source of the current formula. Includes both command name and the arguments.</summary>
        public SourceSpan FormulaSource { get; }

        /// <summary>
        /// A position inside of source where the command name start. Useful to provide the source information, not for
        /// the parsing itself.
        /// </summary>
        public int CommandNameStartPosition { get; }

        /// <summary>
        /// A position inside of source where the command arguments start. Should be a parser start position.
        /// </summary>
        public int ArgumentsStartPosition { get; }

        public CommandContext(TexFormulaParser parser, TexFormula formula, SourceSpan formulaSource, int commandNameStartPosition, int argumentsStartPosition)
        {
            Parser = parser;
            Formula = formula;
            FormulaSource = formulaSource;
            CommandNameStartPosition = commandNameStartPosition;
            ArgumentsStartPosition = argumentsStartPosition;
        }
    }

    internal class CommandProcessingResult
    {
        /// <summary>A parsed atom.</summary>
        public Atom Atom { get; }

        /// <summary>
        /// A position pointing to the part of the <see cref="CommandContext.FormulaSource"/> where the parsing should
        /// proceed.
        /// </summary>
        public int NextPosition { get; }

        public CommandProcessingResult(Atom atom, int nextPosition)
        {
            Atom = atom;
            NextPosition = nextPosition;
        }
    }

    /// <summary>A command parser interface. Inheritors of this class can perform parsing</summary>
    internal interface ICommandParser
    {
        /// <summary>Parsing of the command arguments.</summary>
        /// <param name="context">The context of the command.</param>
        /// <returns>The parsing result, never <c>null</c>.</returns>
        /// <exception cref="TexParseException">Should be thrown on any error.</exception>
        CommandProcessingResult ProcessCommand(CommandContext context);
    }
}
