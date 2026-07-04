using System;

using UwpMath.Example.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace UwpMath.Example.Views;

public sealed partial class FormulaPresetView : UserControl
{
    public FormulaPresetView()
    {
        InitializeComponent();
    }

    public PresetViewModel? Preset
    {
        get => (PresetViewModel?)GetValue(PresetProperty);
        set => SetValue(PresetProperty, value);
    }

    public static readonly DependencyProperty PresetProperty = DependencyProperty.Register(
        nameof(Preset),
        typeof(PresetViewModel),
        typeof(FormulaPresetView),
        new PropertyMetadata(default(PresetViewModel))
    );

    private void OnSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        double fontSize = e.NewValue;
        fontSize = Math.Max(4, fontSize);
        fontSize = Math.Min(96, fontSize);
        PART_FormulaControl.FontSize = fontSize;
    }

    private void OnTextBoxSelectionChanged(object sender, RoutedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        PART_FormulaControl.SelectionStart = textBox.SelectionStart;
        PART_FormulaControl.SelectionLength = textBox.SelectionLength;
    }
}
