using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfI420ShaderEffect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowStatic : Window
    {
        public static MainWindowStatic Main { get; set; }
        public MainWindowStatic()
        {
            InitializeComponent();
            Main = this;
            //VisualBrush visualBrush = new VisualBrush()
            //{
            //    Visual = new Rectangle()
            //    {
            //        Fill = Brushes.Red,
            //        Width = 1920,
            //        Height = 1080,
            //        Effect = new I420ShaderEffect()
            //    }
            //};
            //visualBrush.Viewport = new Rect(0, 0, 100, 100);
            //r1.Fill = visualBrush;
        }
    }
}
