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
using Brush = System.Windows.Media.Brush;

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

        private bool _Enable_while_capture;
        public bool Enable_while_capture
        {
            get { return _Enable_while_capture; }
            set { _Enable_while_capture = value; OnPropertyChanged("Enable_while_capture"); }
        }

        private string _Tb1_text;
        public string Tb1_text
        {
            get { return _Tb1_text; }
            set { _Tb1_text = value; OnPropertyChanged("Tb1_text"); }
        }
        private string _Tb2_text;
        public string Tb2_text
        {
            get { return _Tb2_text; }
            set { _Tb2_text = value; OnPropertyChanged("Tb2_text"); }
        }
        private string _Tb3_text;
        public string Tb3_text
        {
            get { return _Tb3_text; }
            set { _Tb3_text = value; OnPropertyChanged("Tb3_text"); }
        }
        private string _Tb4_text;
        public string Tb4_text
        {
            get { return _Tb4_text; }
            set { _Tb4_text = value; OnPropertyChanged("Tb4_text"); }
        }
        private string _Tb5_text;
        public string Tb5_text
        {
            get { return _Tb5_text; }
            set { _Tb5_text = value; OnPropertyChanged("Tb5_text"); }
        }
        private string _Tb6_text;
        public string Tb6_text
        {
            get { return _Tb6_text; }
            set { _Tb6_text = value; OnPropertyChanged("Tb6_text"); }
        }

        private string _IP_textbox;
        public string IP_textbox
        {
            get { return _IP_textbox; }
            set { _IP_textbox = value; OnPropertyChanged("IP_textbox"); }
        }

        private int _Tabindex;
        public int Tabindex
        {
            get { return _Tabindex; }
            set { _Tabindex = value; OnPropertyChanged("Tabindex"); }
        }

        private int _Selected_camera_type;
        public int Selected_camera_type
        {
            get { return _Selected_camera_type; }
            set { _Selected_camera_type = value; OnPropertyChanged("Selected_camera_type"); }
        }

        private Visibility _Background1_visibility;
        public Visibility Background1_visibility
        {
            get { return _Background1_visibility; }
            set { _Background1_visibility = value; OnPropertyChanged("Background1_visibility"); }
        }

        private Visibility _Background2_visibility;
        public Visibility Background2_visibility
        {
            get { return _Background2_visibility; }
            set { _Background2_visibility = value; OnPropertyChanged("Background2_visibility"); }
        }

        private Visibility _IPbox_visi;
        public Visibility IPbox_visi
        {
            get { return _IPbox_visi; }
            set { _IPbox_visi = value; OnPropertyChanged("IPbox_visi"); }
        }

        private Brush _Start_capture_color_bg;
        public Brush Start_capture_color_bg
        {
            get { return _Start_capture_color_bg; }
            set { _Start_capture_color_bg = value; OnPropertyChanged("Start_capture_color_bg"); }
        }

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
