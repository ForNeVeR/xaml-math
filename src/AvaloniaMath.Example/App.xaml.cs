using Avalonia;
using Avalonia.Markup.Xaml;

namespace AvaloniaMath.Example
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
