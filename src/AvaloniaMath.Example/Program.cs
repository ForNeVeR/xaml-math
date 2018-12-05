using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using AvaloniaMath.Example.ViewModels;
using AvaloniaMath.Example.Views;

namespace AvaloniaMath.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>(() => new MainWindowViewModel());
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                //.UsePlatformDetect()
                .UseDirect2D1()
                .UseWin32()
                .UseReactiveUI()
                .LogToDebug();
    }
}
