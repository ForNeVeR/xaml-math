using System.Collections.Generic;
using System.Linq;
using XamlMath.Atoms;
using XamlMath.Exceptions;

namespace XamlMath.Parsers.Matrices;

/// <summary>A parser for matrix-like constructs.</summary>
internal sealed class MatrixCommandParser : ICommandParser, IEnvironmentParser
{
    internal static readonly MatrixCommandParser Align = new(null, null, MatrixCellAlignment.Aligned);
    internal static readonly MatrixCommandParser Cases = new("lbrace", null, MatrixCellAlignment.Left);
    internal static readonly MatrixCommandParser Matrix = new(null, null, MatrixCellAlignment.Center);
    internal static readonly MatrixCommandParser PMatrix = new("lbrack", "rbrack", MatrixCellAlignment.Center);

    private readonly string? _leftDelimiterSymbolName;
    private readonly string? _rightDelimiterSymbolName;
    private readonly MatrixCellAlignment _cellAlignment;

    private MatrixCommandParser(
        string? leftDelimiterSymbolName,
        string? rightDelimiterSymbolName,
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

        var cellsSource = TexFormulaParser.ReadElement(source, ref position);
        var matrixSource = context.CommandSource.Segment(
            context.CommandNameStartPosition,
            position - context.CommandNameStartPosition);

        var envContext = new EnvironmentContext(
            context.Parser,
            context.Formula,
            context.Environment,
            matrixSource,
            cellsSource);
        var result = ProcessEnvironment(envContext);
        return new CommandProcessingResult(result.Atom, position, result.AppendMode);
    }

    public EnvironmentProcessingResult ProcessEnvironment(EnvironmentContext context)
    {
        var cellsSource = context.EnvironmentBodySource;
        var matrixSource = context.EnvironmentSource;

        var cells = ReadMatrixCells(context.Parser, context.Formula, cellsSource, context.Environment);
        var matrix = new MatrixAtom(matrixSource, cells, _cellAlignment);

        SymbolAtom? GetDelimiter(string? name) =>
            name == null
                ? null
                : TexFormulaParser.GetDelimiterSymbol(name, null) ??
                  throw new TexParseException($"The delimiter {name} could not be found");

        SymbolAtom? leftDelimiter = GetDelimiter(_leftDelimiterSymbolName);
        SymbolAtom? rightDelimiter = GetDelimiter(_rightDelimiterSymbolName);

        var atom = leftDelimiter == null && rightDelimiter == null
            ? (Atom)matrix
            : new FencedAtom(
                matrixSource,
                matrix,
                leftDelimiter,
                rightDelimiter);
        return new EnvironmentProcessingResult(atom);
    }

    private static List<List<Atom>> ReadMatrixCells(
        TexFormulaParser parser,
        TexFormula formula,
        SourceSpan source,
        ICommandEnvironment parentEnvironment)
    {
        var rows = new List<List<Atom>> { new List<Atom>() }; // enter first row by default

        // Commands from the environment will add all the finished cells to the matrix body, but the last one should
        // be extracted here.
        var environment = new MatrixInternalEnvironment(parentEnvironment, rows);
        var lastCellAtom = parser.Parse(source, formula.TextStyle, environment).RootAtom;
        if (lastCellAtom != null)
        {
            var lastRow = rows.LastOrDefault();
            if (lastRow == null)
                rows.Add(lastRow = new List<Atom>());

            lastRow.Add(lastCellAtom);
        }

        MakeRectangular(rows);

        return rows;
    }

    private static void MakeRectangular(List<List<Atom>> rowAtoms)
    {
        var maxRowLength = rowAtoms.Max(r => r.Count);
        foreach (var row in rowAtoms.Where(r => r.Count < maxRowLength))
        {
            while (row.Count < maxRowLength)
                row.Add(new NullAtom());
        }
    }
}
