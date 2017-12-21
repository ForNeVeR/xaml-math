using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using WpfMath;
using WpfMath.Exceptions;

namespace AvaloniaMath.Controls
{
    /// <summary>
    /// Interaction logic for <see cref="FormulaControl"/> xaml.
    /// </summary>
    public class FormulaControl : UserControl
    {
        private static TexFormulaParser formulaParser = new TexFormulaParser();
        private TexFormula texFormula;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaControl"/> class.
        /// </summary>
        public FormulaControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public string Formula
        {
            get { return (string)GetValue(FormulaProperty); }
            set
            {
                SetValue(FormulaProperty, value);
                RenderSettingsChanged();
            }
        }

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public string SystemTextFontName
        {
            get { return (string) GetValue(SystemTextFontNameProperty); }
            set
            {
                SetValue(SystemTextFontNameProperty, value);
                RenderSettingsChanged();
            }
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

        public static readonly StyledProperty<string> FormulaProperty =
            AvaloniaProperty.Register<FormulaControl, string>(
                "Formula", "", false, BindingMode.OneWay, CoerceFormula);

        public static readonly StyledProperty<double> ScaleProperty = AvaloniaProperty.Register<FormulaControl,double> (
            "Scale", 20d,false,BindingMode.Default, CoerceScaleValue);

        public static readonly AvaloniaProperty<string> SystemTextFontNameProperty =
            AvaloniaProperty.Register<FormulaControl, string>(
                nameof(SystemTextFontName), "Arial", false, BindingMode.Default);

        public static readonly AvaloniaProperty HasErrorProperty = AvaloniaProperty.Register<FormulaControl,bool>("HasError");

        public static readonly AvaloniaProperty ErrorsProperty = AvaloniaProperty.Register<FormulaControl,ObservableCollection<Exception>>(
            "Errors");

        public static readonly AvaloniaProperty ErrorTemplateProperty = AvaloniaProperty.Register<FormulaControl,ControlTemplate> 
        ("ErrorTemplate");

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var renderer = texFormula.GetRenderer(TexStyle.Display, Scale, SystemTextFontName);

         //   var container = this.FindControl<VisualContainerElement>("formulaContainerElement");
            renderer.Render(context, 0, 0);
        }

        private void Render1()
        {
            // Create formula object from input text.
            //if (texFormula == null)
            //{
            //    formulaContainerElement.Visual = null;
            //    return;
            //}


        }

        private static string CoerceFormula(AvaloniaObject d, string baseValue)
        {
            var control = (FormulaControl)d;
            var formula = baseValue;
            try
            {
                control.HasError = false;
               // control.Errors.Clear();
                control.texFormula = formulaParser.Parse(formula);
                return baseValue;
            }
            catch (TexException e)
            {
                control.SetError(e);
                return "";
            }
        }

        private static double CoerceScaleValue(AvaloniaObject d, double baseValue)
        {
            var val = baseValue;
            return val < 1d ? 1d : val;
        }

        private void RenderSettingsChanged()
        {
            try
            {
                Render1();
            }
            catch (TexException exception)
            {
                SetError(exception);
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
