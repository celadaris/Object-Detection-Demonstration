using Yolov7net;

namespace Bo3AimBot
{
    internal class YoloCode
    {
        public static Task YoloPredictTask { get; set; }
        public static CancellationTokenSource tokenSource { get; set; }
        public static CancellationToken ct { get; set; }

        public static List<YoloPrediction> displayPrediction1 { get; set; } = new List<YoloPrediction>();
        public static List<YoloPrediction> displayPrediction2 { get; set; } = new List<YoloPrediction>();
        public static List<YoloPrediction> displayPrediction3 { get; set; } = new List<YoloPrediction>();

        public static List<YoloPrediction> aimPrediction1 { get; set; } = new List<YoloPrediction>();
        public static List<YoloPrediction> aimPrediction2 { get; set; } = new List<YoloPrediction>();
        public static List<YoloPrediction> aimPrediction3 { get; set; } = new List<YoloPrediction>();

        public static int predictCount { get; private set; }

        public static void YoloPredict()
        {
            tokenSource = new CancellationTokenSource();
            ct = tokenSource.Token;

            YoloPredictTask = Task.Run(() =>
            {
                // init Yolov7 with onnx (include nms results)file path
                using var yolo = new Yolov7("C:\\Users\\Tony\\source\\repos\\2023\\C# Machine Learning\\Computer Vision\\Useful ML Files\\resources\\yolov7-main\\runs\\train\\yolov7-custom3\\weights\\best.onnx", false);
                // setup labels of onnx model
                //yolo.SetupYoloDefaultLabels();
                yolo.SetupLabels(new string[] { "Zombie Body", "Zombie Head" });

                while (!ct.IsCancellationRequested)
                {
                    if (VideoCaptureCode.currentPic != null)
                    {
                        switch (predictCount)
                        {
                            case 0:
                                displayPrediction3 = yolo.Predict(VideoCaptureCode.currentPic, 0.4f, 0.5f);
                                aimPrediction3 = displayPrediction3;
                                break;
                            case 1:
                                displayPrediction1 = yolo.Predict(VideoCaptureCode.currentPic, 0.4f, 0.5f);
                                aimPrediction1 = displayPrediction1;
                                break;
                            case 2:
                                displayPrediction2 = yolo.Predict(VideoCaptureCode.currentPic, 0.4f, 0.5f);
                                aimPrediction2 = displayPrediction2;
                                break;
                        }
                    }


                    if (predictCount >= 2)
                    {
                        predictCount = 0;
                    }
                    else
                    {
                        predictCount++;
                    }
                }

            }, ct);

            YoloPredictTask.ContinueWith(t => { Console.WriteLine(t.Exception); },
        TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
