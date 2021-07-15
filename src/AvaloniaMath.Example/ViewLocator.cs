using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AvaloniaMath.Example.ViewModels;

namespace AvaloniaMath.Example
{
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            if (!(data.GetType().FullName is { } fullName))
                return new TextBlock { Text = "Error: data doesn't have type fullName"};

            var name = fullName.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            return type is null
                ? new TextBlock { Text = "Not Found: " + name }
                : Activator.CreateInstance(type) is Control control
                    ? control : new TextBlock { Text = "Not Create Instance: " + type };
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}
