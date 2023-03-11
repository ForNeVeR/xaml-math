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

namespace WpfMath.Controls;

public class FormulaControl : Control
{
    private static readonly TexFormulaParser _formulaParser = WpfTeXFormulaParser.Instance;
    private TexFormula? texFormula;
    private Size _bounds;

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

    public string SystemTextFontName
    {
        get => (string)GetValue(SystemTextFontNameProperty);
        set => SetValue(SystemTextFontNameProperty, value);
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

    public int SelectionStart
    {
        get { return (int)GetValue(SelectionStartProperty); }
        set { SetValue(SelectionStartProperty, value); }
    }

    public int SelectionLength
    {
        get { return (int)GetValue(SelectionLengthProperty); }
        set { SetValue(SelectionLengthProperty, value); }
    }

    public Brush? SelectionBrush
    {
        get => (Brush)GetValue(SelectionBrushProperty);
        set => SetValue(SelectionBrushProperty, value);
    }

    public static readonly DependencyProperty FormulaProperty = DependencyProperty.Register(
        nameof(Formula), typeof(string), typeof(FormulaControl),
        new FrameworkPropertyMetadata(
            "",
            FrameworkPropertyMetadataOptions.AffectsMeasure,
            null,
            coerceValueCallback: CoerceFormula));

    public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
        nameof(Scale), typeof(double), typeof(FormulaControl),
        new FrameworkPropertyMetadata(
            20d,
            FrameworkPropertyMetadataOptions.AffectsMeasure,
            null,
            coerceValueCallback: CoerceScaleValue));

    public static readonly DependencyProperty SystemTextFontNameProperty = DependencyProperty.Register(
        nameof(SystemTextFontName), typeof(string), typeof(FormulaControl),
        new FrameworkPropertyMetadata(
            "Arial",
            FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register(
        nameof(SelectionStart), typeof(int), typeof(FormulaControl),
        new FrameworkPropertyMetadata(
            0,
            FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty SelectionLengthProperty = DependencyProperty.Register(
        nameof(SelectionLength), typeof(int), typeof(FormulaControl),
        new FrameworkPropertyMetadata(
            0,
            FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty SelectionBrushProperty = DependencyProperty.Register(
        nameof(SelectionBrush), typeof(Brush), typeof(FormulaControl),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty HasErrorProperty = DependencyProperty.Register(
        nameof(HasError), typeof(bool), typeof(FormulaControl),
        new PropertyMetadata(false));

    public static readonly DependencyProperty ErrorsProperty = DependencyProperty.Register(
        nameof(Errors), typeof(ObservableCollection<Exception>), typeof(FormulaControl),
        new PropertyMetadata(new ObservableCollection<Exception>()));

    public static readonly DependencyProperty ErrorTemplateProperty = DependencyProperty.Register(
        nameof(ErrorTemplate), typeof(ControlTemplate), typeof(FormulaControl),
        new PropertyMetadata(new ControlTemplate()));

    static FormulaControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(FormulaControl),
            new FrameworkPropertyMetadata(typeof(FormulaControl)));
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        // Create formula object from input text.
        if (texFormula == null)
        {
            return;
        }

        // Omit passing the background, since the control background will be effectively used anyway.
        var environment = WpfTeXEnvironment.Create(
            scale: Scale,
            systemTextFontName: SystemTextFontName,
            foreground: Foreground);

        var formulaSource = texFormula.Source;
        if (formulaSource != null)
        {
            var selectionBrush = SelectionBrush;
            if (selectionBrush != null)
            {
                var formulaBox = texFormula.CreateBox(environment);
                var allBoxes = new List<Box>(formulaBox.Children);
                var selectionStart = SelectionStart;
                var selectionEnd = selectionStart+SelectionLength;
                for (var idx = 0; idx<allBoxes.Count; idx++)
                {
                    var box = allBoxes[idx];
                    allBoxes.AddRange(box.Children);
                    var source = box.Source;
                    if (source == null ||
                        !source.SourceName.Equals(formulaSource.SourceName, StringComparison.Ordinal) ||
                        !source.Source.Equals(formulaSource.Source, StringComparison.Ordinal)) continue;

                    if (selectionStart<source.Start+source.Length
                        && source.Start<selectionEnd
                        && box is CharBox)
                    {
                        box.Background = WpfBrush.FromBrush(selectionBrush);
                    }
                }
            }
        }

        _bounds = texFormula.RenderTo(drawingContext, environment, Scale);
    }

    protected override Size MeasureOverride(Size constraint)
    {
        using var context = new DrawingVisual().RenderOpen();

        return texFormula?.RenderTo(
            context,
            WpfTeXEnvironment.Create(
                scale: Scale,
                systemTextFontName: SystemTextFontName,
                foreground: Foreground),
            Scale) ?? new Size();
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
    }

    private static object CoerceScaleValue(DependencyObject d, object baseValue)
    {
        var val = (double)baseValue;
        return val < 1d ? 1d : val;
    }

    private void SetError(TexException exception)
    {
        Errors.Add(exception);
        HasError=true;
        texFormula=null;
    }
}
