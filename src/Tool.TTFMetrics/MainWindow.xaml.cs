using System.Globalization;

namespace Tool.TTFMetrics;

public partial class MainWindow
{
    public MainWindow()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        InitializeComponent();
    }
}
