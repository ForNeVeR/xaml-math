using CommunityToolkit.Mvvm.ComponentModel;

namespace WinUIMath.Example.ViewModels;

public partial class PresetViewModel : ObservableObject
{
    public PresetViewModel(string name, string formula)
    {
        Name = name;
        Formula = formula;
    }

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial string Formula { get; set; }
}
