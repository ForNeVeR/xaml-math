using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfMath.Example
{
    public partial class MainWindow : Window
    {
        private TexFormulaParser formulaParser;

        public MainWindow()
        {
            InitializeComponent();
        }

        private TexFormula ParseFormula(string input)
        {
            // Create formula object from input text.
            TexFormula formula = null;
            try
            {
                formula = this.formulaParser.Parse(input);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while parsing the given input:" + Environment.NewLine +
                    Environment.NewLine + ex.Message, "WPF-Math Example", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return formula;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            // Choose file
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "SVG Files (*.svg)|*.svg|PNG Files (*.png)|*.png"
            };
            var result = saveFileDialog.ShowDialog();
            if (result == false) return;

            // Create formula object from input text.
            var formula = ParseFormula(inputTextBox.Text);
            if (formula == null) return;
            var renderer = formula.GetRenderer(TexStyle.Display, this.formula.Scale, "Arial");

            // Open stream
            var filename = saveFileDialog.FileName;
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        var geometry = renderer.RenderToGeometry(0, 0);
                        var converter = new SVGConverter();
                        var svgPathText = converter.ConvertGeometry(geometry);
                        var svgText = AddSVGHeader(svgPathText);
                        using (var writer = new StreamWriter(stream))
                            writer.WriteLine(svgText);
                        break;

                    case 2:
                        var bitmap = renderer.RenderToBitmap(0, 0);
                        var encoder = new PngBitmapEncoder
                        {
                            Frames = { BitmapFrame.Create(bitmap) }
                        };
                        encoder.Save(stream);
                        break;

                    default:
                        return;
                }
            }
        }

        private string AddSVGHeader(string svgText)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>")
                .AppendLine("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" >")
                .AppendLine(svgText)
                .AppendLine("</svg>");

            return builder.ToString();
        }

     private void TxbCM_Opened(object o, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText() == false)
            {
                pasteMenuItem.IsEnabled = false;
            }
            if (Clipboard.ContainsText() == true)
            {
                pasteMenuItem.IsEnabled = true;
            }
            if (inputTextBox.SelectedText.Length == 0)
            {
                cutMenuItem.IsEnabled = false;
                copyMenuItem.IsEnabled = false;
                copyMMLMenuItem.IsEnabled = false;
            }
            if (inputTextBox.SelectedText.Length > 0)
            {
                cutMenuItem.IsEnabled = true;
                copyMenuItem.IsEnabled = true;
                copyMMLMenuItem.IsEnabled = true;
            }
        }


        private void Cut_Click(object o, RoutedEventArgs e)
        {
            inputTextBox.Cut();
        }

        private void Copy_Click(object o, RoutedEventArgs e)
        {
            Clipboard.SetText(inputTextBox.SelectedText);
        }

        private void CopyasMML_Click(object o, RoutedEventArgs e)
        {

        }

        private void Paste_Click(object o, RoutedEventArgs e)
        {
            if (inputTextBox.SelectedText.Length>0)
            {
                int tci = inputTextBox.CaretIndex;
                int cTL = Clipboard.GetText().Length;
                string newText = inputTextBox.Text.Insert(inputTextBox.CaretIndex, Clipboard.GetText());

            }
            else
            {
                int tci = inputTextBox.CaretIndex;
                int cTL = Clipboard.GetText().Length;
                string newText = inputTextBox.Text.Insert(inputTextBox.CaretIndex, Clipboard.GetText());
                inputTextBox.Text = newText;
                inputTextBox.CaretIndex = tci + cTL;
                inputTextBox.Select(tci, cTL);
            }
        }

        private void OpenMMLFile_Click(object o, RoutedEventArgs e)
        {
            try
            {
                Converters.MathMLtoTexFormula MML2Tex = new Converters.MathMLtoTexFormula();
                OpenFileDialog dlg = new OpenFileDialog
                {
                    Title = "Select the MathML File.",
                    Multiselect = false,
                    Filter = "Math ML File(*.mml)|*.mml"
                };
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    inputTextBox.Text = MML2Tex.Parse(dlg.FileName);
                }
            }
            catch (Exception Ex)
            {

                MessageBox.Show(Ex.Message);
            }
            
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.formulaParser = new TexFormulaParser();

            var testFormula1 = "\\int_0^{\\infty}{x^{2n} e^{-a x^2} dx} = \\frac{2n-1}{2a} \\int_0^{\\infty}{x^{2(n-1)} e^{-a x^2} dx} = \\frac{(2n-1)!!}{2^{n+1}} \\sqrt{\\frac{\\pi}{a^{2n+1}}}";
            var testFormula2 = "\\int_a^b{f(x) dx} = (b - a) \\sum_{n = 1}^{\\infty}  {\\sum_{m = 1}^{2^n  - 1} { ( { - 1} )^{m + 1} } } 2^{ - n} f(a + m ( {b - a}  )2^{-n} )";
            var testFormula3 = @"L = \int_a^b \sqrt[4]{ \left| \sum_{i,j=1}^ng_{ij}\left(\gamma(t)\right) \left[\frac{d}{dt}x^i\circ\gamma(t) \right] \left{\frac{d}{dt}x^j\circ\gamma(t) \right} \right|}dt";
            this.inputTextBox.Text = testFormula3;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //
        }

        private void inputTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            formula.SelectionStart = inputTextBox.SelectionStart;
            formula.SelectionLength = inputTextBox.SelectionLength;
        }
    }
}
