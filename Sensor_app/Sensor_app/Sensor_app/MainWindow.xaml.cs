using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Emgu.CV.XImgproc;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util;
using Emgu.CV;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Timers;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Sensor_app
{
    
    public partial class MainWindow : Window
    {
        
        private bool _captureInProgress;
        private VideoCapture _capture = null;
        
        public MainWindow()
        {
            InitializeComponent();



            CvInvoke.UseOpenCL = false;
            try
            {
                _capture = new VideoCapture("https://192.168.0.6:8080/video");//
                _capture.ImageGrabbed += ProcessFrame;
            }
            catch (NullReferenceException excpt)
            {
                
            }
            
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            Mat frame = new Mat();
            _capture.Retrieve(frame, 0);
            Bitmap imgeOrigenal = frame.ToBitmap();
            captureImageBox.Image = imgeOrigenal;

        }


        private void captureButton_Click(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                if (_captureInProgress)
                {  //stop the capture

                    _capture.Pause();
                }
                else
                {
                    //start the capture

                    _capture.Start();
                }

                _captureInProgress = !_captureInProgress;
            }
        }

        public static int hmin = 0, smin = 0, vmin = 0;
        public static int hmax = 0, smax = 0, vmax = 0;
        private void Button_click1(object sender, RoutedEventArgs e)
        {
            Detection.Detect_color();
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            hmin = (int)slider1.Value;
            TB1.Text = ((int)slider1.Value).ToString();
            smin = (int)slider2.Value;
            TB2.Text = ((int)slider2.Value).ToString();
            vmin = (int)slider3.Value;
            TB3.Text = ((int)slider3.Value).ToString();
            hmax = (int)slider4.Value;
            TB4.Text = ((int)slider4.Value).ToString();
            smax = (int)slider5.Value;
            TB5.Text = ((int)slider5.Value).ToString();
            vmax = (int)slider6.Value;
            TB6.Text = ((int)slider6.Value).ToString();

            Detection.Detect_color();
        }
    }
}
