using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static WpfMath.Utilities.CustomFontUtilities;

namespace WpfMath.Controls
{
    /// <summary>
    /// Interaction logic for FontDescriptionPreviewControl.xaml
    /// </summary>
    public partial class FontDescriptionPreview : UserControl
    {
        private ObservableCollection<UnicodeListItem> fontsymitms=new ObservableCollection<UnicodeListItem>();

        public FontDescriptionPreview()
        {
            InitializeComponent();
            FontSymbolslistBox.ItemsSource = FontItems;

        }

        public FontDescriptionPreview(ObservableCollection<UnicodeListItem> item)
        {
            InitializeComponent();
            FontItems = item;
            FontSymbolslistBox.ItemsSource = item;
        }

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<UnicodeListItem> FontItems
        {
            get
            {
                return fontsymitms;
            }
            set
            {
                fontsymitms = value;
                FontSymbolslistBox.ItemsSource = value;
            }
        }

        /// <summary>
        /// Gets or sets the point from which the unicode characters start to get picked.
        /// </summary>
        public int StartPoint
        {
            get { return (int)startTxb.Value; }
            set { startTxb.Value = value; }
        }

        /// <summary>
        /// Gets or sets the point from which the unicode characters will stop to be picked.
        /// </summary>
        public int EndPoint
        {
            get { return (int)endTxb.Value; }
            set { endTxb.Value = value; }
        }

        public string FontName
        {
            get;set;
        }

        public string FontPath
        {
            get;set;
        }

        /// <summary>
        /// Gets or sets the selected <see cref="UnicodeListItem"/>.
        /// </summary>
        public UnicodeListItem CurrentSelectedItem
        {
            get;private set;
        }
        #endregion


        private void FontDescriptionListBoxItem_SelectionChanged(object o, SelectionChangedEventArgs e)
        {
            UnicodeListItem item = FontSymbolslistBox.SelectedItem as UnicodeListItem;
            CurrentSelectedItem = item;
            if (item!=null)
            {
                codeptTxbx.Text = item.Point.ToString() ?? " ";
            }
            
        }

        private void Update_Click(object o, RoutedEventArgs e)
        {
            Init(FontName,FontPath);
            FontItems = GetFontChars(FontName, StartPoint, EndPoint);
        }

        private void CopyListBoxItem_Click(object o, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentSelectedItem.UnicodeText);
        }
    }
}
