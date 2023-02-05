using System.Diagnostics;
using System.Windows;

namespace WpfMath.Example;

public partial class App
{
    public static new App Current
    {
        get { return (App) Application.Current; }
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        Trace.AutoFlush = true;
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
    }
}
