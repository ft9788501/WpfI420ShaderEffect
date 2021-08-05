using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfI420ShaderEffect
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //MainWindow window = new MainWindow();
            //window.Show();
            //MainWindow1 window1 = new MainWindow1();
            //window1.Show();
            MainWindowStatic window = new MainWindowStatic();
            window.Show();
        }
    }
}
