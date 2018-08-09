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
    /// Interaction logic for SymbolGroupDialog.xaml
    /// </summary>
    public partial class SymbolGroupDialog : Window
    {
        public SymbolGroupDialog()
        {
            InitializeComponent();
        }

        public SymbolGroupDialog(string input)
        {
            InitializeComponent();
            SymbolGroupName = input;
        }

        public string SymbolGroupName
        {
            get { return symboltxb.Text; }
            set { symboltxb.Text = value; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
