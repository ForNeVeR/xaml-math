using WpfMath.Atoms;
using WpfMath.Exceptions;

namespace WpfMath.Parsers
{
    /// <summary>A parser for matrix-like constructs.</summary>
    internal class MatrixCommandParser : ICommandParser
    {
        public CommandProcessingResult ProcessCommand(CommandContext context)
        {
            var position = context.ArgumentsStartPosition;
            var source = context.CommandSource;

            if (position == source.Length)
                throw new TexParseException("illegal end!");

            var matrixSource = TexFormulaParser.ReadElement(source, ref position);

            var cells = context.Parser.GetMatrixData(context.Formula, matrixSource);
            var matrix = new MatrixAtom(matrixSource, cells, MatrixCellAlignment.Left);
            var atom = new FencedAtom(
                matrixSource,
                matrix,
                TexFormulaParser.GetDelimiterSymbol("lbrace", null),
                null);
            return new CommandProcessingResult(atom, position);
        }
    }
}
