using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using System.IO;
//using Emgu.CV.XImgproc;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util;

namespace Sensor_app
{
    class Detection
    {

        public static void Detect_color(string image_file)
        {
            string path = Environment.CurrentDirectory;
            string file = Path.Combine(path, "kolor4.png");


            try
            {
                Mat img = CvInvoke.Imread(image_file);
            
            Image<Hsv, byte> imgHSV = new Image<Hsv, byte>(400, 500);
            Image<Hsv, byte> imageHSVDest = new Image<Hsv, byte>(400, 500);

            CvInvoke.CvtColor(img, imgHSV, ColorConversion.Bgr2Hsv);
            

            MCvScalar lower = new MCvScalar(Sensor_app.MainWindow.hmin, Sensor_app.MainWindow.smin, Sensor_app.MainWindow.vmin);
            MCvScalar upper = new MCvScalar(Sensor_app.MainWindow.hmax, Sensor_app.MainWindow.smax, Sensor_app.MainWindow.vmax);
            CvInvoke.InRange(imgHSV, new ScalarArray(lower),
                           new ScalarArray(upper), imageHSVDest);

            CvInvoke.Imshow("ImageHSVDest", imageHSVDest);
            CvInvoke.Imshow("Image", img);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Image not chosen!");
            }
        }


        // 7,48,222,188,125,255 rozowy
        //86,148,85,207,241,253 czerw



    }
}
