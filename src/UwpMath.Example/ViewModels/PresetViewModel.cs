using CommunityToolkit.Mvvm.ComponentModel;

using WinRT;

namespace UwpMath.Example.ViewModels;

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
