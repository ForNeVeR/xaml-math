using WinFormsMath.Controls;

namespace WinFormsApp.Example;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        var control = new FormulaControl();
        Controls.Add(control);

        control.Top = 5;
        control.Left = 5;
        control.Width = Width - 5;
        control.Height = Height - 5;
        control.FormulaText = @"\sqrt 2";
    }
}
