using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    /// <summary>
    /// An atom representing a tabular arrangement of atoms.
    /// </summary>
    internal class TableAtom:Atom
    {
        /// <summary>
        /// Initializes a new <see cref="TableAtom"/> with the specified cell atoms.
        /// </summary>
        public TableAtom(SourceSpan source,List<List<Atom>> _tblcells,VerticalAlignment cellValignment=VerticalAlignment.Center, HorizontalAlignment cellHAlignment = HorizontalAlignment.Center, double tbPad=0.35, double lrpad = 0.35):base(source)
        {
            TableCells = _tblcells;
            this.CellHorizontalAlignment = cellHAlignment;
            this.CellVerticalAlignment = cellValignment;
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
                if (TableCells == null||TableCells[0]==null)
                {
                    return 0;
                }
                else
                {
                    return TableCells[0].Count;
                }
            }
        }

        public VerticalAlignment CellVerticalAlignment { get; private set; }
        public HorizontalAlignment CellHorizontalAlignment { get; private set; }
        #endregion

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);

            //Region for adjustment vars
            double maxrowWidth = 0;
            
            //stores the max cell height for each row
            var RowsMaxCellHeight = new List<double>();
            for (int i = 0; i < RowCount; i++)
            {
                RowsMaxCellHeight.Add(0);
            }
            //stores the max cell width for each column
            var ColumnsMaxCellWidth = new List<double>();
            for (int i = 0; i < ColumnCount; i++)
            {
                ColumnsMaxCellWidth.Add(0);
            }
            //Create a vertical box to hold the rows and their cells
            var resultBox = new VerticalBox();

            for (int i = 0; i < RowCount; i++)
            {
                //row top pad
                resultBox.Add(new StrutBox(maxrowWidth, CellBottomTopPadding / 2, 0, 0));

                var rowbox =  new HorizontalBox() {Tag= $"Row:{i}",};
                
                for (int j = 0; j < ColumnCount; j++)
                {
                    double maxrowHeight = 0;
                    //cell left pad
                    var cellleftpad = new StrutBox(CellLeftRightPadding / 2, 0, 0, 0)
                    {
                        Tag = $"CellLeftPad{i}:{j}",
                        Shift =0,
                    };

                    //cell right pad
                    var cellrightpad = new StrutBox(CellLeftRightPadding / 2, 0, 0, 0)
                    {
                        Tag = $"CellRightPad{i}:{j}",
                        Shift = 0,
                    };

                    //cell box
                    var rowcellbox = TableCells[i][j] == null ? StrutBox.Empty : TableCells[i][j].CreateBox(environment);
                    ColumnsMaxCellWidth[j] = rowcellbox.TotalWidth > ColumnsMaxCellWidth[j] ? rowcellbox.TotalWidth : ColumnsMaxCellWidth[j];
                    rowcellbox.Tag = "innercell";
                    //cell box holder
                    var rowcolbox = new VerticalBox(){Tag=$"Cell{i}:{j}",ShowBounds=false};

                    var celltoppad = new StrutBox(rowcellbox.TotalWidth, CellBottomTopPadding / 2, 0, 0){Tag = $"CellTopPad{i}:{j}",};
                    var cellbottompad = new StrutBox(rowcellbox.TotalWidth, CellBottomTopPadding / 2, 0, 0){Tag = $"CellBottomPad{i}:{j}",};
                    rowcolbox.Add(celltoppad);
                    rowcolbox.Add(rowcellbox);
                    rowcolbox.Add(cellbottompad);

                    //maxrowHeight += rowcolbox.TotalHeight;
                    maxrowHeight += rowcellbox.TotalHeight + CellBottomTopPadding;
                    rowcolbox.Width = rowcellbox.TotalWidth;
                    //rowcolbox.Height = maxrowHeight;

                    rowbox.Add(cellleftpad);
                    rowbox.Add(rowcolbox);
                    rowbox.Add(cellrightpad);

                    RowsMaxCellHeight[i] = maxrowHeight > RowsMaxCellHeight[i] ? maxrowHeight : RowsMaxCellHeight[i];

                }
                
                rowbox.Shift = 0;
                resultBox.Add(rowbox);
                //row bottom pad
                resultBox.Add(new StrutBox(maxrowWidth, CellBottomTopPadding / 2, 0, 0));
            }

            int a = 0;
            int b = 0;
            //1->Left and right, 2->Top and Bottom
            //create the item below to hold the left-right gaps(Tuple.Item1) and top-bottom gaps (Tuple.Item2) for the rows
            List<List<Tuple<double, double>>> TableCellGaps = new List<List<Tuple<double, double>>>();
            for (int i = 0; i < resultBox.Children.Count; i++)
            {
                var tablerowitem = resultBox.Children[i];
                List<Tuple<double, double>> RowGaps = new List<Tuple<double, double>>();

                if (tablerowitem is HorizontalBox&&tablerowitem.Tag.ToString()==$"Row:{a}")
                {
                    for (int j = 0; j < ((HorizontalBox)tablerowitem).Children.Count; j++)
                    {
                        var rowcolitem = ((HorizontalBox)tablerowitem).Children[j];
                        if (rowcolitem is StrutBox)
                        {
                            rowcolitem.Height = RowsMaxCellHeight[a];
                        }
                        else if(rowcolitem is VerticalBox && rowcolitem.Tag.ToString() == $"Cell{a}:{b}")
                        {
                            double cellVShift = RowsMaxCellHeight[a] - rowcolitem.TotalHeight;
                                                        
                            double cellHShift = ColumnsMaxCellWidth[b]-rowcolitem.TotalWidth;
                            ((HorizontalBox)tablerowitem).Children[j - 1].Shift = rowcolitem.Depth;// + (cellVShift / 2);//.Width += cellHShift / 2;
                            ((HorizontalBox)tablerowitem).Children[j + 1].Shift = rowcolitem.Depth;// +(cellVShift / 2);// Width += cellHShift / 2;
                            //rowcolitem.Shift =  cellVShift/2;
                            RowGaps.Add( new Tuple<double, double> (cellHShift/2, cellVShift/2));

                            b++;
                        }
                    }

                    b = 0;
                    a++;
                    TableCellGaps.Add(RowGaps);
                }

            }

            a = 0;
            b = 0;
            for (int i = 0; i < resultBox.Children.Count; i++)
            {
                var tablerowitem = resultBox.Children[i];

                if (tablerowitem is HorizontalBox && tablerowitem.Tag.ToString() == $"Row:{a}")
                {
                    double rowwidth = 0;
                    for (int j = 0; j < ((HorizontalBox)tablerowitem).Children.Count; j++)
                    {
                        var currowcolitem = ((HorizontalBox)tablerowitem).Children[j];
                        var prevrowcolitem = j > 0 ? ((HorizontalBox)tablerowitem).Children[j - 1] : ((HorizontalBox)tablerowitem).Children[j];
                        var nextrowcolitem = j < ((HorizontalBox)tablerowitem).Children.Count-1 ? ((HorizontalBox)tablerowitem).Children[j + 1] : ((HorizontalBox)tablerowitem).Children[j];
                        
                        if (currowcolitem is VerticalBox&& Regex.IsMatch(currowcolitem.Tag.ToString(), @"Cell[0-9]+:[0-9]+"))
                        {
                            rowwidth += currowcolitem.TotalWidth;
                            var leftstructboxtag = $"CellLeftPad{a}:{b}";
                            var rightstructboxtag = $"CellRightPad{a}:{b}";

                            switch (CellHorizontalAlignment)
                            {
                                case HorizontalAlignment.Left:
                                    {
                                        if (prevrowcolitem is StrutBox && prevrowcolitem.Tag.ToString() == leftstructboxtag)
                                        {
                                            //prevrowcolitem.Width += TableCellGaps[a][b].Item1;
                                            rowwidth += prevrowcolitem.TotalWidth;
                                        }
                                        if (nextrowcolitem is StrutBox && nextrowcolitem.Tag.ToString() == rightstructboxtag)
                                        {
                                            nextrowcolitem.Width +=2* TableCellGaps[a][b].Item1;
                                            rowwidth += nextrowcolitem.TotalWidth;
                                        }
                                        break;
                                    }

                                case HorizontalAlignment.Right:
                                    {
                                        if (prevrowcolitem is StrutBox && prevrowcolitem.Tag.ToString() == leftstructboxtag)
                                        {
                                            prevrowcolitem.Width +=2* TableCellGaps[a][b].Item1;
                                            rowwidth += prevrowcolitem.TotalWidth;
                                        }
                                        if (nextrowcolitem is StrutBox && nextrowcolitem.Tag.ToString() == rightstructboxtag)
                                        {
                                            //nextrowcolitem.Width += TableCellGaps[a][b].Item1;
                                            rowwidth += nextrowcolitem.TotalWidth;
                                        }
                                        break;
                                    }

                                case HorizontalAlignment.Center:
                                    {
                                        if (prevrowcolitem is StrutBox && prevrowcolitem.Tag.ToString() == leftstructboxtag)
                                        {
                                            prevrowcolitem.Width += TableCellGaps[a][b].Item1;
                                            rowwidth += prevrowcolitem.TotalWidth;
                                        }
                                        if (nextrowcolitem is StrutBox && nextrowcolitem.Tag.ToString() == rightstructboxtag)
                                        {
                                            nextrowcolitem.Width += TableCellGaps[a][b].Item1;
                                            rowwidth += nextrowcolitem.TotalWidth;
                                        }
                                        break;
                                    }
                                    
                                case HorizontalAlignment.Stretch:
                                    {
                                        if (prevrowcolitem is StrutBox && prevrowcolitem.Tag.ToString() == leftstructboxtag)
                                        {
                                            prevrowcolitem.Width = 0;
                                            prevrowcolitem.Italic = 0;
                                            rowwidth += prevrowcolitem.TotalWidth;
                                        }
                                        if (nextrowcolitem is StrutBox && nextrowcolitem.Tag.ToString() == rightstructboxtag)
                                        {
                                            nextrowcolitem.Width = 0;
                                            nextrowcolitem.Italic = 0;
                                            rowwidth += nextrowcolitem.TotalWidth;
                                        }
                                        break;
                                    }

                                default:
                                    break;
                            }

                            double cellheight = 0;
                            //check the vertical cell gap size and increase appropriately
                            for (int k = 0; k < ((VerticalBox)currowcolitem).Children.Count; k++)
                            {
                                var curcellitem = ((VerticalBox)currowcolitem).Children[k];
                                var prevcellitem =k>0? ((VerticalBox)currowcolitem).Children[k-1]:((VerticalBox)currowcolitem).Children[k];
                                var nextcellitem =k<(((VerticalBox)currowcolitem).Children.Count -1)? ((VerticalBox)currowcolitem).Children[k+1]:((VerticalBox)currowcolitem).Children[k];
                                
                                if (curcellitem.Tag.ToString() == "innercell" )
                                {
                                    cellheight += curcellitem.TotalHeight;
                                    var topstructboxtag = $"CellTopPad{a}:{b}";
                                    var bottomstructboxtag = $"CellBottomPad{a}:{b}";

                                    switch (CellVerticalAlignment)
                                    {
                                        case VerticalAlignment.Bottom:
                                            {
                                                if (prevcellitem.Tag.ToString() == topstructboxtag)
                                                {
                                                    prevcellitem.Height +=2* TableCellGaps[a][b].Item2;
                                                    //prevcellitem.Background = Brushes.Aquamarine;
                                                    cellheight += prevcellitem.TotalHeight;
                                                    if (prevcellitem.Height > (currowcolitem.Height / 2))
                                                    {

                                                    }
                                                }
                                                if (nextcellitem.Tag.ToString() == bottomstructboxtag)
                                                {
                                                    //nextcellitem.Height += TableCellGaps[a][b].Item2;
                                                    //nextcellitem.Background = Brushes.BurlyWood;
                                                    cellheight += nextcellitem.TotalHeight;
                                                }
                                                break;
                                            }
                                        case VerticalAlignment.Center:
                                            {
                                                if (prevcellitem.Tag.ToString() == topstructboxtag)
                                                {
                                                    prevcellitem.Height += TableCellGaps[a][b].Item2;
                                                    //prevcellitem.Background = Brushes.Aquamarine;
                                                    cellheight += prevcellitem.TotalHeight;
                                                    if (prevcellitem.Height > (currowcolitem.Height / 2))
                                                    {

                                                    }
                                                }
                                                if (nextcellitem.Tag.ToString() == bottomstructboxtag)
                                                {
                                                    nextcellitem.Height += TableCellGaps[a][b].Item2;
                                                    //nextcellitem.Background = Brushes.BurlyWood;
                                                    cellheight += nextcellitem.TotalHeight;
                                                }
                                                break;
                                            }
                                        case VerticalAlignment.Stretch:
                                            {
                                                if (prevcellitem.Tag.ToString() == topstructboxtag)
                                                {
                                                    prevcellitem.Height = 0;
                                                    prevcellitem.Depth = 0;
                                                    //prevcellitem.Background = Brushes.Aquamarine;
                                                    cellheight += prevcellitem.TotalHeight;
                                                    if (prevcellitem.Height > (currowcolitem.Height / 2))
                                                    {
    
                                                    }
                                                }
                                                if (nextcellitem.Tag.ToString() == bottomstructboxtag)
                                                {
                                                    nextcellitem.Height = 0;
                                                    nextcellitem.Depth = 0;
                                                    //nextcellitem.Background = Brushes.BurlyWood;
                                                    cellheight += nextcellitem.TotalHeight;
                                                }
                                                break;
                                            }
                                        case VerticalAlignment.Top:
                                            {
                                                if (prevcellitem.Tag.ToString() == topstructboxtag)
                                                {
                                                    //prevcellitem.Height += TableCellGaps[a][b].Item2;
                                                    //prevcellitem.Background = Brushes.Aquamarine;
                                                    cellheight += prevcellitem.TotalHeight;
                                                    if (prevcellitem.Height > (currowcolitem.Height / 2))
                                                    {
                                                    }
                                                }
                                                if (nextcellitem.Tag.ToString() == bottomstructboxtag)
                                                {
                                                    nextcellitem.Height +=2* TableCellGaps[a][b].Item2;
                                                    //nextcellitem.Background = Brushes.BurlyWood;
                                                    cellheight += nextcellitem.TotalHeight;
                                                }
                                                break;
                                            }
                                        default:
                                            break;
                                    }
                                    
                                    if (prevrowcolitem is StrutBox && prevrowcolitem.Tag.ToString() == leftstructboxtag)
                                    {
                                        prevrowcolitem.Shift += TableCellGaps[a][b].Item2;
                                    }
                                    if (nextrowcolitem is StrutBox && nextrowcolitem.Tag.ToString() == rightstructboxtag)
                                    {
                                        nextrowcolitem.Shift += TableCellGaps[a][b].Item2;
                                    }
                                    //currowcolitem.Shift -= TableCellGaps[a][b].Item2; ;
                                }

                            }
                            //currowcolitem.Height = cellheight;
                            b++;
                        }
                    }

                    tablerowitem.Width = rowwidth;
                    b = 0;
                    a++;
                }

            }

            double sigmaTotalHeight = 0;
            double sigmaDepth = 0;
            double sigmaHeight = 0;
            double adjustedwidth = 0;
            foreach (var item in resultBox.Children)
            {
                sigmaTotalHeight += item.TotalHeight;
                sigmaHeight += item.Height;
                sigmaDepth += item.Depth;
                if (item.TotalWidth>adjustedwidth)
                {
                    adjustedwidth = item.TotalWidth;
                }
            }

            double adjustedTotalHeight = RowsMaxCellHeight.Sum()+ (RowCount * CellBottomTopPadding);

            resultBox.Depth = 0;
            resultBox.Height = adjustedTotalHeight>sigmaTotalHeight?adjustedTotalHeight:sigmaTotalHeight;
            resultBox.Width = adjustedwidth>resultBox.TotalWidth?adjustedwidth:resultBox.TotalWidth;
            var enviroYDiff = axis>= resultBox.TotalHeight ? - (axis- resultBox.TotalHeight)/2: ( resultBox.TotalHeight-axis) / 2;
            resultBox.Shift = enviroYDiff;

            var finalbox = new HorizontalBox() ;
            finalbox.Add(new StrutBox(CellLeftRightPadding/8, 0, 0, 0));
            finalbox.Add(resultBox);
            finalbox.Add(new StrutBox(CellLeftRightPadding/8, 0, 0, 0));

            return finalbox;
        }
    }
}
