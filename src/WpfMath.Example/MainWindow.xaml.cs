using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WpfMath.Converters;
using WpfMath.Parsers;
using WpfMath.Rendering;
using XamlMath;

namespace WpfMath.Example;

public partial class MainWindow
{
    private readonly TexFormulaParser _formulaParser = WpfTeXFormulaParser.Instance;

    private static ComboBoxItem DemoFormula(string name, string text) =>
        new ComboBoxItem { Content = name, DataContext = text };

    private readonly IList<ComboBoxItem> _testFormulas = new[]
    {
        DemoFormula("Simple", @"\sqrt 2"),
        DemoFormula("Integral 1", @"\int_0^{\infty}{x^{2n} e^{-a x^2} \, dx} = \frac{2n-1}{2a} \int_0^{\infty}{x^{2(n-1)} e^{-a x^2} \, dx} = \frac{(2n-1)!!}{2^{n+1}} \sqrt{\frac{\pi}{a^{2n+1}}}"),
        DemoFormula("Integral 1", @"\int_0^{\infty}{x^{2n} e^{-a x^2} \, dx} = \frac{2n-1}{2a} \int_0^{\infty}{x^{2(n-1)} e^{-a x^2} \, dx} = \frac{(2n-1)!!}{2^{n+1}} \sqrt{\frac{\pi}{a^{2n+1}}}"),
        DemoFormula("Integral 2", @"\int_a^b{f(x) \, dx} = (b - a) \sum_{n = 1}^{\infty}  {\sum_{m = 1}^{2^n  - 1} { ( { - 1} )^{m + 1} } } 2^{ - n} f(a + m ( {b - a}  )2^{-n} )"),
        DemoFormula("Integral 3", @"L = \int_a^\infty \sqrt[4]{ \left\vert \sum_{i,j=1}^ng_{ij}\left\(\gamma(t)\right\) \left\[\frac{d}{dt}x^i\circ\gamma(t) \right\] \left\{\frac{d}{dt}x^j\circ\gamma(t) \right\} \right\|} \, dt"),
        DemoFormula("Number matrix", @"\matrix{4&78&3 \\ 5 & 9  & 82 }"),
        DemoFormula("Nested matrix", @"\matrix{4&78&3\\ 57 & {\matrix{78 \\ 12}}  & 20782 }"),
        DemoFormula("Cases", @"f(x) = \cases{1/3 & \mathrm{if} \thinspace 0\le x\le 1;\cr 2/3 & \mathrm{if} \thinspace 3\le x \le 4; \cr 0 & \mathrm{elsewhere.}\cr}"),
        DemoFormula("Matrix and new lines", @"v \times w = \left( \matrix{v_2 w_3 - v_3 w_2 \\ v_3 w_1 - v_1 w_3 \\ v_1 w_2 - v_2 w_1} \right) \\ \matrix{\mathrm{where} & v= \left(\matrix{ v_1 \\ v_2 \\ v_3 }\right), \\ & w= \left( \matrix{w_1 \\ w_2  \\ w_3} \right)}"),
        DemoFormula("Big matrix", @"\Gamma_{\mu \rho} ^{\sigma}= \pmatrix{\pmatrix{0 & 0 & 0 \\ 0 & -r & 0 \\ 0 & 0 & -r \sin^2(\theta)} \\ \pmatrix{0 & \frac{1}{r} & 0 \\ \frac{1}{r} & 0 & 0 \\ 0 & 0 & -\sin(\theta) \cos(\theta)} \\ \pmatrix{0 & 0 & \frac{1}{r} \\ 0 & 0 & \frac{1}{\tan(\theta)} \\ \frac{1}{r} & \frac{1}{\tan(\theta)} & 0 }}"),
        DemoFormula("Environment with Matrix", @"\begin{pmatrix} a_1 & a_2 & a_3 \\ b_1 & b_2 & b_3 \\ c_1 & c_2 & c_3 \end{pmatrix}")
    };

    public MainWindow()
    {
        InitializeComponent();

        FormulaSelector.ItemsSource = _testFormulas;
        FormulaSelector.SelectedIndex = 0;
    }

    private TexFormula? ParseFormula(string input)
    {
        // Create formula object from input text.
        TexFormula? formula = null;
        try
        {
            formula = this._formulaParser.Parse(input);
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
        var formula = ParseFormula(InputTextBox.Text);
        if (formula == null) return;
        var scale = Formula.Scale;
        var environment = WpfTeXEnvironment.Create(scale: scale);

        // Open stream
        var filename = saveFileDialog.FileName;
        using var stream = new FileStream(filename, FileMode.Create);
        switch (saveFileDialog.FilterIndex)
        {
            case 1:
                var geometry = formula.RenderToGeometry(environment, scale: scale);
                var converter = new SVGConverter();
                var svgPathText = converter.ConvertGeometry(geometry);
                var svgText = AddSvgHeader(svgPathText);
                using (var writer = new StreamWriter(stream))
                    writer.WriteLine(svgText);
                break;

            case 2:
                var bitmap = formula.RenderToBitmap(environment, scale, dpi: 300);
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

    private string AddSvgHeader(string svgText)
    {
        var builder = new StringBuilder();
        builder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>")
            .AppendLine("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" >")
            .AppendLine(svgText)
            .AppendLine("</svg>");

        return builder.ToString();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        //
    }

    private void inputTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        Formula.SelectionStart = InputTextBox.SelectionStart;
        Formula.SelectionLength = InputTextBox.SelectionLength;
    }

    private void FormulaTextBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = (ComboBoxItem) ((ComboBox) sender).SelectedItem;
        InputTextBox.Text = (string)item.DataContext;
    }
}
