using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using AvaloniaMath.Parsers;
using AvaloniaMath.Rendering;
using XamlMath.Boxes;
using XamlMath.Exceptions;
using XamlMath.Rendering;
using Size = Avalonia.Size;

namespace AvaloniaMath.Controls;

public class FormulaBlock : TemplatedControl
{
    private Box? _box;

    public static readonly StyledProperty<string> FormulaProperty =
        AvaloniaProperty.Register<FormulaBlock, string>(
            nameof(Formula), string.Empty, false, BindingMode.TwoWay);

    public static readonly StyledProperty<double> ScaleProperty =
        AvaloniaProperty.Register<FormulaBlock, double>(
            nameof(Scale), 20d, false, BindingMode.Default);

    public static readonly StyledProperty<string> SystemTextFontNameProperty =
        AvaloniaProperty.Register<FormulaBlock, string>(
            nameof(SystemTextFontName), "Arial", false, BindingMode.Default);

    public static readonly StyledProperty<bool> HasErrorProperty =
        AvaloniaProperty.Register<FormulaBlock, bool>(nameof(HasError));

    public static readonly StyledProperty<ObservableCollection<Exception>> ErrorsProperty =
        AvaloniaProperty.Register<FormulaBlock, ObservableCollection<Exception>>(nameof(Errors));

    public static readonly StyledProperty<ControlTemplate> ErrorTemplateProperty =
        AvaloniaProperty.Register<FormulaBlock, ControlTemplate>(nameof(ErrorTemplate));

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
        get => GetValue(FormulaProperty);
        set => SetValue(FormulaProperty, value);
    }

    public double Scale
    {
        get => GetValue(ScaleProperty);
        set => SetValue(ScaleProperty, value);
    }

    public string SystemTextFontName
    {
        get => GetValue(SystemTextFontNameProperty);
        set => SetValue(SystemTextFontNameProperty, value);
    }

    public bool HasError
    {
        get => GetValue(HasErrorProperty);
        private set => SetValue(HasErrorProperty, value);
    }

    public ObservableCollection<Exception> Errors
    {
        get => GetValue(ErrorsProperty);
        private set => SetValue(ErrorsProperty, value);
    }

    // TODO[#353]: Make it used
    public ControlTemplate ErrorTemplate
    {
        get => GetValue(ErrorTemplateProperty);
        set => SetValue(ErrorTemplateProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (_box == null) return;

        var renderer = new AvaloniaElementRenderer(context, Scale, Background, Foreground);
        TeXFormulaExtensions.Render(_box, renderer, 0.0, 0.0);
    }

    /// <summary>
    /// Invalidates <see cref="FormattedText"/>.
    /// </summary>
    protected void InvalidateFormattedText()
    {
        try
        {
            var formula = AvaloniaTeXFormulaParser.Instance.Parse(Formula);
            var texEnvironment = AvaloniaTeXEnvironment.Create(
                scale: Scale,
                systemTextFontName: SystemTextFontName);
            _box = formula.CreateBox(texEnvironment);

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
        return _box == null
            ? new Size()
            : new Size(_box.TotalWidth * Scale, _box.TotalHeight * Scale);
    }

    private void SetError(TexException exception)
    {
        Errors.Add(exception);
        HasError = true;
        _box = null;
    }
}

