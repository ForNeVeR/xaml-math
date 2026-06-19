using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using System;
using System.Collections.Generic;

using Windows.Foundation;

using WinUIMath.Parsers;
using WinUIMath.Rendering;

using XamlMath;
using XamlMath.Boxes;

namespace WinUIMath.Controls;

[TemplatePart(Name = nameof(PART_CanvasWin2D), Type = typeof(CanvasControl))]
public sealed partial class FormulaControl : Control
{
    public FormulaControl()
    {
        DefaultStyleKey = typeof(FormulaControl);
    }

    public string? Formula
    {
        get => (string?)GetValue(FormulaProperty);
        set => SetValue(FormulaProperty, value);
    }

    public static readonly DependencyProperty FormulaProperty = DependencyProperty.Register(
        nameof(Formula),
        typeof(string),
        typeof(FormulaControl),
        new PropertyMetadata(string.Empty, (d, e) => ((FormulaControl)d).OnFormulaChanged(e))
    );

    private void OnFormulaChanged(DependencyPropertyChangedEventArgs e)
    {
        try
        {
            _texFormula = _formulaParser.Parse(Formula ?? string.Empty);
        }
        catch
        {
            _texFormula = null;
        }
        InvalidateMeasure();
        Render();
    }

    public int SelectionStart
    {
        get => (int)GetValue(SelectionStartProperty);
        set => SetValue(SelectionStartProperty, value);
    }

    public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register(
        nameof(SelectionStart),
        typeof(int),
        typeof(FormulaControl),
        new PropertyMetadata(default(int), (d, e) => ((FormulaControl)d).OnSelectionStartChanged(e))
    );

    private void OnSelectionStartChanged(DependencyPropertyChangedEventArgs e)
    {
        Render();
    }

    public int SelectionLength
    {
        get => (int)GetValue(SelectionLengthProperty);
        set => SetValue(SelectionLengthProperty, value);
    }

    public static readonly DependencyProperty SelectionLengthProperty = DependencyProperty.Register(
        nameof(SelectionLength),
        typeof(int),
        typeof(FormulaControl),
        new PropertyMetadata(default(int), (d, e) => ((FormulaControl)d).OnSelectionLengthChanged(e))
    );

    private void OnSelectionLengthChanged(DependencyPropertyChangedEventArgs e)
    {
        Render();
    }

    public Brush? SelectionBrush
    {
        get => (Brush?)GetValue(SelectionBrushProperty);
        set => SetValue(SelectionBrushProperty, value);
    }

    public static readonly DependencyProperty SelectionBrushProperty = DependencyProperty.Register(
        nameof(SelectionBrush),
        typeof(Brush),
        typeof(FormulaControl),
        new PropertyMetadata(default(Brush), (d, e) => ((FormulaControl)d).OnSelectionBrushChanged(e))
    );

    private void OnSelectionBrushChanged(DependencyPropertyChangedEventArgs e)
    {
        Render();
    }

    private static readonly TexFormulaParser _formulaParser = Win2DTeXFormulaParser.Instance;
    private TexFormula? _texFormula;
    private Box? _formulaBox;
    private CanvasControl? PART_CanvasWin2D;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        PART_CanvasWin2D = GetTemplateChild(nameof(PART_CanvasWin2D)) as CanvasControl;
        PART_CanvasWin2D?.Draw += OnCanvasDraw;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        PART_CanvasWin2D?.RemoveFromVisualTree();
    }

    private void Render()
    {
        PART_CanvasWin2D?.Invalidate();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (PART_CanvasWin2D is null || _texFormula is null)
            return new Size(0, 0);

        _formulaBox = GetBoxToRender();
        return _formulaBox is null ? new Size(0, 0) : new Size(_formulaBox.TotalWidth * FontSize, _formulaBox.TotalHeight * FontSize);
    }

    private void OnCanvasDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        _formulaBox ??= GetBoxToRender();
        if (_formulaBox is null)
            return;

        using CanvasDrawingSession session = args.DrawingSession;
        WinUIElementRenderer renderer = new(session) { Scale = FontSize };
        renderer.RenderElement(_formulaBox, 0, _formulaBox.Height);
        renderer.FinishRendering();
        _formulaBox = null;
    }

    private Box GetBoxToRender()
    {
        // Omit passing the background, since the control background will be effectively used anyway.
        TexEnvironment environment = WinUITeXEnvironment.Create(
            scale: FontSize,
            systemTextFontName: FontFamily.XamlAutoFontFamily.Source,
            foreground: Foreground);

        var formulaSource = _texFormula!.Source;
        var formulaBox = _texFormula!.CreateBox(environment);

        ProcessBox(formulaSource, formulaBox);

        return formulaBox;
    }

    private void ProcessBox(SourceSpan? formulaSource, Box formulaBox)
    {
        if (formulaSource == null)
            return;

        if (SelectionBrush == null)
            return;

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
                !source.Source.Equals(formulaSource.Source, StringComparison.Ordinal))
                continue;

            if (selectionStart < source.Start + source.Length
                && source.Start < selectionEnd
                && box is CharBox)
            {
                box.Background = SelectionBrush.ToPlatform();
            }
        }
    }
}
