using WpfMath.Atoms;
using WpfMath.Exceptions;

namespace WpfMath.Parsers
{
    /// <summary>A parser for matrix-like constructs.</summary>
    internal class MatrixCommandParser : ICommandParser
    {
        private readonly string _leftDelimiterSymbolName;
        private readonly string _rightDelimiterSymbolName;
        private readonly MatrixCellAlignment _cellAlignment;

        public MatrixCommandParser(
            string leftDelimiterSymbolName,
            string rightDelimiterSymbolName,
            MatrixCellAlignment cellAlignment)
        {
            _leftDelimiterSymbolName = leftDelimiterSymbolName;
            _rightDelimiterSymbolName = rightDelimiterSymbolName;
            _cellAlignment = cellAlignment;
        }

        public CommandProcessingResult ProcessCommand(CommandContext context)
        {
            var position = context.ArgumentsStartPosition;
            var source = context.CommandSource;

            if (position == source.Length)
                throw new TexParseException("illegal end!");

            var matrixSource = TexFormulaParser.ReadElement(source, ref position);

            var cells = context.Parser.GetMatrixData(context.Formula, matrixSource);
            var matrix = new MatrixAtom(matrixSource, cells, _cellAlignment);

            SymbolAtom GetDelimiter(string name) =>
                name == null
                    ? null
                    : TexFormulaParser.GetDelimiterSymbol(name, null) ??
                      throw new TexParseException($"The delimiter {name} could not be found");

            var leftDelimiter = GetDelimiter(_leftDelimiterSymbolName);
            var rightDelimiter = GetDelimiter(_rightDelimiterSymbolName);

            var atom = leftDelimiter == null && rightDelimiter == null
                ? (Atom) matrix
                : new FencedAtom(
                    matrixSource,
                    matrix,
                    leftDelimiter,
                    rightDelimiter);
            return new CommandProcessingResult(atom, position);
        }
    }
}
