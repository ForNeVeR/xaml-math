using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static WpfMath.Utils.CustomFontUtilities;

namespace WpfMath.Controls
{
    /// <summary>
    /// Interaction logic for SymbolMapPreview.xaml
    /// </summary>
    public partial class SymbolMapPreview : UserControl
    {
        private ObservableCollection<UnicodeListItem> symitems = new ObservableCollection<UnicodeListItem>();
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolMapPreview"/>.
        /// </summary>
        public SymbolMapPreview()
        {
            InitializeComponent();
            SymbolItems = new ObservableCollection<UnicodeListItem>();
            SymbolAdded = Brushes.Green;
            SymbolNotAdded = Brushes.Yellow;
        }

        public SymbolMapPreview(ObservableCollection<UnicodeListItem> input)
        {
            SymbolItems = input;
        }
        #region Properties
        /// <summary>
        /// Gets or sets the symbols contained in this group.
        /// </summary>
        public ObservableCollection<UnicodeListItem> SymbolItems
        {
            get { return symitems; }
            set
            {
                SymbolslistBox.ItemsSource = value;
            }
        }

        /// <summary>
        /// Gets or sets the symbol mapping of the symbols contained in this group.
        /// </summary>
        public List<Tuple<string, int, int>> SymbolValues
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of symbols contained in this <see cref="SymbolMapPreview"/>.
        /// </summary>
        public int SymbolsCount
        {
            get { return SymbolslistBox.Items.Count; }
        }

        /// <summary>
        /// Gets the number of added symbols in this <see cref="SymbolMapPreview"/>.
        /// </summary>
        public int SymbolsAdded
        {
            get { return SymbolValues.Count; }
        }

        /// <summary>
        /// Gets or sets the brush of the Symbol List box item which has been added to <see cref="SymbolValues"/>.
        /// </summary>
        public Brush SymbolAdded
        {
            get;set;
        }

        /// <summary>
        /// Gets or sets the brush of the Symbol List box item which has not been added to <see cref="SymbolValues"/>.
        /// </summary>
        public Brush SymbolNotAdded
        {
            get;set;
        }

        /// <summary>
        /// Gets or sets the selected <see cref="UnicodeListItem"/>.
        /// </summary>
        public UnicodeListItem CurrentSelectedItem
        {
            get; private set;
        }



        #endregion

        #region UI Event Handlers
        private void SelectedItem_KeyDown(object o, KeyEventArgs e)
        {
            if (e.Key==Key.Return)
            {
                if (CurrentSelectedItem!=null)
                {
                    if (CurrentSelectedItem.IsAdded == true)
                    {
                        CurrentSelectedItem.Background = SymbolNotAdded;
                        CurrentSelectedItem.IsAdded = false;
                    }
                    else
                    {
                        SymbolMapDialog dlg = new SymbolMapDialog(CurrentSelectedItem.CodeName,CurrentSelectedItem.Point,CurrentSelectedItem.CodeId);
                        if (dlg.ShowDialog()==true)
                        {
                            CurrentSelectedItem.IsAdded = true;
                            CurrentSelectedItem.Background = SymbolAdded;
                            CurrentSelectedItem.CodeName = dlg.SymbolName;
                        }
                    }
                }
                
            }
        }

        private void SymbolListBoxItem_SelectionChanged(object o, SelectionChangedEventArgs e)
        {
            UnicodeListItem item = o as UnicodeListItem;
            CurrentSelectedItem = item;
        }
        #endregion
    }
}
