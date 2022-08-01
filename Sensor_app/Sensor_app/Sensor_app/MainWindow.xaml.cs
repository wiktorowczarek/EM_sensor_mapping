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
using Emgu.CV.Util;
using System.Net.Sockets;
using System.Net;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Threading;

namespace Sensor_app
{
    
    public partial class MainWindow : Window
    {
        
        private bool _captureInProgress;
        private VideoCapture _capture = null;
        
        public MainWindow()
        {
            InitializeComponent();
            
            Connect_to_webcam();
            DataContext = this;
        }

        private void Connect_to_webcam()        {
            CvInvoke.UseOpenCL = false;           
            try
            {
                _capture = new VideoCapture("https://192.168.0.4:8080/video");//
                _capture.ImageGrabbed += ProcessFrame;
            }
            catch (NullReferenceException excpt)
            {

            }
        }


        public Mat frame = new Mat();
        VectorOfVectorOfInt newPoints = new VectorOfVectorOfInt();
        int k = 0;
        List<System.Drawing.Point> PointList = new List<System.Drawing.Point>();
        string Imagepath;
        bool symulation=false;
        bool CaptureCheck = false;
        private void ProcessFrame(object sender, EventArgs arg)
        {
            
            Mat frame2 = new Mat();
            
            _capture.Retrieve(frame, 0);
            
            findColor(frame, hmin, smin, vmin, hmax, smax, vmax);
            
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
            
            Imagepath =LoadChosenImage();
        }

        public string LoadChosenImage()
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Choose Image";
            theDialog.Filter = "PNG files|*.png";
            theDialog.InitialDirectory = Environment.CurrentDirectory; 
            if (theDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return theDialog.FileName;
            }
            else { return ""; }
            
        }



        private void Reconnect_Button_Click(object sender, RoutedEventArgs e)
        {
            Connect_to_webcam();
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

            Detection.Detect_color(Imagepath);
        }

        private void SaveFrame_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFrame();
        }

        private void Symulation_button_checked(object sender, RoutedEventArgs e)
        {
            symulation = true;
        }

        private void Symulation_button_unchecked(object sender, RoutedEventArgs e)
        {
            symulation = false;
        }

        private void DeleteContour_Button_Click(object sender, RoutedEventArgs e)
        {
            if(PointList!=null) PointList.Clear();

        }

        private void SaveImage_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveImage();
        }

        private System.Drawing.Point getContour(Mat imgDil)
        {
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            
            CvInvoke.FindContours(imgDil, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            System.Drawing.Point myPoint = new(0, 0);

            for(int i =0; i<contours.Size; i++)
            {
                int area =(int)CvInvoke.ContourArea(contours[i]);
                VectorOfVectorOfPoint conPoly = new VectorOfVectorOfPoint(contours.Size);
                VectorOfRect boundRect = new VectorOfRect(contours.Size);

                if (area > 1000)
                {
                    double peri = CvInvoke.ArcLength(contours[i], true);
                    CvInvoke.ApproxPolyDP(contours[i], conPoly[i], 0.02 * peri, true);
                    boundRect.Push(new System.Drawing.Rectangle[] { CvInvoke.BoundingRectangle(conPoly[i]) });

                    myPoint.X = boundRect[contours.Size].X + boundRect[contours.Size].Width / 2;
                    myPoint.Y = boundRect[contours.Size].Y + boundRect[contours.Size].Height / 2;
                    CvInvoke.DrawContours(frame, conPoly, i, new MCvScalar(255, 0, 255), 2);
                    //CvInvoke.Rectangle(frame, boundRect[i], new MCvScalar(0, 255, 255), 5);
                }


            }
            return myPoint;
        }

        private void DetectionCheck_button_checked(object sender, RoutedEventArgs e)
        {
            CaptureCheck = true;
        }

        private void DetectionCheck_button_unchecked(object sender, RoutedEventArgs e)
        {
            CaptureCheck = false;
        }

        private void Button_click3(object sender, RoutedEventArgs e)
        {
            Task<int> read_socket = null;
            read_socket = new Task<int>(() => { return ReadData(); });
            read_socket.Start();
        }

        private const int PORT = 80;
        private int ReadData()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                socket.Connect("192.168.0.13", PORT);
                if(socket.Connected) SensorData = "3";
                else SensorData = "0";
                string requestText = "GET /H";
                byte[] requestBytes = Encoding.UTF8.GetBytes(requestText);
                byte[] buffer = new byte[100];

                //string requestText = "GET / HTTP/1.1\r\n" +
                //                     "Host: 192.168.0.13\r\n";
                while (true)
                {
                    
                    socket.Send(requestBytes);
                    
                    int iRx = socket.Receive(buffer);
                    byte a = buffer[0];
                    SensorData = a.ToString();
                    Thread.Sleep(100);
                }
            }
            catch { }
            return 0;
            //while (true)
            //{

            //}

        }
 
        private void findColor(Mat img, int hmin, int smin, int vmin, int hmax, int smax, int vmax)
        {
            Mat imgHSV = new Mat();
            Mat imgHSVDest = new Mat();
            CvInvoke.CvtColor(img, imgHSV, ColorConversion.Bgr2Hsv);
            // 7,48,222,188,125,255
            //86,148,85,207,241,253
            //31,66,130,237,238,253
            MCvScalar lower = new MCvScalar(hmin, smin, vmin);
            MCvScalar upper = new MCvScalar(hmax, smax, vmax);
            CvInvoke.InRange(imgHSV, new ScalarArray(lower),
                           new ScalarArray(upper), imgHSVDest);

            if (symulation == true ||CaptureCheck==true)
            {
                System.Drawing.Point myPoint = getContour(imgHSVDest);




                if (myPoint.X != 0 && myPoint.Y != 0 && CaptureCheck==false)
                {
                    PointList.Add(myPoint);
                }
            }


            for (int i = 0; i < PointList.Count; i++)
            {
                DrawOnCanvas(PointList[i]);
            }

            
        }

        private void DrawOnCanvas(System.Drawing.Point Points)
        {    
                CvInvoke.Circle(frame, Points, 2, new MCvScalar(255, 0, 255),10, LineType.Filled);
        }

        private void SaveFrame()
        {

            Bitmap imgeOrigenal = frame.ToBitmap();
            string path = Environment.CurrentDirectory;
            string file = System.IO.Path.Combine(path, "SavedFrame.png");
            imgeOrigenal.Save(file);

        }

        private void SaveImage()
        {
            Bitmap imgeOrigenal = frame.ToBitmap();
            string path = Environment.CurrentDirectory;
            string file = System.IO.Path.Combine(path, "SavedImage.png");

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "png files (*.png)|*.png";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file = saveFileDialog1.FileName;
                
            }
            imgeOrigenal.Save(file);
        }

    }
}
