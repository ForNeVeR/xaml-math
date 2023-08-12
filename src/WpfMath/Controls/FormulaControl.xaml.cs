using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfMath.Parsers;
using WpfMath.Rendering;
using XamlMath;
using XamlMath.Boxes;
using XamlMath.Exceptions;

namespace WpfMath.Controls
{
    /// <summary>
    /// Interaction logic for FormulaControl.xaml
    /// </summary>
    public partial class FormulaControl : UserControl
    {
        private static readonly TexFormulaParser _formulaParser = WpfTeXFormulaParser.Instance;
        private TexFormula? texFormula;

        public string Formula
        {
            get => (string)GetValue(FormulaProperty);
            set => SetValue(FormulaProperty, value);
        }

        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public string SystemTextFontName
        {
            get => (string)GetValue(SystemTextFontNameProperty);
            set => SetValue(SystemTextFontNameProperty, value);
        }

        public bool HasError
        {
            get => (bool)GetValue(HasErrorProperty);
            private set => SetValue(HasErrorProperty, value);
        }

        public ObservableCollection<Exception> Errors
        {
            get => (ObservableCollection<Exception>)GetValue(ErrorsProperty);
            private set => SetValue(ErrorsProperty, value);
        }

        public ControlTemplate ErrorTemplate
        {
            get => (ControlTemplate)GetValue(ErrorTemplateProperty);
            set => SetValue(ErrorTemplateProperty, value);
        }

        public int SelectionStart
        {
            get => (int)GetValue(SelectionStartProperty);
            set => SetValue(SelectionStartProperty, value);
        }

        public int SelectionLength
        {
            get => (int)GetValue(SelectionLengthProperty);
            set => SetValue(SelectionLengthProperty, value);
        }

        public Brush? SelectionBrush
        {
            get => (Brush)GetValue(SelectionBrushProperty);
            set => SetValue(SelectionBrushProperty, value);
        }

        public static readonly DependencyProperty FormulaProperty = DependencyProperty.Register(
            nameof(Formula), typeof(string), typeof(FormulaControl),
            new PropertyMetadata("", OnRenderSettingsChanged, CoerceFormula));

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
            nameof(Scale), typeof(double), typeof(FormulaControl),
            new PropertyMetadata(20d, OnRenderSettingsChanged, CoerceScaleValue));

        public static readonly DependencyProperty SystemTextFontNameProperty = DependencyProperty.Register(
            nameof(SystemTextFontName), typeof(string), typeof(FormulaControl),
            new PropertyMetadata("Arial", OnRenderSettingsChanged));

        public static readonly DependencyProperty HasErrorProperty = DependencyProperty.Register(
            nameof(HasError), typeof(bool), typeof(FormulaControl),
            new PropertyMetadata(false));

        public static readonly DependencyProperty ErrorsProperty = DependencyProperty.Register(
            nameof(Errors), typeof(ObservableCollection<Exception>), typeof(FormulaControl),
            new PropertyMetadata(new ObservableCollection<Exception>()));

        public static readonly DependencyProperty ErrorTemplateProperty = DependencyProperty.Register(
            nameof(ErrorTemplate), typeof(ControlTemplate), typeof(FormulaControl),
            new PropertyMetadata(new ControlTemplate()));

        public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register(
            nameof(SelectionStart), typeof(int), typeof(FormulaControl),
            new PropertyMetadata(0, OnRenderSettingsChanged));

        public static readonly DependencyProperty SelectionLengthProperty = DependencyProperty.Register(
            nameof(SelectionLength), typeof(int), typeof(FormulaControl),
            new PropertyMetadata(0, OnRenderSettingsChanged));

        public static readonly DependencyProperty SelectionBrushProperty = DependencyProperty.Register(
            nameof(SelectionBrush), typeof(Brush), typeof(FormulaControl),
            new PropertyMetadata(null, OnRenderSettingsChanged));

        static FormulaControl()
        {
            // Call OnRenderSettingsChanged on Foreground property change.
            ForegroundProperty.OverrideMetadata(
                typeof(FormulaControl),
                new FrameworkPropertyMetadata(
                    SystemColors.ControlTextBrush,
                    FrameworkPropertyMetadataOptions.Inherits,
                    OnRenderSettingsChanged));
        }

        public FormulaControl()
        {
            InitializeComponent();
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

            Box formulaBox = GetBoxToRender();

            using (var drawingContext = visual.RenderOpen())
            {
                var renderer = new WpfElementRenderer(drawingContext, Scale);
                renderer.RenderElement(formulaBox, 0, formulaBox.Height);
                renderer.FinishRendering();
            }

            formulaContainerElement.Visual = visual;
        }

        private Box GetBoxToRender()
        {
            // Omit passing the background, since the control background will be effectively used anyway.
            var environment = WpfTeXEnvironment.Create(
                scale: Scale,
                systemTextFontName: SystemTextFontName,
                foreground: Foreground);

            var formulaSource = texFormula!.Source; // TODO: Get rid of null forgiving operator
            var formulaBox = texFormula.CreateBox(environment);

            ProcessBox(formulaSource, formulaBox);

            return formulaBox;
        }

        private void ProcessBox(SourceSpan? formulaSource, Box formulaBox)
        {
            if (formulaSource == null) return;

            var selectionBrush = SelectionBrush;

            if (selectionBrush == null) return;

            var allBoxes = new List<Box> { formulaBox };
            var selectionStart = SelectionStart;
            var selectionEnd = selectionStart + SelectionLength;
            for (var idx = 0; idx < allBoxes.Count; idx++)
            {
                var box = allBoxes[idx];
                allBoxes.AddRange(box.Children);
                var source = box.Source;
                if (source == null ||
                    !source.SourceName.Equals(formulaSource.SourceName, StringComparison.Ordinal) ||
                    !source.Source.Equals(formulaSource.Source, StringComparison.Ordinal)) continue;

                if (selectionStart < source.Start + source.Length
                    && source.Start < selectionEnd
                    && box is CharBox)
                {
                    box.Background = WpfBrush.FromBrush(selectionBrush);
                }
            }
        }

        private static object CoerceFormula(DependencyObject d, object baseValue)
        {
            var control = (FormulaControl)d;
            var formula = (string)baseValue;
            try
            {
                control.HasError = false;
                control.Errors.Clear();
                control.texFormula = _formulaParser.Parse(formula);
                return baseValue;
            }
            catch (TexException e)
            {
                control.SetError(e);
                return "";
            }
            catch (Exception e)
            {
                control.SetError(new TexParseException("Parser crash! " + e.Message, e));
                return "";
            }
        }

        private static object CoerceScaleValue(DependencyObject d, object baseValue)
        {
            var val = (double)baseValue;
            return val < 1d ? 1d : val;
        }

        private static void OnRenderSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FormulaControl)d;
            try
            {
                control.Render();
            }
            catch (TexException exception)
            {
                control.SetError(exception);
            }
        }

        private void SetError(TexException exception)
        {
            Errors.Add(exception);
            HasError = true;
            texFormula = null;
        }
    }
}
