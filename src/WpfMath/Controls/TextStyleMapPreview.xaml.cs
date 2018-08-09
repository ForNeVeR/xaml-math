using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfMath.Controls
{
    /// <summary>
    /// Interaction logic for TextStyleMapPreview.xaml
    /// </summary>
    public partial class TextStyleMapPreview : UserControl
    {
        /// <summary>
        /// Initializes a new <see cref="TextStyleMapPreview"/>.
        /// </summary>
        public TextStyleMapPreview()
        {
            InitializeComponent();
            TextStyleMapName = " ";
            TextStyleMapRanges = new List<Tuple<string, uint, uint>>();
        }

        public TextStyleMapPreview(string txtmap, List<Tuple<string, uint, uint>> mapranges)
        {
            TextStyleMapName = txtmap;
            TextStyleMapRanges = mapranges;

        }

        #region Properties
        /// <summary>
        /// Gets or sets the name of this Text Style Mapping.
        /// </summary>
        public string TextStyleMapName
        {
            get { return txtStyleTxb.Text; }
            set { txtStyleTxb.Text = value; }
        }

        /// <summary>
        /// Gets or sets the map ranges of the text style.
        /// </summary>
        public List<Tuple<string, uint, uint>> TextStyleMapRanges
        {
            get
            {
                var result = new List<Tuple<string, uint, uint>>();
                if (mapRangeGrid.RowDefinitions.Count > 1)
                {
                    for (int i = 1; i < mapRangeGrid.RowDefinitions.Count; i ++)
                    {
                        string maprangeStr = "";
                        uint maprangefid = 0;
                        uint maprangestart = 0;
                        foreach (UIElement item in mapRangeGrid.Children)
                        {
                            if ((int)item.GetValue(Grid.RowProperty) == i)
                            {
                                if ((int)item.GetValue(Grid.ColumnProperty) == 0 && item is TextBlock)
                                {
                                    maprangeStr = ((TextBlock)item).Text.Trim();
                                }
                                if ((int)item.GetValue(Grid.ColumnProperty) == 1 && item is TextBlock)
                                {
                                    maprangefid = uint.Parse(((TextBlock)item).Text);
                                }
                                if ((int)item.GetValue(Grid.ColumnProperty) == 2 && item is TextBlock)
                                {
                                    maprangestart = uint.Parse(((TextBlock)item).Text);
                                }
                            }
                        }
                        result.Add(new Tuple<string, uint, uint>(maprangeStr, maprangefid, maprangestart));
                    }
                }
                return result;
            }
            set
            {
                mapRangeGrid.RowDefinitions.Clear();
                mapRangeGrid.Children.Clear();
                mapRangeGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Pixel) });

                TextBlock txbk = new TextBlock
                {
                    Text = "Code",
                    ToolTip = "The name of the map range"
                };
                txbk.SetValue(Grid.RowProperty, 0);
                txbk.SetValue(Grid.ColumnProperty, 0);
                TextBlock txbk1 = new TextBlock
                {
                    Text = "Font Id ",
                    ToolTip = "The font id of the map range"
                };
                txbk1.SetValue(Grid.RowProperty, 0);
                txbk1.SetValue(Grid.ColumnProperty, 1);
                TextBlock txbk2 = new TextBlock
                {
                    Text = "Start ",
                    ToolTip = "The start point of the map range"
                };
                txbk2.SetValue(Grid.RowProperty, 0);
                txbk2.SetValue(Grid.ColumnProperty, 2);
                mapRangeGrid.Children.Add(txbk);
                mapRangeGrid.Children.Add(txbk1);
                mapRangeGrid.Children.Add(txbk2);
                for (int i = 0; i < value.Count; i++)
                {
                    TextBlock txb = new TextBlock
                    {
                        Height = 20,
                        Text = value[i].Item1
                    };
                    txb.SetValue(Grid.RowProperty, mapRangeGrid.RowDefinitions.Count);
                    txb.SetValue(Grid.ColumnProperty, 0);
                    TextBlock fidTxb = new TextBlock
                    {
                        Height = 20,
                        Text = value[i].Item2.ToString()
                    };
                    fidTxb.SetValue(Grid.RowProperty, mapRangeGrid.RowDefinitions.Count);
                    fidTxb.SetValue(Grid.ColumnProperty, 1);
                    TextBlock startTxb = new TextBlock
                    {
                        Height = 20,
                        //Maximum = 5,
                        Text = value[i].Item3.ToString()
                    };
                    startTxb.SetValue(Grid.RowProperty, mapRangeGrid.RowDefinitions.Count);
                    startTxb.SetValue(Grid.ColumnProperty, 2);
                    RowDefinition rowdef = new RowDefinition()
                    {
                        Height = new GridLength(20, GridUnitType.Pixel)
                    };
                    mapRangeGrid.RowDefinitions.Add(rowdef);
                    mapRangeGrid.Children.Add(txb);
                    mapRangeGrid.Children.Add(fidTxb);
                    mapRangeGrid.Children.Add(startTxb);
                }
            }
        }

        #endregion

        private void EditBut_Click(object sender, RoutedEventArgs e)
        {
            TextStyleMapDialog dlg = new TextStyleMapDialog(TextStyleMapName, TextStyleMapRanges) {
            
            };
            if (dlg.ShowDialog()==true)
            {
                TextStyleMapName = dlg.TextStyleMapName;
                TextStyleMapRanges = dlg.TextStyleMapRanges;
            }
        }
    }
}
