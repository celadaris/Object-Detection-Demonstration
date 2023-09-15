using AForge.Video;
using AForge.Video.DirectShow;
using System.IO;

namespace Bo3AimBot
{
    public partial class Form1 : Form
    {
        ScreenCaptureStream stream;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // get entire desktop area size
            Rectangle screenArea = Rectangle.Empty;
            foreach (System.Windows.Forms.Screen screen in
                      System.Windows.Forms.Screen.AllScreens)
            {
                screenArea = Rectangle.Union(screenArea, screen.Bounds);
            }

            // create screen capture video source
            stream = new ScreenCaptureStream(screenArea, 16);

            // set NewFrame event handler
            stream.NewFrame += new NewFrameEventHandler(video_NewFrame);

            // start the video source
            stream.Start();

            //Start Predictions
            YoloCode.YoloPredict();

            //Move mouse to Target
            MouseAim.AimAtTarget();
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // get new frame
            Bitmap bitmap = VideoCaptureCode.MainCamHandler(eventArgs.Frame);
            pic.Image = new Bitmap(bitmap, new Size(bitmap.Width / 4, bitmap.Height / 4));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (stream.IsRunning)
            {
                MouseAim.tokenSource.Cancel();
                YoloCode.tokenSource.Cancel();
                stream.SignalToStop();
                stream.WaitForStop();
            }
            stream.NewFrame -= new NewFrameEventHandler(video_NewFrame);
        }
    }
}