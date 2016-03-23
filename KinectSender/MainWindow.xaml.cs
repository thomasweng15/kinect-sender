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
using Microsoft.Kinect;

namespace KinectSender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor kinectSensor = null;

        private ColorModule colorModule = null;

        private bool isColorChecked = true; // TODO get this from settings file

        public MainWindow()
        {
            this.kinectSensor = KinectSensor.GetDefault();
            if (this.kinectSensor != null)
            {
                this.kinectSensor.Open();
            }
            else
            {
                return; // TODO set status text
            }

            this.colorModule = new ColorModule(this.kinectSensor);
            
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.colorModule.MainWindow_Loaded();
            this.ColorDisplay.Source = (this.isColorChecked) ? this.colorModule.getImageSource() : null;
        }

        private void Toggle_Color(object sender, RoutedEventArgs e)
        {
            this.isColorChecked = !this.isColorChecked;
            this.ColorDisplay.Source = (this.isColorChecked) ? this.colorModule.getImageSource() : null;
        }
    }
}
