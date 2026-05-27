using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

using System;
using System.ComponentModel;

using WinUIMath.Example.ViewModels;

namespace WinUIMath.Example.Views;

public sealed partial class FormulaPresetView : UserControl, INotifyPropertyChanged
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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        double fontSize = e.NewValue;
        fontSize = Math.Max(4, fontSize);
        fontSize = Math.Min(96, fontSize);
        PART_FormulaControl.FontSize = fontSize;
    }
}
