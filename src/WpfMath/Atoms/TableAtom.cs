 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    //TODO: Make each cell be as wide as the widest in its vertical group 
    //each row to be as wide as the widest row
    /// <summary>
    /// Atom representing a tabular arrangement of Atoms.
    /// </summary>
    internal class TableAtom:Atom
    {
        /// <summary>
        /// Initializes a new <see cref="TableAtom"/> with the specified cell atoms.
        /// </summary>
        /// <param name="input"></param>
        public TableAtom(SourceSpan source,List<List<Atom>> _tblcells, List<Atom> input=null,double tbPad=0.15, double lrpad = 0.5, bool showgridlines=false):base(source)
        {
            TableCells = _tblcells;
            BaseAtoms = input;
            GridLinesVisible = showgridlines;
            CellBottomTopPadding = tbPad;
            CellLeftRightPadding = lrpad;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the table cell <see cref="Atom"/>s contained in this table.
        /// </summary>
        public List<List<Atom>> TableCells
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the rows <see cref="Atom"/>s contained in this table.
        /// </summary>
        public List<Atom> BaseAtoms
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the visibility of the gridlines in the <see cref="TableAtom"/>.
        /// </summary>
        public bool GridLinesVisible
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the top and bottom padding of the cells.
        /// </summary>
        public double CellBottomTopPadding
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the left and right padding of the cells.
        /// </summary>
        public double CellLeftRightPadding
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of rows in this <see cref="TableAtom"/>.
        /// </summary>
        public int RowCount
        {
            get
            {
                if (TableCells == null)
                {
                    return 0;
                }
                else
                {
                    return TableCells.Count;
                }
            }
        }

        /// <summary>
        /// Gets the number of columns in this <see cref="TableAtom"/>.
        /// </summary>
        public int ColumnCount
        {
            get
            {
                if (TableCells == null)
                {
                    return 0;
                }
                else
                {
                    return TableCells[0].Count;
                }
            }
        }

        #endregion

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);



            //Region for adjustment vars
            double maxrowWidth = 0;
            //double maxCellWidth = 0;
            //double maxCellHeight = 0;
            double maxrowHeight = 0;
            //store the max cell height for each row
            List<double> RowsMaxCellHeight = new List<double>();
            for (int i = 0; i < RowCount; i++)
            {
                RowsMaxCellHeight.Add(0);
            }
            //store the max cell width for each column
            List<double> ColumnsMaxCellWidth = new List<double>();
            for (int i = 0; i < ColumnCount; i++)
            {
                ColumnsMaxCellWidth.Add(0);
            }
            //Create a vertical box to hold the rows and their cells
            var resultBox = new VerticalBox
            {
                Height =0,
                Background=Brushes.Red,//environment.Background,
            };

            //List<Box> tblboxes = new List<Box>();
            //Focus on this sect
            for (int i = 0; i < RowCount; i++)
            {
                //row top pad
                var rectbox = new StrutBox(maxrowWidth, CellBottomTopPadding / 2, 0, 0);
                resultBox.Add(rectbox);

                var rowbox =  new HorizontalBox(StrutBox.Empty,0,TexAlignment.Center);
                double chkrwidth = 0;
                for (int j = 0; j < ColumnCount; j++)
                {
                    //cell left pad
                    rowbox.Add(new StrutBox( CellLeftRightPadding / 2, maxrowHeight, 0, 0));
                    chkrwidth += CellLeftRightPadding / 2;
                    //cell box
                    var rowcolbox = TableCells[i][j] == null ? StrutBox.Empty : TableCells[i][j].CreateBox(environment);
                    RowsMaxCellHeight[i] = rowcolbox.TotalHeight > RowsMaxCellHeight[i] ? rowcolbox.TotalHeight : RowsMaxCellHeight[i];
                    ColumnsMaxCellWidth[j]= rowcolbox.TotalWidth > ColumnsMaxCellWidth[j] ? rowcolbox.TotalWidth : ColumnsMaxCellWidth[j];
                    rowbox.Add(rowcolbox);
                    chkrwidth += rowcolbox.TotalWidth;

                    //cell right pad
                    rowbox.Add(new StrutBox(CellLeftRightPadding / 2, maxrowHeight, 0, 0));
                    chkrwidth += CellLeftRightPadding / 2;
                }
                maxrowWidth = chkrwidth > maxrowWidth ? chkrwidth : maxrowWidth;
                maxrowHeight = RowsMaxCellHeight[i] > maxrowHeight ? RowsMaxCellHeight[i] : maxrowHeight;
                rowbox.Height = maxrowHeight;
                rowbox.Width = maxrowWidth;

                resultBox.Add(rowbox);
                //row bottom pad
                resultBox.Add(new StrutBox(maxrowWidth, CellBottomTopPadding / 2, 0, 0));
            }

            int a = 0;
            int b = 0;
            double maxpad = 0;
            for (int i = 0; i < resultBox.Children.Count; i++)
            {
                var tablerowitem = resultBox.Children[i];
                
                if (tablerowitem is HorizontalBox)
                {
                    tablerowitem.Width = maxrowWidth;
                    tablerowitem.Height = maxrowHeight;
                    double paddedrowwidth = 0;
                    for (int j = 0; j < ((HorizontalBox)tablerowitem).Children.Count; j++)
                    {
                        var rowcolitem = ((HorizontalBox)tablerowitem).Children[j];
                        if (rowcolitem is StrutBox)
                        {

                        }
                        else
                        {
                            
                            double cellVShift = RowsMaxCellHeight[a] - rowcolitem.TotalHeight;
                            rowcolitem.Shift = -(cellVShift / 2);
                            double cellHShift = ColumnsMaxCellWidth[b]-rowcolitem.TotalWidth;
                            ((HorizontalBox)tablerowitem).Children[j-1].Width = cellHShift / 2;
                            ((HorizontalBox)tablerowitem).Children[j + 1].Width = cellHShift / 2;
                            if (paddedrowwidth==0)
                            {

                            }
                            paddedrowwidth += rowcolitem.TotalWidth+2*(cellHShift / 2);
                            //item1.Width = maxCellWidth;
                            b++;
                        }
                    }
                    if (paddedrowwidth>maxpad)
                    {
                        maxpad = paddedrowwidth;
                    }
                    b = 0;
                    a++;
                }
            }

            //    sigmaHeight += rowatm.Height;
            //    sigmaHeight += 2 * CellBottomTopPadding;

            foreach (var item in resultBox.Children)
            {
                item.Width = maxrowWidth;
            }

            //(RowCount * maxrowHeight) 
            double adjustedTotalHeight = RowsMaxCellHeight.Sum()+ (RowCount * 2 * CellBottomTopPadding);
            resultBox.Depth = 0;// + 2 * defaultLineThickness;
            resultBox.Height = adjustedTotalHeight;
            resultBox.Width = maxpad;// +2*CellBottomTopPadding;
            resultBox.Shift = axis>= resultBox.TotalHeight? - (axis- resultBox.TotalHeight)/2: ( resultBox.TotalHeight-axis) / 2;
            var testbox = new HorizontalBox();
            testbox.Add(new StrutBox(CellLeftRightPadding/8, 0, 0, 0));
            testbox.Add(resultBox);
            testbox.Height = resultBox.TotalHeight-resultBox.Shift + (CellBottomTopPadding / 8);
            //testbox.Shift = (CellBottomTopPadding / 8);
            testbox.Add(new StrutBox(CellLeftRightPadding/8, 0, 0, 0));
            //MessageBox.Show("Def: "+defaultLineThickness.ToString() + "ThisTable: " + resultBox.Height.ToString());
            return testbox;
        }

    }
}
