using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using AvaloniaMath.Example.ViewModels;
using AvaloniaMath.Example.Views;

namespace AvaloniaMath.Example
{
    class Program
    {
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                //.UsePlatformDetect()
                .UseDirect2D1()
                .UseWin32()
                .UseReactiveUI()
                .LogToDebug();
    }
}
