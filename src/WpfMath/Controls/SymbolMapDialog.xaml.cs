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
    /// Interaction logic for SymbolMapDialogWindow.xaml
    /// </summary>
    public partial class SymbolMapDialog : Window
    {
        /// <summary>
        /// Initializes a new <see cref="SymbolMapDialog"/>.
        /// </summary>
        public SymbolMapDialog()
        {
            InitializeComponent();
            SymbolName = "comma";
            SymbolCharCode = 59;
            SymbolFontId = 0;
        }

        /// <summary>
        /// Initializes a new <see cref="SymbolMapDialogWindow"/> with the specified
        /// <paramref name="symNm"/>, <paramref name="symch"/> and <paramref name="symfontid"/>.
        /// </summary>
        /// <param name="symNm"></param>
        /// <param name="symch"></param>
        /// <param name="symfontid"></param>
        public SymbolMapDialog(string symNm,int symch,int symfontid)
        {
            SymbolName = symNm;
            SymbolCharCode = symch;
            SymbolFontId = uint.Parse(symfontid.ToString());
        }

        #region Properties
        /// <summary>
        /// Gets or sets the name of the symbol.
        /// </summary>
        public string SymbolName
        {
            get { return symbNameTxb.Text; }
            set { symbNameTxb.Text = value; }
        }

        /// <summary>
        /// Gets or sets the Unicode code point of the symbol's character.
        /// </summary>
        public int SymbolCharCode
        {
            get { return int.Parse(symbchTxbk.Text); }
            set { symbchTxbk.Text = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets the id of the symbol's reference font.
        /// </summary>
        public uint SymbolFontId
        {
            get { return uint.Parse(fontIdTxbk.Text); }
            set { fontIdTxbk.Text = value.ToString(); }
        }
        #endregion

        private void OkBut_Click(object sender, RoutedEventArgs e)
        {
            if (symbNameTxb.Text.Trim()==null|| symbNameTxb.Text.Trim() == "" || symbchTxbk.Text==null||fontIdTxbk.Text==null)
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
