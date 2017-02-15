using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfMath.Controls
{
    /// <summary>
    /// Interaction logic for FormulaControl.xaml
    /// </summary>
    public partial class FormulaControl : UserControl
    {
        private static TexFormulaParser formulaParser = new TexFormulaParser();

        public string Formula
        {
            get { return (string) GetValue(FormulaProperty); }
            set { SetValue(FormulaProperty, value); }
        }

        public static readonly DependencyProperty FormulaProperty = DependencyProperty.Register(
          "Formula", typeof(string), typeof(FormulaControl), new PropertyMetadata("", OnFormulaChanged), ValidateFormula);

        public FormulaControl()
        {
            InitializeComponent();
        }

        private void Render()
        {
            // Create formula object from input text.
            var formula = formulaParser.Parse(Formula);
            if (formula == null) return;

            // Render formula to visual.
            var visual = new DrawingVisual();
            var renderer = formula.GetRenderer(TexStyle.Display, 20d);
            var formulaSize = renderer.RenderSize;

            using (var drawingContext = visual.RenderOpen())
            {
                renderer.Render(drawingContext, 0, 0);
            }
            formulaContainerElement.Visual = visual;
        }

        private static bool ValidateFormula(object value)
        {
            var formula = (string) value;
            try
            {
                formulaParser.Parse(formula);
                return true;
            }
            catch { return false; }
        }

        private static void OnFormulaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FormulaControl) d;
            control.Render();
        }
    }
}
