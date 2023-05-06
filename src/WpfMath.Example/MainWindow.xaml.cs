using System.Windows;

namespace WpfMath.Example;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnSelectionChanged(object sender, RoutedEventArgs e)
    {
        var formulaControl = FormulaControl;
        if (formulaControl is null)
            return;

        formulaControl.SelectionLength = 0;
        formulaControl.SelectionStart = InputTextBox.SelectionStart;
        formulaControl.SelectionLength = InputTextBox.SelectionLength;
    }
}
