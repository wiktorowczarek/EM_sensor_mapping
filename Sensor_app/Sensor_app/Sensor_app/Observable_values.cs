using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
//using Emgu.CV.XImgproc;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util;
using Emgu.CV;
using System.Windows;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Sensor_app
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private double _hmin;
        public double Hmin
        {
            get { return _hmin; }
            set
            {
                _hmin = value;
                OnPropertyChanged("Hmin");
            }
        }

        private ImageSource capturedImage;
        public ImageSource CapturedImage
        {
            get { return capturedImage; }
            set { capturedImage = value; OnPropertyChanged("CapturedImage"); }
        }

        private string _SensorData;
        public string SensorData
        {
            get { return _SensorData; }
            set { _SensorData = value; OnPropertyChanged("SensorData"); }
        }

        //private VideoCapture capture;
        //Mat CurrentFrame;
        //public void run()
        //{
        //    if (capture == null)
        //    {
        //        capture = new Emgu.CV.VideoCapture(0);
        //        CurrentFrame = new Mat();

        //    }
        //    capture.ImageGrabbed += VideoCapture_ImageGrabbed;
        //    capture.Start();

        //}
        //private void VideoCapture_ImageGrabbed(object sender, EventArgs e) // runs in worker thread
        //{
        //    capture.Retrieve(CurrentFrame);
        //    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        CapturedImage = ImageSourceFromBitmap(CurrentFrame.ToImage<Bgr, byte>().ToBitmap());
        //    }));
        //}

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
