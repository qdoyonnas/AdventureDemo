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

namespace WaywardEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public Viewbox mainView;
        public Canvas mainCanvas;

        public MainWindow()
        {
            InitializeComponent();

            //mainView = new Viewbox();
            //mainView.Stretch = Stretch.Uniform;

            mainCanvas = new Canvas();
            //mainCanvas.Background = Brushes.Yellow;
            //mainView.Child = mainCanvas;

            this.AddChild( mainCanvas );
        }

        public void Exit_Click( object sender, RoutedEventArgs e )
        {
            Application.Current.Shutdown();
        }
    }
}
