using Avalonia;

namespace AvaloniaMath.Example
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .Start<MainWindow>();
            
        }
    }
}
