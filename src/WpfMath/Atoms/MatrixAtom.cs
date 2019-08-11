using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    /// <summary>An atom representing a tabular arrangement of atoms.</summary>
    internal class MatrixAtom : Atom
    {
        public MatrixAtom(
            SourceSpan source,
            List<List<Atom>> cells,
            VerticalAlignment verticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
            double verticalPadding = 0.35,
            double horizontalPadding = 0.35) : base(source)
        {
            MatrixCells = new ReadOnlyCollection<ReadOnlyCollection<Atom>>(
                cells.Select(row => new ReadOnlyCollection<Atom>(row)).ToList());
            HorizontalAlignment = horizontalAlignment;
            VerticalAlignment = verticalAlignment;
            VerticalPadding = verticalPadding;
            HorizontalPadding = horizontalPadding;
        }

        public ReadOnlyCollection<ReadOnlyCollection<Atom>> MatrixCells { get; }

        public double VerticalPadding { get; }

        public double HorizontalPadding { get; }

        public VerticalAlignment VerticalAlignment { get; }

        public HorizontalAlignment HorizontalAlignment { get; }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);

            var rowCount = MatrixCells.Count;
            var maxColumnCount = MatrixCells.Max(row => row.Count);

            var rowHeights = new double[rowCount];
            var columnWidths = new double[maxColumnCount];

            //Create a vertical box to hold the rows and their cells
            var resultBox = new VerticalBox();

            for (int i = 0; i < rowCount; i++)
            {
                var verticalPadding = new StrutBox(0.0, VerticalPadding / 2, 0, 0);
                var row = CreateRowBox(environment, i, maxColumnCount, rowHeights, columnWidths);

                resultBox.Add(verticalPadding);
                resultBox.Add(row);
                resultBox.Add(verticalPadding);
            }

            int rows = 0;
            int columns = 0;
            //1->Left and right, 2->Top and Bottom
            //create the item below to hold the left-right gaps(Tuple.Item1) and top-bottom gaps (Tuple.Item2) for the rows
            List<List<Tuple<double, double>>> MatrixCellGaps = new List<List<Tuple<double, double>>>();
            for (int i = 0; i < resultBox.Children.Count; i++)
            {
                var Matrixrowitem = resultBox.Children[i];
                List<Tuple<double, double>> RowGaps = new List<Tuple<double, double>>();

                if (Matrixrowitem is HorizontalBox&&Matrixrowitem.Tag.ToString()==$"Row:{rows}")
                {
                    for (int j = 0; j < ((HorizontalBox)Matrixrowitem).Children.Count; j++)
                    {
                        var rowcolitem = ((HorizontalBox)Matrixrowitem).Children[j];
                        if (rowcolitem is StrutBox)
                        {
                            rowcolitem.Height = rowHeights[rows];
                        }
                        else if(rowcolitem is VerticalBox && rowcolitem.Tag.ToString() == $"Cell{rows}:{columns}")
                        {
                            double cellVShift = rowHeights[rows] - rowcolitem.TotalHeight;

                            double cellHShift = columnWidths[columns]-rowcolitem.TotalWidth;
                            ((HorizontalBox)Matrixrowitem).Children[j - 1].Shift = rowcolitem.Depth;// + (cellVShift / 2);//.Width += cellHShift / 2;
                            ((HorizontalBox)Matrixrowitem).Children[j + 1].Shift = rowcolitem.Depth;// +(cellVShift / 2);// Width += cellHShift / 2;
                            //rowcolitem.Shift =  cellVShift/2;
                            RowGaps.Add( new Tuple<double, double> (cellHShift/2, cellVShift/2));

                            columns++;
                        }
                    }

                    columns = 0;
                    rows++;
                    MatrixCellGaps.Add(RowGaps);
                }

            }

            rows = 0;
            columns = 0;
            for (int i = 0; i < resultBox.Children.Count; i++)
            {
                var Matrixrowitem = resultBox.Children[i];

                if (Matrixrowitem is HorizontalBox && Matrixrowitem.Tag.ToString() == $"Row:{rows}")
                {
                    double rowwidth = 0;
                    for (int j = 0; j < ((HorizontalBox)Matrixrowitem).Children.Count; j++)
                    {
                        var currowcolitem = ((HorizontalBox)Matrixrowitem).Children[j];
                        var prevrowcolitem = j > 0 ? ((HorizontalBox)Matrixrowitem).Children[j - 1] : ((HorizontalBox)Matrixrowitem).Children[j];
                        var nextrowcolitem = j < ((HorizontalBox)Matrixrowitem).Children.Count-1 ? ((HorizontalBox)Matrixrowitem).Children[j + 1] : ((HorizontalBox)Matrixrowitem).Children[j];

                        if (currowcolitem is VerticalBox&& Regex.IsMatch(currowcolitem.Tag.ToString(), @"Cell[0-9]+:[0-9]+"))
                        {
                            rowwidth += currowcolitem.TotalWidth;
                            var leftstructboxtag = $"CellLeftPad{rows}:{columns}";
                            var rightstructboxtag = $"CellRightPad{rows}:{columns}";

                            switch (HorizontalAlignment)
                            {
                                case HorizontalAlignment.Left:
                                    {
                                        if (prevrowcolitem is StrutBox && prevrowcolitem.Tag.ToString() == leftstructboxtag)
                                        {
                                            //prevrowcolitem.Width += MatrixCellGaps[a][b].Item1;
                                            rowwidth += prevrowcolitem.TotalWidth;
                                        }
                                        if (nextrowcolitem is StrutBox && nextrowcolitem.Tag.ToString() == rightstructboxtag)
                                        {
                                            nextrowcolitem.Width +=2* MatrixCellGaps[rows][columns].Item1;
                                            rowwidth += nextrowcolitem.TotalWidth;
                                        }
                                        break;
                                    }

                                case HorizontalAlignment.Right:
                                    {
                                        if (prevrowcolitem is StrutBox && prevrowcolitem.Tag.ToString() == leftstructboxtag)
                                        {
                                            prevrowcolitem.Width +=2* MatrixCellGaps[rows][columns].Item1;
                                            rowwidth += prevrowcolitem.TotalWidth;
                                        }
                                        if (nextrowcolitem is StrutBox && nextrowcolitem.Tag.ToString() == rightstructboxtag)
                                        {
                                            //nextrowcolitem.Width += MatrixCellGaps[a][b].Item1;
                                            rowwidth += nextrowcolitem.TotalWidth;
                                        }
                                        break;
                                    }

                                case HorizontalAlignment.Center:
                                    {
                                        if (prevrowcolitem is StrutBox && prevrowcolitem.Tag.ToString() == leftstructboxtag)
                                        {
                                            prevrowcolitem.Width += MatrixCellGaps[rows][columns].Item1;
                                            rowwidth += prevrowcolitem.TotalWidth;
                                        }
                                        if (nextrowcolitem is StrutBox && nextrowcolitem.Tag.ToString() == rightstructboxtag)
                                        {
                                            nextrowcolitem.Width += MatrixCellGaps[rows][columns].Item1;
                                            rowwidth += nextrowcolitem.TotalWidth;
                                        }
                                        break;
                                    }

                                //case HorizontalAlignment.Stretch:
                                //    {
                                //        if (prevrowcolitem is StrutBox && prevrowcolitem.Tag.ToString() == leftstructboxtag)
                                //        {
                                //            prevrowcolitem.Width = 0;
                                //            prevrowcolitem.Italic = 0;
                                //            rowwidth += prevrowcolitem.TotalWidth;
                                //        }
                                //        if (nextrowcolitem is StrutBox && nextrowcolitem.Tag.ToString() == rightstructboxtag)
                                //        {
                                //            nextrowcolitem.Width = 0;
                                //            nextrowcolitem.Italic = 0;
                                //            rowwidth += nextrowcolitem.TotalWidth;
                                //        }
                                //        break;
                                //    }

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
                                    var topstructboxtag = $"CellTopPad{rows}:{columns}";
                                    var bottomstructboxtag = $"CellBottomPad{rows}:{columns}";

                                    switch (VerticalAlignment)
                                    {
                                        case VerticalAlignment.Bottom:
                                            {
                                                if (prevcellitem.Tag.ToString() == topstructboxtag)
                                                {
                                                    prevcellitem.Height +=2* MatrixCellGaps[rows][columns].Item2;
                                                    //prevcellitem.Background = Brushes.Aquamarine;
                                                    cellheight += prevcellitem.TotalHeight;
                                                    if (prevcellitem.Height > (currowcolitem.Height / 2))
                                                    {

                                                    }
                                                }
                                                if (nextcellitem.Tag.ToString() == bottomstructboxtag)
                                                {
                                                    //nextcellitem.Height += MatrixCellGaps[a][b].Item2;
                                                    //nextcellitem.Background = Brushes.BurlyWood;
                                                    cellheight += nextcellitem.TotalHeight;
                                                }
                                                break;
                                            }
                                        case VerticalAlignment.Center:
                                            {
                                                if (prevcellitem.Tag.ToString() == topstructboxtag)
                                                {
                                                    prevcellitem.Height += MatrixCellGaps[rows][columns].Item2;
                                                    //prevcellitem.Background = Brushes.Aquamarine;
                                                    cellheight += prevcellitem.TotalHeight;
                                                    if (prevcellitem.Height > (currowcolitem.Height / 2))
                                                    {

                                                    }
                                                }
                                                if (nextcellitem.Tag.ToString() == bottomstructboxtag)
                                                {
                                                    nextcellitem.Height += MatrixCellGaps[rows][columns].Item2;
                                                    //nextcellitem.Background = Brushes.BurlyWood;
                                                    cellheight += nextcellitem.TotalHeight;
                                                }
                                                break;
                                            }
                                        //case VerticalAlignment.Stretch:
                                        //    {
                                        //        if (prevcellitem.Tag.ToString() == topstructboxtag)
                                        //        {
                                        //            prevcellitem.Height = 0;
                                        //            prevcellitem.Depth = 0;
                                        //            //prevcellitem.Background = Brushes.Aquamarine;
                                        //            cellheight += prevcellitem.TotalHeight;
                                        //            if (prevcellitem.Height > (currowcolitem.Height / 2))
                                        //            {

                                        //            }
                                        //        }
                                        //        if (nextcellitem.Tag.ToString() == bottomstructboxtag)
                                        //        {
                                        //            nextcellitem.Height = 0;
                                        //            nextcellitem.Depth = 0;
                                        //            //nextcellitem.Background = Brushes.BurlyWood;
                                        //            cellheight += nextcellitem.TotalHeight;
                                        //        }
                                        //        break;
                                        //    }
                                        case VerticalAlignment.Top:
                                            {
                                                if (prevcellitem.Tag.ToString() == topstructboxtag)
                                                {
                                                    //prevcellitem.Height += MatrixCellGaps[a][b].Item2;
                                                    //prevcellitem.Background = Brushes.Aquamarine;
                                                    cellheight += prevcellitem.TotalHeight;
                                                    if (prevcellitem.Height > (currowcolitem.Height / 2))
                                                    {

                                                    }
                                                }
                                                if (nextcellitem.Tag.ToString() == bottomstructboxtag)
                                                {
                                                    nextcellitem.Height +=2* MatrixCellGaps[rows][columns].Item2;
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
                                        prevrowcolitem.Shift += MatrixCellGaps[rows][columns].Item2;
                                    }
                                    if (nextrowcolitem is StrutBox && nextrowcolitem.Tag.ToString() == rightstructboxtag)
                                    {
                                        nextrowcolitem.Shift += MatrixCellGaps[rows][columns].Item2;
                                    }
                                    //currowcolitem.Shift -= MatrixCellGaps[a][b].Item2; ;
                                }


                            }
                            //currowcolitem.Height = cellheight;
                            columns++;
                        }
                    }

                    Matrixrowitem.Width = rowwidth;
                    columns = 0;
                    rows++;
                }

            }

            double sigmaTotalHeight = 0;
            double sigmaDepth = 0;
            double sigmaHeight = 0;
            double adjwidth = 0;
            foreach (var item in resultBox.Children)
            {
                sigmaTotalHeight += item.TotalHeight;
                sigmaHeight += item.Height;
                sigmaDepth += item.Depth;
                if (item.TotalWidth>adjwidth)
                {
                    adjwidth = item.TotalWidth;
                }
            }

            double adjustedTotalHeight = rowHeights.Sum()+ (rowCount * VerticalPadding);

            resultBox.Depth = 0;
            resultBox.Height = adjustedTotalHeight>sigmaTotalHeight?adjustedTotalHeight:sigmaTotalHeight;
            resultBox.Width = adjwidth>resultBox.TotalWidth?adjwidth:resultBox.TotalWidth;
            var enviroYDiff = axis>= resultBox.TotalHeight ? - (axis- resultBox.TotalHeight)/2: ( resultBox.TotalHeight-axis) / 2;
            resultBox.Shift = enviroYDiff;

            var finalbox = new HorizontalBox() ;
            finalbox.Add(new StrutBox(HorizontalPadding/8, 0, 0, 0));
            finalbox.Add(resultBox);
            finalbox.Add(new StrutBox(HorizontalPadding/8, 0, 0, 0));

            return finalbox;
        }

        private HorizontalBox CreateRowBox(
            TexEnvironment environment,
            int rowIndex,
            int maxColumnCount,
            double[] rowHeights,
            double[] columnWidths)
        {
            var rowBox = new HorizontalBox {Tag = $"Row:{rowIndex}",};

            for (int j = 0; j < maxColumnCount; j++)
            {
                //cell left pad
                var cellleftpad = new StrutBox(HorizontalPadding / 2, 0, 0, 0)
                {
                    Tag = $"CellLeftPad{rowIndex}:{j}",
                    Shift = 0,
                };

                //cell right pad
                var cellrightpad = new StrutBox(HorizontalPadding / 2, 0, 0, 0)
                {
                    Tag = $"CellRightPad{rowIndex}:{j}",
                    Shift = 0,
                };

                //cell box
                var rowcellbox = MatrixCells[rowIndex][j] == null ? StrutBox.Empty : MatrixCells[rowIndex][j].CreateBox(environment);
                rowcellbox.Tag = "innercell";
                //cell box holder
                var rowcolbox = new VerticalBox() {Tag = $"Cell{rowIndex}:{j}"};

                var celltoppad = new StrutBox(rowcellbox.TotalWidth, VerticalPadding / 2, 0, 0) {Tag = $"CellTopPad{rowIndex}:{j}",};
                var cellbottompad = new StrutBox(rowcellbox.TotalWidth, VerticalPadding / 2, 0, 0)
                    {Tag = $"CellBottomPad{rowIndex}:{j}",};
                rowcolbox.Add(celltoppad);
                rowcolbox.Add(rowcellbox);
                rowcolbox.Add(cellbottompad);

                rowcolbox.Width = rowcellbox.TotalWidth;

                rowBox.Add(cellleftpad);
                rowBox.Add(rowcolbox);
                rowBox.Add(cellrightpad);

                var cellHeight = rowcellbox.TotalHeight + VerticalPadding;
                rowHeights[rowIndex] = Math.Max(rowHeights[rowIndex], cellHeight);

                var columnWidth = rowcellbox.TotalWidth;
                columnWidths[j] = Math.Max(columnWidths[j], columnWidth);
            }

            return rowBox;
        }
    }
}
