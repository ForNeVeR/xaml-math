 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
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
        public TableAtom(List<List<Atom>> _tblcells, List<Atom> input=null,double tbPad=0.15, double lrpad = 0.5, bool showgridlines=false)
        {
            this.Type = TexAtomType.Ordinary;
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

        public override Box CreateBox(TexEnvironment environment)
        {
            var defaultLineThickness = environment.MathFont.GetDefaultLineThickness(environment.Style);

            #region Suggested Height and width             
            //sum of rows height
            double b = defaultLineThickness * TableCells.Count;
            //sum of table height
            double c = b + (TableCells.Count * 2 * CellBottomTopPadding);

            #endregion

            //Region for adjustment vars
            double maxrowWidth = 0;
            double maxCellWidth = 0;
            double maxCellHeight = 0;
            double maxrowHeight = 0;
            //Table Atom Height should be the sum of its childrens height
            double sigmaHeight = 0;
            //Create a vertical box to hold the rows and their cells
            var resultBox = new VerticalBox
            {
                Height =0,
                Background=environment.Background
            };

            List<Box> tblboxes = new List<Box>();
            //Focus on this sect
            for (int i = 0; i < RowCount; i++)
            {
                #region Row Top
                if (GridLinesVisible == true)
                {
                    var rectbox = new HorizontalRule(environment, CellBottomTopPadding / 4, maxrowWidth, 0);
                    resultBox.Add(rectbox);
                    tblboxes.Add(rectbox);
                }
                if (GridLinesVisible == false)
                {
                    var rectbox = new StrutBox(maxrowWidth, CellBottomTopPadding / 2, 0, 0);
                    resultBox.Add(rectbox);
                    tblboxes.Add(rectbox);
                }
                #endregion

                var rowbox =  new HorizontalBox(StrutBox.Empty,0,TexAlignment.Center);
                double chkrwidth = 0;
                for (int j = 0; j < ColumnCount; j++)
                {
                    #region Cell Left
                    if (GridLinesVisible == true)
                    {
                        rowbox.Add(new VerticalRule(environment, CellLeftRightPadding / 4, maxrowHeight, 0));
                        chkrwidth += CellLeftRightPadding / 4;
                    }
                    if (GridLinesVisible == false)
                    {
                        rowbox.Add(new StrutBox( CellLeftRightPadding / 2, maxrowHeight, 0, 0));
                        chkrwidth += CellLeftRightPadding / 2;
                    }
                    #endregion

                    var rowcolbox = TableCells[i][j] == null ? StrutBox.Empty : TableCells[i][j].CreateBox(environment);
                    maxCellHeight = rowcolbox.Height > maxCellHeight ? rowcolbox.Height : maxCellHeight;
                    maxCellWidth= rowcolbox.Width > maxCellWidth ? rowcolbox.Width : maxCellWidth;
                    rowbox.Add(rowcolbox);
                    chkrwidth += rowcolbox.Width;

                    #region Cell Right
                    if (GridLinesVisible == true)
                    {
                        rowbox.Add(new VerticalRule(environment, CellLeftRightPadding / 4, maxrowHeight, 0));
                        chkrwidth += CellLeftRightPadding / 4;
                    }
                    if (GridLinesVisible == false)
                    {
                        rowbox.Add(new StrutBox(CellLeftRightPadding / 2, maxrowHeight, 0, 0));
                        chkrwidth += CellLeftRightPadding / 2;
                    }
                    #endregion

                }
                maxrowWidth = chkrwidth > maxrowWidth ? chkrwidth : maxrowWidth;
                maxrowHeight = maxCellHeight > maxrowHeight ? maxCellHeight : maxrowHeight;
                rowbox.Height = maxrowHeight;
                rowbox.Width = maxrowWidth;

                resultBox.Add(rowbox);

                #region Row Bottom
                if (GridLinesVisible == true)
                {
                    resultBox.Add(new HorizontalRule(environment, CellBottomTopPadding / 4, maxrowWidth, 0));
                }
                if (GridLinesVisible == false)
                {
                    resultBox.Add(new StrutBox(maxrowWidth, CellBottomTopPadding / 2, 0, 0));
                }
                #endregion

            }

            foreach (var item in tblboxes)
            {
                item.Width = maxrowWidth;
                if (item is HorizontalBox)
                {
                    item.Height = maxrowHeight;
                    
                    foreach (var item1 in ((HorizontalBox)item).Children)
                    {
                        if (item1 is VerticalRule||item1 is StrutBox)
                        {

                        }
                        else
                        {
                            item1.Width = maxCellWidth;
                        }
                    }
                }
            }
               
            //    sigmaHeight += rowatm.Height;
            //    sigmaHeight += 2 * CellBottomTopPadding;

            foreach (var item in resultBox.Children)
            {
                item.Width = maxrowWidth;
            }

            double adjTotheight = (TableCells.Count * maxrowHeight) + (TableCells.Count * 2 * CellBottomTopPadding);
            resultBox.Depth = 0;// + 2 * defaultLineThickness;
            resultBox.Height = c>adjTotheight?c:adjTotheight;
            resultBox.Width = maxrowWidth+2*CellBottomTopPadding;

            //MessageBox.Show("Def: "+defaultLineThickness.ToString() + "ThisTable: " + resultBox.Height.ToString());
            return resultBox;
        }

    }
}
