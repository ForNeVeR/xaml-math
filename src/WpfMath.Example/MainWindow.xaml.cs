using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WpfMath.Converters;

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
                        var bitmap = renderer.RenderToBitmap(0, 0, 300);
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.formulaParser = new TexFormulaParser();

            var testFormula1 = "\\int_0^{\\infty}{x^{2n} e^{-a x^2} dx} = \\frac{2n-1}{2a} \\int_0^{\\infty}{x^{2(n-1)} e^{-a x^2} dx} = \\frac{(2n-1)!!}{2^{n+1}} \\sqrt{\\frac{\\pi}{a^{2n+1}}}";
            var testFormula2 = "\\int_a^b{f(x) dx} = (b - a) \\sum_{n = 1}^{\\infty}  {\\sum_{m = 1}^{2^n  - 1} { ( { - 1} )^{m + 1} } } 2^{ - n} f(a + m ( {b - a}  )2^{-n} )";
            var testFormula3 = @"L = \int_a^b \sqrt[4]{ \left| \sum_{i,j=1}^ng_{ij}\left(\gamma(t)\right) \left[\frac{d}{dt}x^i\circ\gamma(t) \right] \left{\frac{d}{dt}x^j\circ\gamma(t) \right} \right|}dt";
            //matrix examples
            var testFormula4 = @"\matrix{4&78&3 \\ 5 & 9  & 82 }";
            var testFormula5 = @"\cases{x,&if x > 0;\cr -x,& otherwise.}";
            var testFormula6 = @"\matrix{4&78&3\\ 57 & {\matrix{78 \\ 12}}  & 20782 }";
            var testFormula7 = @"\cases{y>78 & if12<=x<125 \\ y=0 & otherwise; }";
            var testFormula8 = @"f(x) = \cases{1/3 & if \thinspace 0\le x\le 1;\cr 2/3 & if \thinspace 3\le x \le 4; \cr 0 & elsewhere.\cr}";
            var testFormula9 = @"v \times w = \left( \matrix{v_2 w_3 - v_3 w_2 \\ v_3 w_1 - v_1 w_3 \\ v_1 w_2 - v_2 w_1} \right) where v= \left(\matrix{ v_1 \\ v_2 \\ v_3 }\right), w= \left( \matrix{w_1 \\ w_2  \\ w_3} \right)";
            var testFormula10 = @"\Gamma_{\mu \rho} ^{\sigma}= \pmatrix{\pmatrix{0 & 0 & 0 \\ 0 & -r & 0 \\ 0 & 0 & -r sin^2(\theta)} \\ \pmatrix{0 & \frac{1}{r} & 0 \\ \frac{1}{r} & 0 & 0 \\ 0 & 0 & -\sin(\theta) \cos(\theta)} \\ \pmatrix{0 & 0 & \frac{1}{r} \\ 0 & 0 & \frac{1}{\tan(\theta)} \\ \frac{1}{r} & \frac{1}{\tan(\theta)} & 0 }}";

            this.inputTextBox.Text = testFormula10;
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
