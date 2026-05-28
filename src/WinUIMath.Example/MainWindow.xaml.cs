using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using WinUIMath.Example.ViewModels;

namespace WinUIMath.Example;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private MainViewModel ViewModel => field ?? new();

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        switch (((FrameworkElement)sender).ActualTheme)
        {
            case ElementTheme.Light:
                AppWindow.TitleBar.PreferredTheme = Microsoft.UI.Windowing.TitleBarTheme.Light;
                break;
            case ElementTheme.Dark:
                AppWindow.TitleBar.PreferredTheme = Microsoft.UI.Windowing.TitleBarTheme.Dark;
                break;
            default:
                break;
        }
    }

    private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
            return;

        PART_FormulaPresetView.Preset = args.SelectedItem as PresetViewModel;
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        switch (sender.ActualTheme)
        {
            case ElementTheme.Light:
                AppWindow.TitleBar.PreferredTheme = Microsoft.UI.Windowing.TitleBarTheme.Light;
                break;
            case ElementTheme.Dark:
                AppWindow.TitleBar.PreferredTheme = Microsoft.UI.Windowing.TitleBarTheme.Dark;
                break;
            default:
                break;
        }
    }
}
