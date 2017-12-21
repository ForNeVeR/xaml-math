using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaMath.Example
{
    public class MainView : UserControl
    {
        public MainView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
