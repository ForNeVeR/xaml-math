using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace WpfMath.Example
{
    public partial class App : Application
    {
        public static new App Current
        {
            get { return Application.Current as App; }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Trace.AutoFlush = true;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
        }
    }
}
