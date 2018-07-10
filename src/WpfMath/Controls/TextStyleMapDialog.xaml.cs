using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfMath.Controls
{
    /// <summary>
    /// Interaction logic for TextStyleMapDialog.xaml
    /// </summary>
    public partial class TextStyleMapDialog : Window
    {
        /// <summary>
        /// Initializes a new <see cref="TextStyleMapDialog"/>.
        /// </summary>
        public TextStyleMapDialog()
        {
            InitializeComponent();
            txtStyleTxb.Text = "textstyle";
            TextStyleMapRanges=new List<Tuple<string, uint, uint>>();
        }

        public TextStyleMapDialog(string txtmap="textstyle", List<Tuple<string, uint, uint>> mapranges =  null)
        {
            InitializeComponent();
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
        public List<Tuple<string, uint,uint>> TextStyleMapRanges
        {
            get
            {
                var result = new List<Tuple<string, uint, uint>>();
                if (mapRangeGrid.RowDefinitions.Count>1)
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
                                if ((int)item.GetValue(Grid.ColumnProperty) == 0 && item is TextBox)
                                {
                                    maprangeStr = ((TextBox)item).Text.Trim();
                                }
                                if ((int)item.GetValue(Grid.ColumnProperty) == 1 && item is Xceed.Wpf.Toolkit.IntegerUpDown)
                                {
                                    maprangefid = (uint)((Xceed.Wpf.Toolkit.IntegerUpDown)item).Value;
                                }
                                if ((int)item.GetValue(Grid.ColumnProperty) == 2 && item is Xceed.Wpf.Toolkit.IntegerUpDown)
                                {
                                    maprangestart = (uint)((Xceed.Wpf.Toolkit.IntegerUpDown)item).Value;
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
                    Text="Code",
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
                    TextBox txb = new TextBox
                    {
                        Height = 20,
                        Text=value[i].Item1
                    };
                    txb.SetValue(Grid.RowProperty, mapRangeGrid.RowDefinitions.Count);
                    txb.SetValue(Grid.ColumnProperty, 0);
                    Xceed.Wpf.Toolkit.IntegerUpDown fidTxb = new Xceed.Wpf.Toolkit.IntegerUpDown
                    {
                        Height = 20,
                        Minimum = 0,
                        //Maximum = 5,
                        Value= (int)value[i].Item2
                    };
                    fidTxb.SetValue(Grid.RowProperty, mapRangeGrid.RowDefinitions.Count);
                    fidTxb.SetValue(Grid.ColumnProperty, 1);
                    Xceed.Wpf.Toolkit.IntegerUpDown startTxb = new Xceed.Wpf.Toolkit.IntegerUpDown
                    {
                        Height = 20,
                        Minimum = 0,
                        //Maximum = 5,
                        Value = (int)value[i].Item3
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

        private void AddMapRange_Click(object o, RoutedEventArgs e)
        {
            
            TextBox txb = new TextBox
            {
                Height=20
            };
            txb.SetValue(Grid.RowProperty, mapRangeGrid.RowDefinitions.Count);
            txb.SetValue(Grid.ColumnProperty, 0);
            Xceed.Wpf.Toolkit.IntegerUpDown fidTxb = new Xceed.Wpf.Toolkit.IntegerUpDown
            {
                Height=20,
                Minimum=0,
                //Maximum=5
            };
            fidTxb.SetValue(Grid.RowProperty, mapRangeGrid.RowDefinitions.Count);
            fidTxb.SetValue(Grid.ColumnProperty, 1);
            Xceed.Wpf.Toolkit.IntegerUpDown startTxb = new Xceed.Wpf.Toolkit.IntegerUpDown
            {
                Height = 20,
                Minimum = 0,
                //Maximum = 5
            };
            startTxb.SetValue(Grid.RowProperty, mapRangeGrid.RowDefinitions.Count);
            startTxb.SetValue(Grid.ColumnProperty, 2);
            RowDefinition rowdef = new RowDefinition()
            {
                Height=new GridLength(20,GridUnitType.Pixel)
            };
            mapRangeGrid.RowDefinitions.Add(rowdef);
            mapRangeGrid.Children.Add(txb);
            mapRangeGrid.Children.Add(fidTxb);
            mapRangeGrid.Children.Add(startTxb);
        }

        private void OkBut_Click(object sender, RoutedEventArgs e)
        {
            if (txtStyleTxb.Text.Trim()==null|| txtStyleTxb.Text.Trim() == ""||mapRangeGrid.RowDefinitions.Count==1)
            {
                MessageBox.Show("The input fields cannot be empty.");
            }
            else
            {
                DialogResult = true;
            }
        }


    }
}
