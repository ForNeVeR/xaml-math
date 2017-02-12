using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

        private void renderButton_Click(object sender, RoutedEventArgs e)
        {
            // Create formula object from input text.
            var formula = ParseFormula(this.inputTextBox.Text);
            if (formula == null) return;

            // Render formula to visual.
            var visual = new DrawingVisual();
            var renderer = formula.GetRenderer(TexStyle.Display, 20d);
            var formulaSize = renderer.RenderSize;

            using (var drawingContext = visual.RenderOpen())
            {
                renderer.Render(drawingContext, 0, 1);
            }

            this.formulaContainerElement.Visual = visual;
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
            var formula = ParseFormula(this.inputTextBox.Text);
            if (formula == null) return;        
            var renderer = formula.GetRenderer(TexStyle.Display, 20d);

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
                        var writer = new StreamWriter(stream);
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TexFormulaParser.Initialize();
            this.formulaParser = new TexFormulaParser();

            var testFormula1 = "\\int_0^{\\infty}{x^{2n} e^{-a x^2} dx} = \\frac{2n-1}{2a} \\int_0^{\\infty}{x^{2(n-1)} e^{-a x^2} dx} = \\frac{(2n-1)!!}{2^{n+1}} \\sqrt{\\frac{\\pi}{a^{2n+1}}}";
            var testFormula2 = "\\int_a^b{f(x) dx} = (b - a) \\sum_{n = 1}^{\\infty}  {\\sum_{m = 1}^{2^n  - 1} { ( { - 1} )^{m + 1} } } 2^{ - n} f(a + m ( {b - a}  )2^{-n} )";
            var testFormula3 = "L = \\int_a^b \\sqrt[4]{ |\\sum_{i,j=1}^ng_{ij}(\\gamma(t)) (\\frac{d}{dt}x^i\\circ\\gamma(t) ) (\\frac{d}{dt}x^j\\circ\\gamma(t) ) |}dt";
            this.inputTextBox.Text = testFormula3;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //
        }
    }
}
