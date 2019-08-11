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

            var rowBoxes = new List<HorizontalBox>();
            for (int i = 0; i < rowCount; i++)
            {
                rowBoxes.Add(CreateRowBox(environment, i, maxColumnCount, rowHeights, columnWidths));
            }

            var matrixCellGaps = CalculateCellGaps(rowBoxes, rowHeights, columnWidths);

            var rowsContainer = new VerticalBox();
            foreach (var row in rowBoxes)
            {
                var verticalPadding = new StrutBox(0.0, VerticalPadding / 2, 0, 0);

                rowsContainer.Add(verticalPadding);
                rowsContainer.Add(row);
                rowsContainer.Add(verticalPadding);
            }

            var rows = 0;
            var columns = 0;
            for (int i = 0; i < rowsContainer.Children.Count; i++)
            {
                var Matrixrowitem = rowsContainer.Children[i];

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
                                            nextrowcolitem.Width +=2* matrixCellGaps[rows][columns].Horizontal;
                                            rowwidth += nextrowcolitem.TotalWidth;
                                        }
                                        break;
                                    }

                                case HorizontalAlignment.Right:
                                    {
                                        if (prevrowcolitem is StrutBox && prevrowcolitem.Tag.ToString() == leftstructboxtag)
                                        {
                                            prevrowcolitem.Width +=2* matrixCellGaps[rows][columns].Horizontal;
                                            rowwidth += prevrowcolitem.TotalWidth;
                                        }
                                        if (nextrowcolitem is StrutBox && nextrowcolitem.Tag.ToString() == rightstructboxtag)
                                        {
                                            //nextrowcolitem.Width += matrixCellGaps[a][b].Horizontal;
                                            rowwidth += nextrowcolitem.TotalWidth;
                                        }
                                        break;
                                    }

                                case HorizontalAlignment.Center:
                                    {
                                        if (prevrowcolitem is StrutBox && prevrowcolitem.Tag.ToString() == leftstructboxtag)
                                        {
                                            prevrowcolitem.Width += matrixCellGaps[rows][columns].Horizontal;
                                            rowwidth += prevrowcolitem.TotalWidth;
                                        }
                                        if (nextrowcolitem is StrutBox && nextrowcolitem.Tag.ToString() == rightstructboxtag)
                                        {
                                            nextrowcolitem.Width += matrixCellGaps[rows][columns].Horizontal;
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
                                                    prevcellitem.Height +=2* matrixCellGaps[rows][columns].Vertical;
                                                    //prevcellitem.Background = Brushes.Aquamarine;
                                                    cellheight += prevcellitem.TotalHeight;
                                                    if (prevcellitem.Height > (currowcolitem.Height / 2))
                                                    {

                                                    }
                                                }
                                                if (nextcellitem.Tag.ToString() == bottomstructboxtag)
                                                {
                                                    //nextcellitem.Height += matrixCellGaps[a][b].Vertical;
                                                    //nextcellitem.Background = Brushes.BurlyWood;
                                                    cellheight += nextcellitem.TotalHeight;
                                                }
                                                break;
                                            }
                                        case VerticalAlignment.Center:
                                            {
                                                if (prevcellitem.Tag.ToString() == topstructboxtag)
                                                {
                                                    prevcellitem.Height += matrixCellGaps[rows][columns].Vertical;
                                                    //prevcellitem.Background = Brushes.Aquamarine;
                                                    cellheight += prevcellitem.TotalHeight;
                                                    if (prevcellitem.Height > (currowcolitem.Height / 2))
                                                    {

                                                    }
                                                }
                                                if (nextcellitem.Tag.ToString() == bottomstructboxtag)
                                                {
                                                    nextcellitem.Height += matrixCellGaps[rows][columns].Vertical;
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
                                                    //prevcellitem.Height += matrixCellGaps[a][b].Vertical;
                                                    //prevcellitem.Background = Brushes.Aquamarine;
                                                    cellheight += prevcellitem.TotalHeight;
                                                    if (prevcellitem.Height > (currowcolitem.Height / 2))
                                                    {

                                                    }
                                                }
                                                if (nextcellitem.Tag.ToString() == bottomstructboxtag)
                                                {
                                                    nextcellitem.Height +=2* matrixCellGaps[rows][columns].Vertical;
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
                                        prevrowcolitem.Shift += matrixCellGaps[rows][columns].Vertical;
                                    }
                                    if (nextrowcolitem is StrutBox && nextrowcolitem.Tag.ToString() == rightstructboxtag)
                                    {
                                        nextrowcolitem.Shift += matrixCellGaps[rows][columns].Vertical;
                                    }
                                    //currowcolitem.Shift -= matrixCellGaps[a][b].Vertical; ;
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
            foreach (var item in rowsContainer.Children)
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

            rowsContainer.Depth = 0;
            rowsContainer.Height = adjustedTotalHeight>sigmaTotalHeight?adjustedTotalHeight:sigmaTotalHeight;
            rowsContainer.Width = adjwidth>rowsContainer.TotalWidth?adjwidth:rowsContainer.TotalWidth;
            var enviroYDiff = axis>= rowsContainer.TotalHeight ? - (axis- rowsContainer.TotalHeight)/2: ( rowsContainer.TotalHeight-axis) / 2;
            rowsContainer.Shift = enviroYDiff;

            var finalbox = new HorizontalBox() ;
            finalbox.Add(new StrutBox(HorizontalPadding/8, 0, 0, 0));
            finalbox.Add(rowsContainer);
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
            // TODO[F]: remove rowIndex parameter altogether with Tags
            // TODO[F]: remove rowHeights and colunmWidths arguments (calculate them outside)
            // TODO[F]: return IEnumerable<Box> from here and make the outside code to control the paddings
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

        private static List<List<CellGaps>> CalculateCellGaps(
            IEnumerable<HorizontalBox> rows,
            double[] rowHeights,
            double[] columnWidths)
        {
            var rowIndex = 0;
            var columns = 0;

            var matrixCellGaps = new List<List<CellGaps>>();
            foreach (var row in rows)
            {
                var rowGaps = new List<CellGaps>();

                for (int j = 0; j < row.Children.Count; j++)
                {
                    var rowcolitem = row.Children[j];
                    if (rowcolitem is VerticalBox && rowcolitem.Tag.ToString() == $"Cell{rowIndex}:{columns}")
                    {
                        // TODO[F]: require the vertical box list as input
                        double cellVShift = rowHeights[rowIndex] - rowcolitem.TotalHeight;

                        double cellHShift = columnWidths[columns] - rowcolitem.TotalWidth;
                        row.Children[j - 1].Shift = rowcolitem.Depth;
                        row.Children[j + 1].Shift = rowcolitem.Depth;
                        rowGaps.Add(new CellGaps {Horizontal = cellHShift / 2, Vertical = cellVShift / 2});

                        columns++;
                    }
                }

                columns = 0;
                rowIndex++;
                matrixCellGaps.Add(rowGaps);
            }

            return matrixCellGaps;
        }

        private struct CellGaps
        {
            public double Horizontal { get; set; }
            public double Vertical { get; set; }
        }
    }
}
