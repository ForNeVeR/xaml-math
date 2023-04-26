using System.Diagnostics;
using System.Windows;

namespace WpfMath.Example;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Trace.AutoFlush = true;
    }
}
