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
        private MultiSourceFrameReader frameReader = null;
        private WriteableBitmap bitmap = null;
        private uint bitmapBackBufferSize = 0;
        private readonly int bytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
        private Rect displayRect;

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

            this.frameReader = this.kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color);
            FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);
            this.displayRect = new Rect(0.0, 0.0, colorFrameDescription.Width, colorFrameDescription.Height);
            this.bitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgra32, null);
            this.bitmapBackBufferSize = (uint)((this.bitmap.BackBufferStride * (this.bitmap.PixelHeight - 1)) + (this.bitmap.PixelWidth * this.bytesPerPixel));

            InitializeComponent();
        }

        public ImageSource getImageSource()
        {
            return this.bitmap;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.frameReader != null)
            {
                this.frameReader.MultiSourceFrameArrived += Reader_FrameArrived;
            }

            this.ColorDisplay.Source = this.getImageSource();
        }

        private void Reader_FrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            ColorFrame colorFrame = null;
            bool isBitmapLocked = false;
            MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();
            if (multiSourceFrame == null)
            {
                return;
            }

            try
            {
                colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame();
                if (colorFrame == null)
                {
                    return;
                }

                // lock the bitmap for writing
                this.bitmap.Lock();
                isBitmapLocked = true;

                colorFrame.CopyConvertedFrameDataToIntPtr(this.bitmap.BackBuffer, this.bitmapBackBufferSize, ColorImageFormat.Bgra);

                // we're done with the ColorFrame 
                colorFrame.Dispose();
                colorFrame = null;

                this.bitmap.AddDirtyRect(new Int32Rect(0, 0, this.bitmap.PixelWidth, this.bitmap.PixelHeight));

                this.bitmap.Unlock();
                isBitmapLocked = false;
            }
            finally
            {
                if (isBitmapLocked)
                {
                    this.bitmap.Unlock();
                }

                if (colorFrame != null)
                {
                    colorFrame.Dispose();
                }
            }
        }
    }
}
