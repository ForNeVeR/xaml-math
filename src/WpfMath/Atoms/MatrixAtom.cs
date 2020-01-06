using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WpfMath.Boxes;
using WpfMath.Utils;

namespace WpfMath.Atoms
{
    /// <summary>An atom representing a tabular arrangement of atoms.</summary>
    internal class MatrixAtom : Atom
    {
        public const double DefaultPadding = 0.35;

        public MatrixAtom(
            SourceSpan source,
            IEnumerable<IEnumerable<Atom>> cells,
            MatrixCellAlignment matrixCellAlignment,
            double verticalPadding = DefaultPadding,
            double horizontalPadding = DefaultPadding) : base(source)
        {
            MatrixCells = new ReadOnlyCollection<ReadOnlyCollection<Atom>>(
                cells.Select(row => new ReadOnlyCollection<Atom>(row.ToList())).ToList());
            MatrixCellAlignment = matrixCellAlignment;
            VerticalPadding = verticalPadding;
            HorizontalPadding = horizontalPadding;
        }

        public ReadOnlyCollection<ReadOnlyCollection<Atom>> MatrixCells { get; }

        public double VerticalPadding { get; }

        public double HorizontalPadding { get; }

        public MatrixCellAlignment MatrixCellAlignment { get; }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var columnCount = MatrixCells.Max(row => row.Count);

            var cells = MatrixCells.Select(row => CreateRowCellBoxes(environment, row).ToList()).ToList();
            var (rowHeights, columnWidths) = CalculateDimensions(cells, columnCount);
            var matrixCellGaps = CalculateCellGaps(cells, columnCount, rowHeights, columnWidths);

            return ApplyCellPaddings(environment, cells, columnCount, matrixCellGaps);
        }

        private IEnumerable<Box> CreateRowCellBoxes(TexEnvironment environment, ReadOnlyCollection<Atom> row) =>
            row.Select(atom => atom == null ? StrutBox.Empty : atom.CreateBox(environment));

        /// <summary>
        /// Calculates the height of each row and the width of each column and returns arrays of those.
        /// </summary>
        /// <returns>A tuple of RowHeights and ColumnWidths arrays.</returns>
        private Tuple<double[], double[]> CalculateDimensions(
            List<List<Box>> matrix,
            int columnCount)
        {
            var rowHeights = new double[matrix.Count];
            var columnWidths = new double[columnCount];
            for (var i = 0; i < matrix.Count; ++i)
            {
                for (var j = 0; j < columnCount; ++j)
                {
                    var cell = matrix[i][j];
                    rowHeights[i] = Math.Max(rowHeights[i], cell.TotalHeight);
                    columnWidths[j] = Math.Max(columnWidths[j], cell.TotalWidth);
                }
            }

            return Tuple.Create(rowHeights, columnWidths);
        }

        private static List<List<CellGaps>> CalculateCellGaps(
            List<List<Box>> matrix,
            int columnCount,
            double[] rowHeights,
            double[] columnWidths)
        {
            var matrixCellGaps = new List<List<CellGaps>>();
            for (var i = 0; i < matrix.Count; ++i)
            {
                var rowGaps = new List<CellGaps>();
                for (var j = 0; j < columnCount; ++j)
                {
                    var cell = matrix[i][j];
                    double cellVShift = rowHeights[i] - cell.TotalHeight;
                    double cellHShift = columnWidths[j] - cell.TotalWidth;

                    rowGaps.Add(new CellGaps {Horizontal = cellHShift / 2, Vertical = cellVShift / 2});
                }

                matrixCellGaps.Add(rowGaps);
            }

            return matrixCellGaps;
        }

        private VerticalBox ApplyCellPaddings(
            TexEnvironment environment,
            IList<List<Box>> matrix,
            int columnCount,
            IList<List<CellGaps>> matrixCellGaps)
        {
            var rowsContainer = new VerticalBox();
            for (var i = 0; i < matrix.Count; ++i)
            {
                var rowContainer = new HorizontalBox();
                for (var j = 0; j < columnCount; ++j)
                {
                    var cell = matrix[i][j];

                    var cellContainer = new VerticalBox();
                    var (topPadding, bottomPadding) = GetTopBottomPadding(i, j);
                    cellContainer.Add(topPadding);
                    cellContainer.Add(cell);
                    cellContainer.Add(bottomPadding);
                    cellContainer.Height = cellContainer.TotalHeight;
                    cellContainer.Depth = 0;

                    var (leftPadding, rightPadding) = GetLeftRightPadding(i, j);
                    if (leftPadding != null) rowContainer.Add(leftPadding);
                    rowContainer.Add(cellContainer);
                    rowContainer.Add(rightPadding);
                }

                rowsContainer.Add(rowContainer);
            }

            var axis = environment.MathFont.GetAxisHeight(environment.Style);
            var containerHeight = rowsContainer.TotalHeight;
            rowsContainer.Depth = containerHeight / 2 - axis;
            rowsContainer.Height = containerHeight / 2 + axis;

            return rowsContainer;

            Tuple<Box, Box> GetTopBottomPadding(int i, int j)
            {
                var value = matrixCellGaps[i][j].Vertical;
                var topBox = new StrutBox(0.0, VerticalPadding / 2 + value, 0.0, VerticalPadding);
                var bottomBox = new StrutBox(0.0, VerticalPadding / 2 + value, 0.0, VerticalPadding);
                return new Tuple<Box, Box>(topBox, bottomBox);
            }

            Tuple<Box, Box> GetLeftRightPadding(int i, int j)
            {
                var value = matrixCellGaps[i][j].Horizontal;
                var leftPadding = MatrixCellAlignment == MatrixCellAlignment.Left ? 0.0 : value;
                var rightPadding = MatrixCellAlignment == MatrixCellAlignment.Left ? value * 2 : value;
                var leftBox = MatrixCellAlignment == MatrixCellAlignment.Left
                    ? null
                    : new StrutBox(HorizontalPadding / 2 + leftPadding, 0.0, 0.0, 0.0);
                var rightBox = new StrutBox(HorizontalPadding / 2 + rightPadding, 0.0, 0.0, 0.0);
                return new Tuple<Box, Box>(leftBox, rightBox);
            }
        }

        private struct CellGaps
        {
            public double Horizontal { get; set; }
            public double Vertical { get; set; }
        }
    }
}
