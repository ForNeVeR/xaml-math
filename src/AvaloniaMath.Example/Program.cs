using System.Diagnostics;
using Avalonia;
using Avalonia.ReactiveUI;

namespace AvaloniaMath.Example
{
    class Program
    {
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
#if NETCOREAPP
                .UsePlatformDetect()
#else
                .UseDirect2D1()
                .UseWin32()
#endif
                .UseReactiveUI()
                .LogToTrace();
        }
    }
}
