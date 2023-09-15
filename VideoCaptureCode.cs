using OpenCvSharp.Extensions;
using OpenCvSharp;
using Yolov7net;

namespace Bo3AimBot
{
    internal class VideoCaptureCode
    {
        public static int imgCount { get; private set; }

        public static Bitmap currentPic { get; private set; }

        public static Bitmap MainCamHandler(Bitmap image)
        {
            if (imgCount >= 2)
            {
                imgCount = 0;
            }
            else
            {
                imgCount++;
            }
            currentPic = (Bitmap)image.Clone();
            return DisplayImage((Bitmap)image.Clone());
        }

        public static Bitmap DisplayImage(Bitmap currentPic)
        {
            Mat img = BitmapConverter.ToMat(currentPic);

            switch (YoloCode.predictCount)
            {
                case 0:
                    DrawBox(img, YoloCode.displayPrediction3);
                    break;
                case 1:
                    DrawBox(img, YoloCode.displayPrediction1);
                    break;
                case 2:
                    DrawBox(img, YoloCode.displayPrediction2);
                    break;
            }

            return BitmapConverter.ToBitmap(img);
        }

        public static void DrawBox(Mat img, List<YoloPrediction> predictionList)
        {
            foreach (var prediction in predictionList) // iterate predictions to draw results
            {
                int x = (int)Math.Round(prediction.Rectangle.X);
                int y = (int)Math.Round(prediction.Rectangle.Y);
                int width = (int)Math.Round(prediction.Rectangle.Width);
                int height = (int)Math.Round(prediction.Rectangle.Height);

                Cv2.Rectangle(img, new OpenCvSharp.Rect(x, y, width, height), new Scalar(0, 255, 0, 255), 3);
                Cv2.PutText(img, prediction.Label.Name, new OpenCvSharp.Point(x, y - 10), HersheyFonts.HersheySimplex, 1.5, new Scalar(255, 255, 255, 255), 4);

                Console.WriteLine(prediction.Label.Name);
            }
        }
    }
}