using System.Collections.Generic;
using System.Linq;
using WpfMath.Atoms;
using WpfMath.Exceptions;

namespace WpfMath.Parsers.Matrices
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

            var cells = ReadMatrixCells(context.Parser, context.Formula, matrixSource, context.Environment);
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

        private List<List<Atom>> ReadMatrixCells(
            TexFormulaParser parser,
            TexFormula formula,
            SourceSpan source,
            ICommandEnvironment parentEnvironment)
        {
            var rowAtoms = new List<List<Atom>> { new List<Atom>() }; // enter first row by default

            // Commands from the environment will add all the finished cells to the matrix body, but the last one should
            // be extracted here.
            var environment = CreateMatrixEnvironment(rowAtoms, parentEnvironment);
            var lastCellAtom = parser.Parse(source, formula.TextStyle, environment).RootAtom;
            if (lastCellAtom != null)
            {
                var lastRow = rowAtoms.LastOrDefault();
                if (lastRow == null)
                    rowAtoms.Add(lastRow = new List<Atom>());

                lastRow.Add(lastCellAtom);
            }

            MakeRectangular(rowAtoms);

            return rowAtoms;
        }

        private ICommandEnvironment CreateMatrixEnvironment(List<List<Atom>> rows, ICommandEnvironment environment)
        {
            var nextRowCommand = new NextRowCommand(rows);
            var commands = new Dictionary<string, ICommandParser>
            {
                ["&"] = new NextCellCommand(rows), // TODO[F]: should work without `\`
                [@"\"] = nextRowCommand,
                ["cr"] = nextRowCommand
            };
            return new NonRecursiveEnvironment(environment.CreateChildEnvironment(), commands);
        }

        private void MakeRectangular(List<List<Atom>> rowAtoms)
        {
            var maxRowLength = rowAtoms.Max(r => r.Count);
            foreach (var row in rowAtoms.Where(r => r.Count < maxRowLength))
            {
                while (row.Count < maxRowLength)
                    row.Add(new NullAtom());
            }
        }
    }
}
