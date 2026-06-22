using UwpMath.Example.ViewModels;

using Windows.UI.Xaml.Controls;

namespace UwpMath.Example;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
    }

    private MainViewModel ViewModel => field ?? new();

    private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
            return;

        PART_FormulaPresetView.Preset = args.SelectedItem as PresetViewModel;
    }
}
