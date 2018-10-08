using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfMath.Boxes;
using WpfMath.Exceptions;

namespace WpfMath.Atoms
{
    /// <summary>
    /// Represents a 2D array of atoms.
    /// </summary>
    class ArrayofAtoms:Atom
    {
        /// <summary>
        /// Initializes a new <see cref="ArrayofAtoms"/> with the specified cells and alignment options.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="arraycells"></param>
        /// <param name="cellsValignment"></param>
        /// <param name="cellsHAlignment"></param>
        /// <param name="type"></param>
        public ArrayofAtoms(SourceSpan source, List<List<Atom>> arraycells, List<List<VerticalAlignment>> cellsValignment = null, List<List<HorizontalAlignment>> cellsHAlignment = null, TexAtomType type = TexAtomType.Ordinary) : base(source, type)
        {
            if (AreColumnsEqual(arraycells, out int columns))
            {
                ColumnCount = columns;
                this.ArrayCells = arraycells;
                List<double> rowspadding = new List<double>();

                for (int i = 0; i < RowCount; i++)
                {
                    rowspadding.Add(0.35);
                }

                CellBottomTopPadding = rowspadding;
                CellsLeftRightPadding = 0.21;

                if (cellsHAlignment == null)
                {
                    //use default(all centred)
                    List<List<HorizontalAlignment>> defaultCellsHAlign = new List<List<HorizontalAlignment>>();

                    for (int i = 0; i < RowCount; i++)
                    {
                        List<HorizontalAlignment> rowcellsHAlign = new List<HorizontalAlignment>();

                        for (int j = 0; j < ColumnCount; j++)
                        {
                            rowcellsHAlign.Add(HorizontalAlignment.Center);
                        }
                        defaultCellsHAlign.Add(rowcellsHAlign);
                    }
                    this.CellsHorizontalAlignment = defaultCellsHAlign;
                }
                else
                {
                    this.CellsHorizontalAlignment = cellsHAlignment;
                }
            }
            else
            {
                throw new TexParseException("The column numbers are not equal.");
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets the cell <see cref="Atom"/>s contained in this array of atoms.
        /// </summary>
        public List<List<Atom>> ArrayCells { get; private set; }

        /// <summary>
        /// Gets or sets the top and bottom padding of the cells.
        /// </summary>
        public List<double> CellBottomTopPadding { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="HorizontalAlignment"/> of each cell in this <see cref="TableAtom"/>.
        /// </summary>
        public List<List<HorizontalAlignment>> CellsHorizontalAlignment { get; private set; }

        /// <summary>
        /// Gets or sets the left and right padding of the cells.
        /// </summary>
        public double CellsLeftRightPadding { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="HorizontalAlignment"/> of each cell in this <see cref="TableAtom"/>.
        /// </summary>
        public List<List<VerticalAlignment>> CellsVerticalAlignment { get; private set; }

        /// <summary>
        /// Gets the number of columns in this <see cref="ArrayofAtoms"/>.
        /// </summary>
        public int ColumnCount { get; }

        /// <summary>
        /// Gets the number of rows in this <see cref="ArrayofAtoms"/>.
        /// </summary>
        public int RowCount
        {
            get
            {
                if (ArrayCells == null)
                {
                    return 0;
                }
                else
                {
                    return ArrayCells.Count;
                }
            }
        }

        #endregion

        /// <summary>
        /// Checks if the columns in the specified rows are equal. 
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private bool AreColumnsEqual(List<List<Atom>> cells, out int columns)
        {
            List<byte> rowColumnsCount = new List<byte>();
            byte colscount = 0;
            for (int i = 0; i < cells.Count; i++)
            {
                var rowcells = cells[i];
                rowColumnsCount.Add(0);
                for (int j = 0; j < rowcells.Count; j++)
                {
                    if (rowcells[j] is MulticolumnAtom)
                    {
                        rowColumnsCount[i] += (((MulticolumnAtom)rowcells[j]).ColumnSpan);
                        ((MulticolumnAtom)rowcells[j]).Column = (byte)j;
                        //j += (((MulticolumnAtom)rowcells[j]).ColumnSpan)-1;
                    }
                    //else if (rowcells[j] is HLineAtom)
                    //{

                    //}
                    else
                    {
                        rowColumnsCount[i]++;
                    }
                }
                colscount = rowColumnsCount[i];
            }

            columns = rowColumnsCount[0];
            int colspassed = 0;
            foreach (var item in rowColumnsCount)
            {
                if (item == colscount)
                {
                    colspassed++;
                }
            }

            return colspassed == rowColumnsCount.Count;
        }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);

            //stores the max cell height for each row
            var RowsMaxCellHeight = new List<double>(RowCount);
            for (int i = 0; i < RowCount; i++)
            {
                RowsMaxCellHeight.Add(0);
            }

            //stores the max cell width for each column
            var ColumnsMaxCellWidth = new List<double>(ColumnCount);
            for (int i = 0; i < ColumnCount; i++)
            {
                ColumnsMaxCellWidth.Add(0);
            }

            //Stage 1---> Get the width of each box from the atoms for adjusting multi-column atoms and other atom types.

            for (int i = 0; i < RowCount; i++)
            {
                //double currentRowWidth = 0;
                for (int j = 0; j < ColumnCount; j++)
                {
                    var rowcellbox = ArrayCells[i][j] == null ? StrutBox.Empty : ArrayCells[i][j].CreateBox(environment);

                    //currentRowWidth += rowcellbox.TotalWidth;

                    if (rowcellbox.TotalHeight > RowsMaxCellHeight[i])
                    {
                        RowsMaxCellHeight[i] = rowcellbox.TotalHeight;
                    }

                    if (ArrayCells[i][j] is MulticolumnAtom)
                    {
                        //ColumnsMaxCellWidth[j] = ((MulticolumnAtom)ArrayCells[i][j]).Width > ColumnsMaxCellWidth[j] ? ((MulticolumnAtom)ArrayCells[i][j]).Width : ColumnsMaxCellWidth[j];

                        j += ((MulticolumnAtom)ArrayCells[i][j]).ColumnSpan - 1;

                    }
                    else
                    {
                        ColumnsMaxCellWidth[j] = rowcellbox.TotalWidth > ColumnsMaxCellWidth[j] ? rowcellbox.TotalWidth : ColumnsMaxCellWidth[j];
                    }
                }

                //if (currentRowWidth>maxrowWidth)
                //{
                //    maxrowWidth = currentRowWidth;
                //}
            }

            //Stage 2---> Create the atoms from each cell, adding it into its row(a horizontal box) and adding the row into a vertical box(the result box).

            //Create a vertical box to hold the rows and their cells
            var resultBox = new VerticalBox();

            //NB: Each cell should have a vertical box for implementing the padding bewtween successive rows.
            double sigmaHeight = 0;
            for (int i = 0; i < RowCount; i++)
            {
                var rowbox = new HorizontalBox() { Tag = $"Row:{i}", };

                for (int j = 0; j < ColumnCount; j++)
                {
                    if (ArrayCells[i][j] is MulticolumnAtom)
                    {
                        //Get the optimum width that should be occupied by this multicolumn atom
                        double multicolumnWidthSpan = 0;
                        int start = ((MulticolumnAtom)ArrayCells[i][j]).Column;
                        int end = ((MulticolumnAtom)ArrayCells[i][j]).Column + ((MulticolumnAtom)ArrayCells[i][j]).ColumnSpan;

                        for (int k = start; k < end; k++)
                        {
                            multicolumnWidthSpan += ColumnsMaxCellWidth[k];
                        }
                        multicolumnWidthSpan += (end - start) * (CellsLeftRightPadding / 2);

                        ((MulticolumnAtom)ArrayCells[i][j]).Width = multicolumnWidthSpan;

                        var rowMulticolumnBox = ArrayCells[i][j].CreateBox(environment);

                        //Get the top and bottom padding for this cell
                        var cellTopPadding = ((RowsMaxCellHeight[i] - rowMulticolumnBox.TotalHeight) / 2) + (CellBottomTopPadding[i] / 2);
                        var cellBottomPadding = ((RowsMaxCellHeight[i] - rowMulticolumnBox.TotalHeight) / 2) + (CellBottomTopPadding[i] / 2);
                        //if (i>0)
                        //{
                        //    cellTopPadding = ((RowsMaxCellHeight[i] - rowMulticolumnBox.TotalHeight) / 2) + CellBottomTopPadding[i-1]/2;
                        //}
                        //if (i <RowCount)
                        //{
                        //    cellBottomPadding = ((RowsMaxCellHeight[i] - rowMulticolumnBox.TotalHeight) / 2) + CellBottomTopPadding[i]/2;
                        //}

                        //---|->Insert a switch case here to adjust the vertical alignment of this cell 

                        var topPadBox = new StrutBox(0, cellTopPadding, 0, 0) { Tag = $"CellTopPad{i}:{j}", };
                        var bottomPadBox = new StrutBox(0, cellBottomPadding, 0, 0) { Tag = $"CellBottomPad{i}:{j}", };

                        var cellVerticalAlignmentBox = new VerticalBox();
                        cellVerticalAlignmentBox.Add(topPadBox);
                        cellVerticalAlignmentBox.Add(rowMulticolumnBox);
                        cellVerticalAlignmentBox.Add(bottomPadBox);

                        cellVerticalAlignmentBox.Height = topPadBox.Height + rowMulticolumnBox.TotalHeight + bottomPadBox.Height;
                        cellVerticalAlignmentBox.Depth = 0;

                        rowbox.Add(cellVerticalAlignmentBox);
                        j += ((MulticolumnAtom)ArrayCells[i][j]).ColumnSpan - 1;
                    }
                    else
                    {
                        //create the main cell
                        var rowColumnBox = ArrayCells[i][j] == null ? StrutBox.Empty : ArrayCells[i][j].CreateBox(environment);

                        //calculate the left and right padding for this cell
                        var cellleftPadding = 0d;
                        var cellrightPadding = 0d;

                        switch (CellsHorizontalAlignment[i][j])
                        {
                            case HorizontalAlignment.Center:
                                {
                                    cellleftPadding = (CellsLeftRightPadding / 2) + ((ColumnsMaxCellWidth[j] - rowColumnBox.TotalWidth) / 2);
                                    cellrightPadding = (CellsLeftRightPadding / 2) + ((ColumnsMaxCellWidth[j] - rowColumnBox.TotalWidth) / 2);
                                    break;
                                }
                            case HorizontalAlignment.Left:
                                {
                                    cellleftPadding = (CellsLeftRightPadding / 2);
                                    cellrightPadding = (CellsLeftRightPadding / 2) + ((ColumnsMaxCellWidth[j] - rowColumnBox.TotalWidth));
                                    break;
                                }
                            case HorizontalAlignment.Right:
                                {
                                    cellleftPadding = (CellsLeftRightPadding / 2) + ((ColumnsMaxCellWidth[j] - rowColumnBox.TotalWidth));
                                    cellrightPadding = (CellsLeftRightPadding / 2);
                                    break;
                                }
                            default:
                                break;
                        }

                        //create the left padding cell and add it to the current rowbox
                        var cellleftpad = new StrutBox(cellleftPadding, 0, 0, 0) { Tag = $"CellLeftPad{i}:{j}", };

                        rowbox.Add(cellleftpad);

                        //calculate the top and bottom padding for this cell
                        var cellTopPadding = 0d;
                        var cellBottomPadding = 0d;

                        switch (CellsVerticalAlignment[i][j])
                        {
                            case VerticalAlignment.Bottom:
                                {
                                    //top pad should take the current row height difference and bottom pad none.
                                    cellTopPadding = ((RowsMaxCellHeight[i] - rowColumnBox.TotalHeight)) + (CellBottomTopPadding[i] / 2);
                                    cellBottomPadding = (CellBottomTopPadding[i] / 2);
                                    break;
                                }
                            case VerticalAlignment.Center:
                                {
                                    //the current row height difference should be equally shared between the bottom pad and top pad.
                                    cellTopPadding = ((RowsMaxCellHeight[i] - rowColumnBox.TotalHeight) / 2) + (CellBottomTopPadding[i] / 2);
                                    cellBottomPadding = ((RowsMaxCellHeight[i] - rowColumnBox.TotalHeight) / 2) + (CellBottomTopPadding[i] / 2);
                                    break;
                                }
                            case VerticalAlignment.Top:
                                {
                                    //bottom pad should take the current row height difference and top pad none.
                                    cellTopPadding = (CellBottomTopPadding[i] / 2);
                                    cellBottomPadding = ((RowsMaxCellHeight[i] - rowColumnBox.TotalHeight)) + (CellBottomTopPadding[i] / 2);
                                    break;
                                }
                            default:
                                break;
                        }

                        var topPadBox = new StrutBox(0, cellTopPadding, 0, 0) { Tag = $"CellTopPad{i}:{j}", };
                        var bottomPadBox = new StrutBox(0, cellBottomPadding, 0, 0) { Tag = $"CellBottomPad{i}:{j}", };

                        var cellVerticalAlignmentBox = new VerticalBox() { Shift = 0, Depth = 0, };
                        cellVerticalAlignmentBox.Add(topPadBox);

                        cellVerticalAlignmentBox.Add(rowColumnBox);

                        cellVerticalAlignmentBox.Add(bottomPadBox);

                        cellVerticalAlignmentBox.Height = topPadBox.Height + rowColumnBox.TotalHeight + bottomPadBox.Height;
                        cellVerticalAlignmentBox.Depth = 0;

                        rowbox.Add(cellVerticalAlignmentBox);

                        //add this cell's right pad
                        var cellrightpad = new StrutBox(cellrightPadding, 0, 0, 0) { Tag = $"CellRightPad{i}:{j}", };

                        rowbox.Add(cellrightpad);
                    }
                }

                sigmaHeight += rowbox.Height;
                resultBox.Add(rowbox);
            }

            resultBox.Height = sigmaHeight;
            resultBox.Depth = 0;
            var enviroYDiff = axis >= resultBox.TotalHeight ? -(axis - resultBox.TotalHeight) / 2 : (resultBox.TotalHeight / 2 - axis);
            enviroYDiff = (resultBox.TotalHeight / 2 - axis);
            resultBox.Depth = enviroYDiff;
            resultBox.Height -= enviroYDiff;

            return resultBox;
        }
    }
}
