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
            AffectsRender<FormulaControl>(FormulaProperty,ScaleProperty);
            this.InitializeComponent();
            this.GetObservable(FormulaProperty).Subscribe(
                x =>
                {
                    try
                    {
                        texFormula = formulaParser.Parse(x);
                    }
                    catch (Exception e)
                    {

                    }
                }
                );
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
            get { return GetValue(FormulaProperty); }
            set { SetValue(FormulaProperty, value); }
        }

        public double Scale
        {
            get { return GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public string SystemTextFontName
        {
            get { return GetValue(SystemTextFontNameProperty); }
            set { SetValue(SystemTextFontNameProperty, value); }
        }

        public bool HasError
        {
            get { return GetValue(HasErrorProperty); }
            private set { SetValue(HasErrorProperty, value); }
        }

        public ObservableCollection<Exception> Errors
        {
            get { return GetValue(ErrorsProperty); }
            private set { SetValue(ErrorsProperty, value); }
        }

        public ControlTemplate ErrorTemplate
        {
            get { return GetValue(ErrorTemplateProperty); }
            set { SetValue(ErrorTemplateProperty, value); }
        }

       // public static readonly StyledProperty<string> FormulaProperty =
       //     AvaloniaProperty.Register<FormulaControl, string>(
       //         "Formula", "", false, BindingMode.OneWay, validate:CoerceFormula);

        public static readonly StyledProperty<String> FormulaProperty =
            AvaloniaProperty.Register<FormulaControl, string>(nameof(Formula),
                "x",false,BindingMode.TwoWay);

        public static readonly StyledProperty<double> ScaleProperty = AvaloniaProperty.Register<FormulaControl,double> (
            nameof(Scale), 20d,false,BindingMode.Default, CoerceScaleValue);

        public static readonly StyledProperty<string> SystemTextFontNameProperty =
            AvaloniaProperty.Register<FormulaControl, string>(
                nameof(SystemTextFontName), "Arial", false, BindingMode.Default);

        public static readonly StyledProperty<bool> HasErrorProperty = AvaloniaProperty.Register<FormulaControl,bool>(nameof(HasError));

        public static readonly StyledProperty<ObservableCollection<Exception>> ErrorsProperty = AvaloniaProperty.Register<FormulaControl,ObservableCollection<Exception>>(
            "Errors");

        public static readonly StyledProperty<ControlTemplate> ErrorTemplateProperty = AvaloniaProperty.Register<FormulaControl,ControlTemplate> 
        ("ErrorTemplate");

        public override void Render(DrawingContext context)
        {
            base.Render(context);

           // texFormula = formulaParser.Parse(Formula);
            if (texFormula != null)
            {
                var renderer = texFormula.GetRenderer(TexStyle.Display, Scale, SystemTextFontName);

                //   var container = this.FindControl<VisualContainerElement>("formulaContainerElement");
                renderer.Render(context, 0, 0);
            }
        }

        /*
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
        */
        private static double CoerceScaleValue(AvaloniaObject d, double baseValue)
        {
            var val = baseValue;
            return val < 1d ? 1d : val;
        }
       
        private void SetError(TexException exception)
        {
            Errors.Add(exception);
            HasError = true;
            texFormula = null;
        }
    }
}
