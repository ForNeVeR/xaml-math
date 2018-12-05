using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using WpfMath;
using WpfMath.Exceptions;

namespace AvaloniaMath.Controls
{
    class FormulaBlock : Control
    {
        private static TexFormulaParser s_formulaParser = new TexFormulaParser();
        private TexFormula _texFormula;
        private TexRenderer _texRenderer;

        public static readonly StyledProperty<String> FormulaProperty =
            AvaloniaProperty.Register<FormulaBlock, string>(nameof(Formula),
                "", false, BindingMode.TwoWay);

        public static readonly StyledProperty<double> ScaleProperty = AvaloniaProperty.Register<FormulaBlock, double>(
            nameof(Scale), 20d, false, BindingMode.Default);

        public static readonly StyledProperty<string> SystemTextFontNameProperty =
            AvaloniaProperty.Register<FormulaBlock, string>(
                nameof(SystemTextFontName), "Arial", false, BindingMode.Default);

        public static readonly StyledProperty<bool> HasErrorProperty = AvaloniaProperty.Register<FormulaBlock, bool>(nameof(HasError));

        public static readonly StyledProperty<ObservableCollection<Exception>> ErrorsProperty = AvaloniaProperty.Register<FormulaBlock, ObservableCollection<Exception>>(
            "Errors");

        public static readonly StyledProperty<ControlTemplate> ErrorTemplateProperty = AvaloniaProperty.Register<FormulaBlock, ControlTemplate>
        ("ErrorTemplate");


        /// <summary>
        /// Initializes static members of the <see cref="FormulaBlock"/> class.
        /// </summary>
        static FormulaBlock()
        {
            ClipToBoundsProperty.OverrideDefaultValue<FormulaBlock>(true);
            
            AffectsRender<FormulaBlock>(
                FormulaProperty,
                ScaleProperty);
            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaBlock"/> class.
        /// </summary>
        public FormulaBlock()
        {
            Errors = new ObservableCollection<Exception>();
            Observable.Merge(
                this.GetObservable(FormulaProperty).Select(_ => Unit.Default),
                this.GetObservable(ScaleProperty).Select(_ => Unit.Default))
                .Subscribe(_ =>
                {
                    InvalidateFormattedText();
                    InvalidateMeasure();
                });

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
        
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (_texFormula != null)
            {
                _texRenderer.Render(context, 0, 0);
            }
        }

        
        /// <summary>
        /// Invalidates <see cref="FormattedText"/>.
        /// </summary>
        protected void InvalidateFormattedText()
        {
            try
            {
                // TODO only parse on formula change
                _texFormula = s_formulaParser.Parse(Formula);
                _texRenderer = _texFormula.GetRenderer(TexStyle.Display, Scale, SystemTextFontName);
        
                HasError = false;
                Errors.Clear();
               
            }
            catch (TexException e)
            {
                SetError(e);
            }           
        }

        /// <summary>
        /// Measures the control.
        /// </summary>
        /// <param name="availableSize">The available size for the control.</param>
        /// <returns>The desired size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_texFormula!=null)
            {
                return _texRenderer.RenderSize;
            }

            return new Size();
        }

        private void SetError(TexException exception)
        {
            Errors.Add(exception);
            HasError = true;
            _texFormula = null;
            _texRenderer = null;
        }
    }
}

