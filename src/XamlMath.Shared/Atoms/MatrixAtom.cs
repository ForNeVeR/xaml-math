#if NET462
using XamlMath.Compatibility;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using XamlMath.Boxes;
using SurroundingGap = System.Tuple<double, double>;

namespace XamlMath.Atoms;

/// <summary>An atom representing a tabular arrangement of atoms.</summary>
internal sealed record MatrixAtom : Atom
{
    /// <summary>Used for grouping of align statements into several columns.</summary>
    /// <remarks>
    /// See section "Aligning several equations" of
    /// <a href="https://www.overleaf.com/learn/latex/Aligning_equations_with_amsmath">this article</a> for details.
    /// </remarks>
    private const double AlignGroupLeftPadding = 4;
    public const double DefaultPadding = 0.35;

    public MatrixAtom(
        SourceSpan? source,
        IEnumerable<IEnumerable<Atom?>> cells,
        MatrixCellAlignment matrixCellAlignment,
        double verticalPadding = DefaultPadding,
        double horizontalPadding = DefaultPadding) : base(source)
    {
        MatrixCells = ToImmutableCollection(cells.Select(ToImmutableCollection));
        MatrixCellAlignment = matrixCellAlignment;
        VerticalPadding = verticalPadding;
        HorizontalPadding = horizontalPadding;
    }

    public IReadOnlyCollection<IReadOnlyCollection<Atom?>> MatrixCells { get; }

    public double VerticalPadding { get; }

    public double HorizontalPadding { get; }

    public MatrixCellAlignment MatrixCellAlignment { get; }

    protected override Box CreateBoxCore(TexEnvironment environment)
    {
        Box CreateCell(Atom? atom) => atom is null ? StrutBox.Empty : atom.CreateBox(environment);

        var cells = MatrixCells.Select(row => row.Select(CreateCell).ToArray()).ToArray();
        var columnCount = MatrixCells.Max(row => row.Count);
        var columnWidths = Enumerable.Range(0, columnCount)
                                     .Select(i => cells.Where(row => i < row.Length)
                                     .Max(row => row[i].TotalWidth))
                                     .ToArray();

        var rowsContainer = new VerticalBox();
        foreach (var row in cells)
        {
            var rowContainer = new HorizontalBox();
            var rowHeight = row.Length > 0 ? row.Max(cell => cell.TotalHeight) : 0.0;

            for (var j = 0; j < columnCount; ++j)
            {
                var cell = row[j];
                var columnWidth = columnWidths[j];

                var vFreeSpace = rowHeight - cell.TotalHeight;
                var tbGap = (VerticalPadding + vFreeSpace) / 2;
                var cellContainer = new VerticalBox();
                cellContainer.Add(new StrutBox(0.0, tbGap, 0.0, 0.0));
                cellContainer.Add(cell);
                cellContainer.Add(new StrutBox(0.0, tbGap, 0.0, 0.0));
                cellContainer.Height = cellContainer.TotalHeight;
                cellContainer.Depth = 0;


                var hFreeSpace = columnWidth - cell.TotalWidth;
                var (lGap, rGap) = GetLeftRightGap(hFreeSpace, j);
                rowContainer.Add(new StrutBox(lGap, 0.0, 0.0, 0.0));
                rowContainer.Add(cellContainer);
                rowContainer.Add(new StrutBox(rGap, 0.0, 0.0, 0.0));
            }

            rowsContainer.Add(rowContainer);
        }

        var axis = environment.MathFont.GetAxisHeight(environment.Style);
        var containerHeight = rowsContainer.TotalHeight;
        rowsContainer.Depth = containerHeight / 2 - axis;
        rowsContainer.Height = containerHeight / 2 + axis;

        return rowsContainer;
    }

    private SurroundingGap GetLeftRightGap(double hFreeSpace, int columnIndex)
    {
        var lrPadding = HorizontalPadding / 2;
        return MatrixCellAlignment switch
        {
            MatrixCellAlignment.Aligned => (columnIndex % 2) switch
            {
                0 when columnIndex != 0 => new SurroundingGap(AlignGroupLeftPadding + lrPadding + hFreeSpace, lrPadding),
                0 => new SurroundingGap(lrPadding + hFreeSpace, lrPadding),
                1 => new SurroundingGap(lrPadding, lrPadding + hFreeSpace),
                _ => throw new ArgumentOutOfRangeException()
            },
            MatrixCellAlignment.Left => new SurroundingGap(lrPadding, lrPadding + hFreeSpace),
            MatrixCellAlignment.Center => new SurroundingGap(lrPadding + hFreeSpace / 2, lrPadding + hFreeSpace / 2),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static IReadOnlyCollection<T> ToImmutableCollection<T>(IEnumerable<T> s) => s.ToList().AsReadOnly();
}
