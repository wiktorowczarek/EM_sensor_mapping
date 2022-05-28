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
                _capture = new VideoCapture("https://192.168.0.12:8080/video");//
                _capture.ImageGrabbed += ProcessFrame;
            }
            catch (NullReferenceException excpt)
            {
                
            }
            
        }
        Mat frame = new Mat();
        VectorOfVectorOfInt newPoints = new VectorOfVectorOfInt();
        int k = 0;
        List<System.Drawing.Point> PointList = new List<System.Drawing.Point>();

        private void ProcessFrame(object sender, EventArgs arg)
        {
            
            Mat frame2 = new Mat();
            //Mat imgGray=new Mat();
            //Mat imgCanny = new Mat();
            //Mat imgBlur = new Mat();
            //Mat kernel = new Mat();
            //Mat imgDilated = new Mat();
            //kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(3,3),new System.Drawing.Point(1,1));

            _capture.Retrieve(frame, 0);
            //CvInvoke.CvtColor(frame, imgGray, ColorConversion.Bgr2Gray);
            //CvInvoke.GaussianBlur(frame, imgBlur, new System.Drawing.Size(3, 3), 3, 0);
            //CvInvoke.Canny(imgBlur, imgCanny, 25, 75);
            //CvInvoke.Dilate(imgCanny, imgDilated, kernel, new System.Drawing.Point(-1, 1), 6, BorderType.Constant, new MCvScalar(255, 255, 255));
            //Bitmap imgeOrigenal = frame.ToBitmap();
            //Image<Hsv, byte> imgHSV = new Image<Hsv, byte>(400, 500);
            //Image<Hsv, byte> imageHSVDest = new Image<Hsv, byte>(400, 500);
            //CvInvoke.CvtColor(frame, imgHSV, ColorConversion.Bgr2Hsv);
            //// 7,48,222,188,125,255
            //MCvScalar lower = new MCvScalar(7, 48, 222);
            //MCvScalar upper = new MCvScalar(188, 125, 255);
            //CvInvoke.InRange(imgHSV, new ScalarArray(lower),
            //               new ScalarArray(upper), imageHSVDest);
            frame2 = findColor(frame);
            
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

                if (area > 100)
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

        private Mat findColor(Mat img)
        {
            Mat imgHSV = new Mat();
            Mat imgHSVDest = new Mat();
            CvInvoke.CvtColor(img, imgHSV, ColorConversion.Bgr2Hsv);
            // 7,48,222,188,125,255
            //86,148,85,207,241,253
            MCvScalar lower = new MCvScalar(86, 148, 85);
            MCvScalar upper = new MCvScalar(207, 241, 253);
            CvInvoke.InRange(imgHSV, new ScalarArray(lower),
                           new ScalarArray(upper), imgHSVDest);
            System.Drawing.Point myPoint = getContour(imgHSVDest);

            //VectorOfInt a = new VectorOfInt();
            //int[] b = ;
            //a.Push(b);
            
            
            if (myPoint.X != 0 && myPoint.Y != 0)
            {

                PointList.Add(myPoint);
                
                
                //newPoints.Push({ myPoint.X, myPoint.Y, 0});
                

            }
            for (int i = 0; i < PointList.Count; i++)
            {

                DrawOnCanvas(PointList[i]);
            }

            return imgHSVDest;
        }

        private void DrawOnCanvas(System.Drawing.Point Points)
        {
            
                CvInvoke.Circle(frame, Points, 2, new MCvScalar(255, 0, 255),10, LineType.Filled);
            

        }





    }
}
