using System;
using System.Collections.ObjectModel;
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
        private TexFormula texFormula; 

        public string Formula
        {
            get { return (string)GetValue(FormulaProperty); }
            set { SetValue(FormulaProperty, value); }
        }

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public bool HasError
        {
            get { return (bool)GetValue(HasErrorProperty); }
            private set { SetValue(HasErrorProperty, value); }
        }

        public ObservableCollection<Exception> Errors
        {
            get { return (ObservableCollection<Exception>)GetValue(ErrorsProperty); }
            private set { SetValue(ErrorsProperty, value); }
        }

        public ControlTemplate ErrorTemplate
        {
            get { return (ControlTemplate)GetValue(ErrorTemplateProperty); }
            set { SetValue(ErrorTemplateProperty, value); }
        }

        public static readonly DependencyProperty FormulaProperty = DependencyProperty.Register(
            "Formula", typeof(string), typeof(FormulaControl), 
            new PropertyMetadata("", OnFormulaChanged, CoerceFormula));

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
            "Scale", typeof(double), typeof(FormulaControl),
            new PropertyMetadata(7d, OnScaleChanged));

        public static readonly DependencyProperty HasErrorProperty = DependencyProperty.Register(
            "HasError", typeof(bool), typeof(FormulaControl),
            new PropertyMetadata(false));

        public static readonly DependencyProperty ErrorsProperty = DependencyProperty.Register(
            "Errors", typeof(ObservableCollection<Exception>), typeof(FormulaControl),
            new PropertyMetadata(new ObservableCollection<Exception>()));

        public static readonly DependencyProperty ErrorTemplateProperty = DependencyProperty.Register(
            "ErrorTemplate", typeof(ControlTemplate), typeof(FormulaControl),
            new PropertyMetadata(new ControlTemplate()));

        public FormulaControl()
        {
            InitializeComponent();
            ErrorTemplate = Resources["DefaultErrorTemplate"] as ControlTemplate;
        }

        private void Render()
        {
            // Create formula object from input text.
            if (texFormula == null)
            {
                formulaContainerElement.Visual = null;
                return;
            }

            // Render formula to visual.
            var visual = new DrawingVisual();
            var renderer = texFormula.GetRenderer(TexStyle.Display, Scale);

            using (var drawingContext = visual.RenderOpen())
            {
                renderer.Render(drawingContext, 0, 0);
            }
            formulaContainerElement.Visual = visual;
        }

        private static object CoerceFormula(DependencyObject d, object baseValue)
        {
            var control = (FormulaControl)d;
            var formula = (string)baseValue;
            try
            {
                control.HasError = false;
                control.Errors.Clear();
                control.texFormula = formulaParser.Parse(formula);
                return baseValue;
            }
            catch (TexParseException e)
            {
                control.Errors.Add(e);
                control.HasError = true;
                control.texFormula = null;
                return "";
            }
        }

        private static void OnFormulaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FormulaControl)d;
            control.Render();
        }

        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FormulaControl)d;
            control.Render();
        }
    }
}
